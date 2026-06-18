using UnityEngine;
using UnityEngine.SceneManagement;

public class FishTankInteraction : MonoBehaviour
{
    public GUISkin skin; 
    
    private bool isPlayerNearby = false; 
    private AudioSource audioSource; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetButtonDown("Submit"))
        {
            SceneManager.LoadScene("AquaFarming-Title"); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            isPlayerNearby = true; 

            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            isPlayerNearby = false; 

            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }
    }

    private void OnGUI()
    {
        if (isPlayerNearby)
        {
            GUI.skin = skin; 
            
            int sw = Screen.width;
            int sh = Screen.height;

            Rect rect = new Rect(sw / 2 - 300, sh / 2 - 50, 600, 100);
            
            GUI.Label(rect, "Press [ ENTER ] to Feed Fishes", "RoomBigMessage");
        }
    }
}