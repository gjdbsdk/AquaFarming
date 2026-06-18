using UnityEngine;

public class WaterBug : MonoBehaviour
{
    private float lifeTimer = 0f;
    
    public AudioClip bugCatchSound; 

    void Update()
    {
        lifeTimer += Time.deltaTime;

        if (Input.GetButtonDown("Fire2"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    if (bugCatchSound != null)
                    {
                        AudioSource.PlayClipAtPoint(bugCatchSound, transform.position);
                    }

                    Destroy(gameObject);
                }
            }
        }
    }

    public float GetLifeTime()
    {
        return lifeTimer;
    }
}