using UnityEngine;

public class WaterBug : MonoBehaviour
{
    // 물벌레가 살아온 시간을 체크할 타이머
    private float lifeTimer = 0f;

    void Update()
    {
        // 물벌레가 태어난 순간부터 매 프레임 시간이 누적됩니다.
        lifeTimer += Time.deltaTime;

        // 🚨 [수업 내용 응용] 마우스 오른쪽 버튼(1)이 눌렸을 때!
        if (Input.GetMouseButtonDown(1))
        {
            // 🚨 유니티 정석 마우스 위치 체크 (마우스가 물벌레 콜라이더 위에 있는지 검사)
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 마우스 우클릭 레이저가 무언가와 부딪혔고
            if (Physics.Raycast(ray, out hit))
            {
                // 🚨 부딪힌 그 오브젝트가 바로 나(물벌레 자신)라면!
                if (hit.collider.gameObject == this.gameObject)
                {
                    // 물벌레 즉시 박멸! 사라집니다. ✨
                    Destroy(gameObject);
                }
            }
        }
    }

    // 외부(WaterFilter)에서 이 벌레가 얼마나 방치되었는지 확인할 수 있게 주는 함수
    public float GetLifeTime()
    {
        return lifeTimer;
    }
}