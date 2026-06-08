using UnityEngine;
using UnityEngine.SceneManagement;

public class FishTankInteraction : MonoBehaviour
{
    public GUISkin skin; 
    
    private bool isPlayerNearby = false; 

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("AquaFarming-FishTank"); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            isPlayerNearby = true; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            isPlayerNearby = false; 
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
            
            GUI.Label(rect, "Press Enter to Feed Fishes", "Message");
        }
    }
}