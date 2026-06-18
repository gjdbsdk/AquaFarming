using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneSceneChanger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            SceneManager.LoadScene("AquaFarming-Room");
        }
    }
}