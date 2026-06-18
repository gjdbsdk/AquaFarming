using UnityEngine;

public class FishOnlyMovement : MonoBehaviour
{
    private float directionTimer = 0f;
    public float changeDirectionTime = 3.0f; 

    private float currentSpeedY = 0f;
    private float currentSpeedZ = 0f;

    private Vector3 originalScale;

    public float minZ = -0.068f;
    public float maxZ = 0.485f;
    public float minY = 0.684f;
    public float maxY = 0.803f;

    void Start()
    {
        originalScale = transform.localScale;
        
        changeDirectionTime = Random.Range(2.5f, 4.5f);
        ChangeRandomDirection();
        directionTimer = 0f;
    }

    void Update()
    {
        Vector3 movement = new Vector3(0f, currentSpeedY * Time.deltaTime, currentSpeedZ * Time.deltaTime);
        transform.Translate(movement);

        KeepInsideTank();

        directionTimer += Time.deltaTime;
        if (directionTimer >= changeDirectionTime)
        {
            changeDirectionTime = Random.Range(2.5f, 4.5f);
            ChangeRandomDirection();
            directionTimer = 0f; 
        }
    }

    void KeepInsideTank()
    {
        Vector3 currentPos = transform.position;
        bool isHitWall = false;

        if (currentPos.z < minZ)
        {
            currentPos.z = minZ;
            currentSpeedZ = Mathf.Abs(currentSpeedZ) > 0.001f ? Mathf.Abs(currentSpeedZ) : 0.1f;
            isHitWall = true;
        }
        else if (currentPos.z > maxZ)
        {
            currentPos.z = maxZ;
            currentSpeedZ = Mathf.Abs(currentSpeedZ) > 0.001f ? -Mathf.Abs(currentSpeedZ) : -0.1f;
            isHitWall = true;
        }

        if (currentPos.y < minY)
        {
            currentPos.y = minY;
            currentSpeedY = Mathf.Abs(currentSpeedY) > 0.001f ? Mathf.Abs(currentSpeedY) : 0.04f;
            isHitWall = true;
        }
        else if (currentPos.y > maxY)
        {
            currentPos.y = maxY;
            currentSpeedY = Mathf.Abs(currentSpeedY) > 0.001f ? -Mathf.Abs(currentSpeedY) : -0.04f;
            isHitWall = true;
        }

        transform.position = currentPos;

        if (isHitWall)
        {
            ApplyScale();
        }
    }

    void ChangeRandomDirection()
    {
        float rawSpeedZ = Random.Range(0.05f, 0.15f);
        currentSpeedZ = Random.Range(0, 2) == 0 ? rawSpeedZ : -rawSpeedZ;

        float rawSpeedY = Random.Range(0.02f, 0.06f);
        currentSpeedY = Random.Range(0, 2) == 0 ? rawSpeedY : -rawSpeedY;

        ApplyScale();
    }

    void ApplyScale()
    {
        float directionSign = -1.0f; 

        if (Mathf.Abs(currentSpeedZ) > 0.001f)
        {
            directionSign = (currentSpeedZ < 0) ? 1.0f : -1.0f;
        }
        else
        {
            directionSign = (transform.localScale.z < 0f) ? -1.0f : 1.0f;
        }

        float finalX = originalScale.x;
        float finalY = originalScale.y;
        float finalZ = originalScale.z * directionSign;

        transform.localScale = new Vector3(finalX, finalY, finalZ);
    }
}