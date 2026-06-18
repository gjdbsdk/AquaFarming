using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    private float directionTimer = 0f;
    public float changeDirectionTime = 3.0f; 

    private float currentSpeedY = 0f;
    private float currentSpeedZ = 0f;

    private Vector3 originalScale;

    private float sizeModifier = 1.0f; 
    private float customTimer = 0f;
    private bool isEnlarged = false; 
    private int foodEatenCount = 0;

    private float hungerTimer = 0f;
    public bool isFull = false;

    private float dirtyWaterTimer = 0f;

    public float minZ = -0.068f;
    public float maxZ= 0.485f;
    public float minY = 0.684f;
    public float maxY = 0.803f;

    private List<Material> cachedMaterials = new List<Material>();
    private Color[] originalColors;

    private bool isIdling = false; 
    private float bugCheckTimer = 0f;
    private bool isWaterCompletelyDirty = false;

    public AudioClip eatSound;  
    public AudioClip deathSound; 
    private AudioSource audioSource;

    void Start()
    {
        originalScale = transform.localScale;
        Renderer[] fishRenderers = GetComponentsInChildren<Renderer>();

        if (fishRenderers != null)
        {
            foreach (Renderer r in fishRenderers)
            {
                if (r != null)
                {
                    foreach (Material mat in r.materials)
                    {
                        if (mat != null) cachedMaterials.Add(mat);
                    }
                }
            }

            originalColors = new Color[cachedMaterials.Count];
            for (int i = 0; i < cachedMaterials.Count; i++)
            {
                originalColors[i] = cachedMaterials[i].color;
            }
        }

        isIdling = true;
        changeDirectionTime = Random.Range(1.5f, 3.5f);
        currentSpeedY = 0f;
        currentSpeedZ = 0f;
        directionTimer = 0f;

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isIdling)
        {
            Vector3 movement = new Vector3(0f, currentSpeedY * Time.deltaTime, currentSpeedZ * Time.deltaTime);
            transform.Translate(movement);
        }

        KeepInsideTank();

        directionTimer += Time.deltaTime;
        if (directionTimer >= changeDirectionTime)
        {
            DecideNextAction();
            directionTimer = 0f; 
        }

        bugCheckTimer += Time.deltaTime;
        if (bugCheckTimer >= 0.2f) 
        {
            bugCheckTimer = 0f;
            isWaterCompletelyDirty = false;
            GameObject bug = GameObject.FindWithTag("Bug");
            
            if (bug != null)
            {
                WaterBug bugScript = bug.GetComponent<WaterBug>();
                if (bugScript != null && bugScript.GetLifeTime() >= 5.0f)
                {
                    isWaterCompletelyDirty = true;
                }
            }
        }

        if (isWaterCompletelyDirty)
        {
            dirtyWaterTimer += Time.deltaTime;

            if (dirtyWaterTimer < 5.0f)
            {
                float dirtyLerpPercent = dirtyWaterTimer / 5.0f;
                ChangeFishColorLerp(Color.red, dirtyLerpPercent);
            }
            else
            {
                TriggerDeathSoundAndDestroy();
                return;
            }
        }
        else
        {
            if (dirtyWaterTimer > 0f)
            {
                dirtyWaterTimer = 0f;
                ResetToOriginalColor();
            }
        }

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
                    TriggerDeathSoundAndDestroy();
                    return;
                }
            }
        }
        else
        {
            hungerTimer = 0f;
            customTimer = 0f;
        }
    }

    void KeepInsideTank()
    {
        Vector3 currentPos = transform.position;
        bool isHitWall = false;

        if (currentPos.z < minZ)
        {
            currentPos.z = minZ;
            currentSpeedZ = Mathf.Abs(currentSpeedZ) > 0.001f ? Mathf.Abs(currentSpeedZ) : 0.1f;
            isHitWall = true;
        }
        else if (currentPos.z > maxZ)
        {
            currentPos.z = maxZ;
            currentSpeedZ = Mathf.Abs(currentSpeedZ) > 0.001f ? -Mathf.Abs(currentSpeedZ) : -0.1f;
            isHitWall = true;
        }

        if (currentPos.y < minY)
        {
            currentPos.y = minY;
            currentSpeedY = Mathf.Abs(currentSpeedY) > 0.001f ? Mathf.Abs(currentSpeedY) : 0.04f;
            isHitWall = true;
        }
        else if (currentPos.y > maxY)
        {
            currentPos.y = maxY;
            currentSpeedY = Mathf.Abs(currentSpeedY) > 0.001f ? -Mathf.Abs(currentSpeedY) : -0.04f;
            isHitWall = true;
        }

        transform.position = currentPos;

        if (isHitWall)
        {
            ApplyScale();
        }
    }

    void DecideNextAction()
    {
        isIdling = Random.Range(0, 2) == 0;

        if (isIdling)
        {
            changeDirectionTime = Random.Range(1.5f, 3.5f);
            currentSpeedY = 0f;
            currentSpeedZ = 0f;
        }
        else
        {
            changeDirectionTime = Random.Range(2.5f, 4.5f);
            ChangeRandomDirection();
        }
    }

    public void EatFood()
    {
        if (foodEatenCount >= 5) return; 

        foodEatenCount += 1;
        sizeModifier *= 1.1f;

        isEnlarged = true;
        isFull = true;   
        customTimer = 0f; 
        hungerTimer = 0f; 

        if (audioSource != null && eatSound != null)
        {
            audioSource.clip = eatSound;
            audioSource.Play();
        }

        GameObject bug = GameObject.FindWithTag("Bug");
        bool isDirty = false;
        if (bug != null)
        {
            WaterBug bugScript = bug.GetComponent<WaterBug>();
            if (bugScript != null && bugScript.GetLifeTime() >= 5.0f) isDirty = true;
        }

        if (!isDirty) ResetToOriginalColor();

        ApplyScale();
    }

    void TriggerDeathSoundAndDestroy()
    {
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }
        Destroy(gameObject);
    }

    void ChangeFishColorLerp(Color targetColor, float percent)
    {
        if (cachedMaterials != null && originalColors != null)
        {
            for (int i = 0; i < cachedMaterials.Count; i++)
            {
                if (cachedMaterials[i] != null)
                {
                    cachedMaterials[i].color = Color.Lerp(originalColors[i], targetColor, percent);
                }
            }
        }
    }

    void ResetToOriginalColor()
    {
        if (cachedMaterials != null && originalColors != null)
        {
            for (int i = 0; i < cachedMaterials.Count; i++)
            {
                if (cachedMaterials[i] != null)
                {
                    cachedMaterials[i].color = originalColors[i];
                }
            }
        }
    }

    void ApplyScale()
    {
        float directionSign = -1.0f; 

        if (Mathf.Abs(currentSpeedZ) > 0.001f)
        {
            directionSign = (currentSpeedZ < 0) ? 1.0f : -1.0f;
        }
        else
        {
            directionSign = (transform.localScale.z < 0f) ? -1.0f : 1.0f;
        }

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