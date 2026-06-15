using UnityEngine;

public class WaterFilter : MonoBehaviour
{
    private Renderer waterRenderer;
    private Color originalWaterColor;

    void Start()
    {
        // 내 오브젝트의 Renderer와 처음 깨끗했던 물 색상을 기억해둡니다.
        waterRenderer = GetComponent<Renderer>();
        if (waterRenderer != null)
        {
            originalWaterColor = waterRenderer.material.color;
        }
    }

    void Update()
    {
        // 1. 현재 씬에 살아있는 물벌레가 있는지 찾아봅니다.
        GameObject bug = GameObject.FindWithTag("Bug");

        // 2. 만약 물벌레가 어항에 존재한다면?
        if (bug != null)
        {
            WaterBug bugScript = bug.GetComponent<WaterBug>();
            if (bugScript != null)
            {
                // 물벌레가 태어난 지 몇 초나 지났는지 수치를 받아옵니다.
                float bugAge = bugScript.GetLifeTime();

                // 🚨 [제안서 규칙] 5초 만에 완전히 흐려지므로 0초~5초 사이의 비율(0.0 ~ 1.0) 계산
                float turbidityPercent = bugAge / 5.0f;

                // 원래 물 색상에서 탁한 색(짙은 회갈색 혹은 녹조 색)으로 서서히 Lerp 섞기!
                // 녹조 느낌을 내고 싶다면 Color.green이나 어두운 색 조합을 쓰시면 됩니다.
                Color dirtyColor = new Color(0.3f, 0.25f, 0.2f, 0.8f); // 탁한 갈색 예시
                
                if (waterRenderer != null)
                {
                    waterRenderer.material.color = Color.Lerp(originalWaterColor, dirtyColor, turbidityPercent);
                }
            }
        }
        // 3. 물벌레를 클릭해서 잡았거나 아직 안 태어났다면 물을 다시 깨끗하게 유지!
        else
        {
            if (waterRenderer != null)
            {
                // 환수 버튼을 안 눌러도 벌레만 잡으면 다시 정화되는 구조 혹은 환수 전까지 유지 구조 중
                // 벌레가 없을 때는 기본 깨끗한 색으로 부드럽게 복구시킵니다.
                waterRenderer.material.color = Color.Lerp(waterRenderer.material.color, originalWaterColor, Time.deltaTime * 2f);
            }
        }
    }
}