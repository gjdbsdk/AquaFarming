using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PBRController : MonoBehaviour
{
    public float walkSpeed = 3.0f;
    public float gravity = 20.0f; 
    private Vector3 velocity;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Idle", true);
        animator.SetBool("Grounded", true); // 점프가 없으니 항상 땅 상태로 초기화
    }

    void Update()
    {
        animator.speed = 2.0f;
        CharacterController controller = GetComponent<CharacterController>();
        
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 1. 키 입력에 따른 평면(X, Z) 이동 값 설정
        velocity.x = moveX * walkSpeed;
        velocity.z = moveZ * walkSpeed;

        // 2. 바닥 체크 및 애니메이션 제어
        if (controller.isGrounded)
        {
            animator.SetBool("Grounded", true);
            
            // 바닥에 있을 때는 중력이 수천씩 무한 누적되지 않도록 고정
            if (velocity.y < 0) 
            {
                velocity.y = -1f; 
            }

            // 키 입력이 있어서 움직이는 상태일 때
            if (new Vector3(velocity.x, 0, velocity.z).magnitude > 0.1f) 
            { 
                animator.SetBool("Idle", false);
                
                // 이동하는 방향을 바라보도록 캐릭터 회전
                transform.LookAt(transform.position + new Vector3(velocity.x, 0, velocity.z)); 
            } 
            // 키 입력이 없을 때 (정지)
            else 
            {
                animator.SetBool("Idle", true);
            }
        }
        else
        {
            // 혹시 공중에 살짝 뜨더라도 땅이 아니라고 체크
            animator.SetBool("Grounded", false);
        }

        // 3. 중력 적용 (매 프레임 Y축 하강 힘 누적)
        velocity.y -= gravity * Time.deltaTime; 
        
        // 4. 애니메이션의 Speed 파라미터 갱신 (트랜지션용)
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        animator.SetFloat("Speed", horizontalVelocity.magnitude);
        
        // 5. 최종 이동 실행
        controller.Move(velocity * Time.deltaTime);
    }
}