using UnityEngine;

public class WaterFilter : MonoBehaviour
{
    private Renderer waterRenderer;
    private Material waterMaterial; 

    private float origR, origG, origB, origA;

    private float dirtyR = 0.3f;
    private float dirtyG = 0.25f;
    private float dirtyB = 0.2f;
    private float dirtyA = 0.8f;

    private float bugCheckTimer = 0f;
    private GameObject cachedBug = null;

    void Start()
    {
        waterRenderer = GetComponent<Renderer>();
        if (waterRenderer != null)
        {
            waterMaterial = waterRenderer.material;

            Color origColor = waterMaterial.color;
            origR = origColor.r;
            origG = origColor.g;
            origB = origColor.b;
            origA = origColor.a;
        }
    }

    void Update()
    {
        if (waterMaterial == null) return;

        bugCheckTimer += Time.deltaTime;
        if (bugCheckTimer >= 0.2f)
        {
            bugCheckTimer = 0f;
            cachedBug = GameObject.FindWithTag("Bug");
        }

        if (cachedBug != null)
        {
            WaterBug bugScript = cachedBug.GetComponent<WaterBug>();
            if (bugScript != null)
            {
                float bugAge = bugScript.GetLifeTime();
                
                float t = bugAge / 5.0f;
                if (t > 1.0f) t = 1.0f; 

                float currentR = origR + (dirtyR - origR) * t;
                float currentG = origG + (dirtyG - origG) * t;
                float currentB = origB + (dirtyB - origB) * t;
                float currentA = origA + (dirtyA - origA) * t;

                waterMaterial.color = new Color(currentR, currentG, currentB, currentA);
            }
        }
        else
        {
            Color currentInGameColor = waterMaterial.color;

            float speed = Time.deltaTime * 2f;

            float nextR = Mathf.MoveTowards(currentInGameColor.r, origR, speed);
            float nextG = Mathf.MoveTowards(currentInGameColor.g, origG, speed);
            float nextB = Mathf.MoveTowards(currentInGameColor.b, origB, speed);
            float nextA = Mathf.MoveTowards(currentInGameColor.a, origA, speed);

            waterMaterial.color = new Color(nextR, nextG, nextB, nextA);
        }
    }
}