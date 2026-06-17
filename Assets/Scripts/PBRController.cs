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
        animator.SetBool("Grounded", true); 
    }

    void Update()
    {
        animator.speed = 2.0f;
        CharacterController controller = GetComponent<CharacterController>();
        
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        velocity.x = moveX * walkSpeed;
        velocity.z = moveZ * walkSpeed;

        if (controller.isGrounded)
        {
            animator.SetBool("Grounded", true);
            
            if (velocity.y < 0) 
            {
                velocity.y = -1f; 
            }

            if (new Vector3(velocity.x, 0, velocity.z).magnitude > 0.1f) 
            { 
                animator.SetBool("Idle", false);
                
                transform.LookAt(transform.position + new Vector3(velocity.x, 0, velocity.z)); 
            } 
            else 
            {
                animator.SetBool("Idle", true);
            }
        }
        else
        {
            animator.SetBool("Grounded", false);
        }

        velocity.y -= gravity * Time.deltaTime; 
        
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        animator.SetFloat("Speed", horizontalVelocity.magnitude);
        
        controller.Move(velocity * Time.deltaTime);
    }
}