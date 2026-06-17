using UnityEngine;

public class FishFood : MonoBehaviour
{
    void Start()
    {
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
            FishMovement fish = other.GetComponent<FishMovement>();
            
            if (fish != null)
            {
                Vector3 beforeScale = other.transform.localScale;
                
                fish.EatFood(); 

                if (other.transform.localScale == beforeScale)
                {
                    return; 
                }
            }

            FishFeedingManager.score += 1;
            Destroy(gameObject);
        }
    }
}