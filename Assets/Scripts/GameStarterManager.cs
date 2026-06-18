using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartManager : MonoBehaviour
{
    public GUISkin skin; 

    public AudioClip beepSound;  
    public AudioClip startSound; 

    private bool isCountingDown = false;
    private string countdownText = "";    
    
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isCountingDown)
        {
            if (Input.GetButtonDown("Submit")) 
            {
                StartCoroutine(StartCountdownRoutine());
            }

            if (Input.GetButtonDown("Cancel"))
            {
                SceneManager.LoadScene("AquaFarming-Room");
            }
        }
    }

    private IEnumerator StartCountdownRoutine()
    {
        isCountingDown = true;

        countdownText = "3";
        PlaySound(beepSound); 
        yield return new WaitForSeconds(1.0f);

        countdownText = "2";
        PlaySound(beepSound); 
        yield return new WaitForSeconds(1.0f);

        countdownText = "1";
        PlaySound(beepSound); 
        yield return new WaitForSeconds(1.0f);

        countdownText = "START!";
        PlaySound(startSound); 
        yield return new WaitForSeconds(0.8f);

        SceneManager.LoadScene("AquaFarming-FishTank");
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip; 
            audioSource.Play();      
        }
    }

    private void OnGUI()
    {
        if (skin != null) GUI.skin = skin;

        int sw = Screen.width;
        int sh = Screen.height;

        if (!isCountingDown)
        {
            Rect titleRect = new Rect(sw / 2 - 250, 20, 500, 50);
            GUI.Label(titleRect, "Aqua Farming", "highlight");


            string guideText = 
                "[ HOW PLAY ]\n\n" +
                "1. Left Mouse Click : Drop fish food from the top of the tank.\n" +
                "2. Growth System : Fishes grow in size and Level (Lv.1 to Lv.5) when fed.\n" +
                "3. Danger Alert : Water bugs contaminate the tank! \nRight-click on bugs immediately to defeat them.\n" +
                "4. Clear Condition : Raise ALL fishes to Lv.5 within the 30-second time limit.\n\n" +
                "Are you ready to manage your beautiful fish tank safely?";
            
            Rect bodyRect = new Rect(80, 150, sw - 160, sh - 300);
            GUI.Label(bodyRect, guideText, "title");

            Rect startNoticeRect = new Rect(0, sh - 50, sw, 50);
            GUI.Label(startNoticeRect, "Press [ ENTER ] to Start / [ ESC ] to Return Room", "highlight");
        }
        else
        {
            Rect countRect = new Rect(sw / 2 - 200, sh / 2 - 50, 400, 100);
            GUI.Label(countRect, countdownText, "highlight");
        }
    }
}