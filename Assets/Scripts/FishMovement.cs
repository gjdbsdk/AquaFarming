using UnityEngine;

public class FishMovement : MonoBehaviour
{
    private float directionTimer = 0f;
    public float changeDirectionTime = 3.0f; 

    private float currentSpeedY = 0f;
    private float currentSpeedZ = 0f;

    private Vector3 originalScale;

    [Header("--- 물고기 크기 및 식사 제한 ---")]
    private float sizeModifier = 1.0f; 
    private float customTimer = 0f;
    private bool isEnlarged = false; 
    private int foodEatenCount = 0;

    [Header("--- 굶주림 및 사망 타이머 (공복용) ---")]
    private float hungerTimer = 0f;
    public bool isFull = false;

    [Header("--- 🚨 오염 사망 타이머 (밥먹기와 완전히 독립!) ---")]
    private float dirtyWaterTimer = 0f;

    private Renderer[] fishRenderers;
    private Color[] originalColors;

    void Start()
    {
        originalScale = transform.localScale;
        fishRenderers = GetComponentsInChildren<Renderer>();

        if (fishRenderers != null)
        {
            int totalMaterialCount = 0;
            foreach (Renderer r in fishRenderers)
            {
                if (r != null) totalMaterialCount += r.materials.Length;
            }

            originalColors = new Color[totalMaterialCount];

            int index = 0;
            foreach (Renderer r in fishRenderers)
            {
                if (r != null)
                {
                    foreach (Material mat in r.materials)
                    {
                        if (mat != null)
                        {
                            originalColors[index] = mat.color; 
                            index++;
                        }
                    }
                }
            }
        }

        ChangeRandomDirection();
    }

    void Update()
    {
        // 1. 이동 처리
        Vector3 movement = new Vector3(0f, currentSpeedY * Time.deltaTime, currentSpeedZ * Time.deltaTime);
        transform.Translate(movement);

        // 2. 3초 방향 전환 타이머
        directionTimer += Time.deltaTime;
        if (directionTimer >= changeDirectionTime)
        {
            ChangeRandomDirection();
            directionTimer = 0f; 
        }


        // =======================================================================
        // 🚨 [핵심 수정] 파트 A: 오염 사망 시스템 (밥과 100% 무관하게 상시 감시)
        // =======================================================================
        bool isWaterCompletelyDirty = false;
        GameObject bug = GameObject.FindWithTag("Bug");
        
        if (bug != null)
        {
            WaterBug bugScript = bug.GetComponent<WaterBug>();
            if (bugScript != null)
            {
                // 물벌레 방치 5초 경과 = 물 오염 확정
                if (bugScript.GetLifeTime() >= 5.0f)
                {
                    isWaterCompletelyDirty = true;
                }
            }
        }

        if (isWaterCompletelyDirty)
        {
            dirtyWaterTimer += Time.deltaTime;

            // 크기 변화 없이 5초 동안 빨개짐
            if (dirtyWaterTimer < 5.0f)
            {
                float dirtyLerpPercent = dirtyWaterTimer / 5.0f;
                ChangeFishColorLerp(Color.red, dirtyLerpPercent);
            }
            // 5초 지나면 사망!
            else
            {
                Destroy(gameObject);
                return; // 파괴되었으므로 이번 프레임 Update 종료
            }
        }
        else
        {
            // 벌레를 잡아서 물이 다시 맑아졌다면 오염 타이머를 부드럽게 리셋
            if (dirtyWaterTimer > 0f)
            {
                dirtyWaterTimer = 0f;
                ResetToOriginalColor();
            }
        }


        // =======================================================================
        // 🚨 [핵심 수정] 파트 B: 일반 소화 및 공복 타이머 (물이 맑을 때만 정상 작동)
        // =======================================================================
        if (!isWaterCompletelyDirty)
        {
            if (isEnlarged)
            {
                customTimer += Time.deltaTime;

                if (customTimer >= 10.0f)
                {
                    foodEatenCount -= 1;
                    sizeModifier /= 1.1f;
                    customTimer = 0f;

                    if (foodEatenCount <= 0)
                    {
                        foodEatenCount = 0;
                        sizeModifier = 1.0f;
                        isEnlarged = false;
                        isFull = false; 
                    }
                    
                    ApplyScale(); 
                }
            }
            else
            {
                hungerTimer += Time.deltaTime;

                if (hungerTimer >= 5.0f && hungerTimer < 10.0f)
                {
                    float lerpPercent = (hungerTimer - 5.0f) / 5.0f;
                    ChangeFishColorLerp(Color.red, lerpPercent);
                }
                else if (hungerTimer >= 10.0f)
                {
                    Destroy(gameObject); 
                }
            }
        }
        else
        {
            // 물이 오염된 동안에는 일반 공복/소화 타이머가 흘러가지 않고 멈춥니다.
            // (오염 사망 타이머가 우선권을 가집니다)
            hungerTimer = 0f;
            customTimer = 0f;
        }
    }

    public void EatFood()
    {
        if (foodEatenCount >= 5)
        {
            return; 
        }

        foodEatenCount += 1;
        sizeModifier *= 1.1f;

        isEnlarged = true;
        isFull = true;   
        customTimer = 0f; 
        hungerTimer = 0f; 

        // 🚨 단, 물이 오염된 상태라면 먹어도 원래 색으로 돌아가지 않고 오염 필터 색을 유지해야 하므로
        // 물이 깨끗할 때만 색상을 초기화해 줍니다.
        GameObject bug = GameObject.FindWithTag("Bug");
        bool isDirty = false;
        if (bug != null)
        {
            WaterBug bugScript = bug.GetComponent<WaterBug>();
            if (bugScript != null && bugScript.GetLifeTime() >= 5.0f) isDirty = true;
        }

        if (!isDirty)
        {
            ResetToOriginalColor();
        }

        ApplyScale();
    }

    void ChangeFishColorLerp(Color targetColor, float percent)
    {
        if (fishRenderers != null && originalColors != null)
        {
            int index = 0;
            foreach (Renderer r in fishRenderers)
            {
                if (r != null)
                {
                    foreach (Material mat in r.materials)
                    {
                        if (mat != null)
                        {
                            mat.color = Color.Lerp(originalColors[index], targetColor, percent);
                            index++;
                        }
                    }
                }
            }
        }
    }

    void ResetToOriginalColor()
    {
        if (fishRenderers != null && originalColors != null)
        {
            int index = 0;
            foreach (Renderer r in fishRenderers)
            {
                if (r != null)
                {
                    foreach (Material mat in r.materials)
                    {
                        if (mat != null)
                        {
                            mat.color = originalColors[index]; 
                            index++;
                        }
                    }
                }
            }
        }
    }

    void ApplyScale()
    {
        float directionSign = (currentSpeedZ < 0) ? 1.0f : -1.0f;

        float finalX = originalScale.x * sizeModifier;
        float finalY = originalScale.y * sizeModifier;
        float finalZ = originalScale.z * sizeModifier * directionSign;

        transform.localScale = new Vector3(finalX, finalY, finalZ);
    }

    void ChangeRandomDirection()
    {
        float rawSpeedZ = Random.Range(0.05f, 0.15f);
        currentSpeedZ = Random.Range(0, 2) == 0 ? rawSpeedZ : -rawSpeedZ;

        float rawSpeedY = Random.Range(0.02f, 0.06f);
        currentSpeedY = Random.Range(0, 2) == 0 ? rawSpeedY : -rawSpeedY;

        ApplyScale();
    }
}