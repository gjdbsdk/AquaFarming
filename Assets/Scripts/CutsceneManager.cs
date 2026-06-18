using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscenemanager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            SceneManager.LoadScene("AquaFarming-Room");
        }
    }
}