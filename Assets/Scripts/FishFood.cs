using UnityEngine;

public class FishFood : MonoBehaviour
{
    void Start()
    {
        // 2초 뒤 자동 삭제 기능
        Destroy(gameObject, 2.0f);
    }

    void Update()
    {
        if (transform.position.y < -5f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fish"))
        {
            // 물고기 스크립트를 먼저 가져옵니다.
            FishMovement fish = other.GetComponent<FishMovement>();
            
            if (fish != null)
            {
                // 🚨 물고기가 5알을 채웠는지 확인하기 위해, 
                // 물고기가 현재 크기 배율(sizeModifier) 상태를 유지하고 있는지 대조하는 방식이나 
                // 배부른 상태를 체크해 밥을 주는 로직입니다. 
                // (위의 EatFood에서 5알 제한을 알아서 걸러주므로 그대로 호출해봅니다!)
                
                // 임시로 원래 크기 배율을 기억해뒀다가 함수 실행 후 커졌는지 대조합니다.
                // 5알 다 차서 안 커졌다면 사료를 파괴하지 않고 그냥 통과시킵니다!
                Vector3 beforeScale = other.transform.localScale;
                
                fish.EatFood(); // 물고기한테 밥 먹으라고 신호 주기

                // 만약 밥을 먹여봤는데도 물고기 크기가 전혀 변하지 않았다면 = 이미 5알 다 먹어서 배부른 상태!
                if (other.transform.localScale == beforeScale)
                {
                    return; // 🚨 사료 파괴 안 하고, 스코어도 안 올리고 그냥 패스!
                }
            }

            // 5알 미만이라 정상적으로 받아먹었을 때만 스코어 업 및 사료 파괴!
            FishFeedingManager.score += 1;
            Destroy(gameObject);
        }
    }
}