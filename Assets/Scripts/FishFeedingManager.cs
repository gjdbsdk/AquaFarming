using UnityEngine;
using UnityEngine.SceneManagement;

public class FishFeedingManager : MonoBehaviour
{
    public GameObject foodPrefab; 
    public GUISkin skin;          
    
    public float tankCenterX = 1.1f; 
    public float tankCenterZ = 0.2f;   
    public float spawnYHeight = 1.5f;   
    public float tankWidth = 0.7f;      
    public float tankDepthOffset = 0.1f; 

    public static int score = 0;   

    public GameObject[] fishPrefabs; 

    private float allFullTimer = 0f; 
    private bool showAddButton = false; 

    public GameObject bugPrefab; 
    private float bugSpawnTimer = 0f;
    private float nextSpawnTime = 0f; 

    private float gameTimer = 0f;          
    private float maxGameTime = 30.0f;     
    private bool isGameOver = false;       
    private bool isGameWin = false;        
    private int finalFishCount = 0;        

    private int[] fishCountByStage = new int[5];

    void Start()
    {
        score = 0; 
        gameTimer = 0f;
        isGameOver = false; 
        isGameWin = false;
        finalFishCount = 0;
        nextSpawnTime = Random.Range(3.0f, 10.0f);
    }

    void Update()
    {
        if (isGameOver || isGameWin)
        {
            if (Input.GetButtonDown("Jump"))
            {
                SceneManager.LoadScene("AquaFarming-Room"); 
            }
            return; 
        }

        gameTimer += Time.deltaTime;

        GameObject[] fishes = GameObject.FindGameObjectsWithTag("Fish");

        if (fishes.Length == 0)
        {
            isGameOver = true;
            return;
        }

        for (int i = 0; i < fishCountByStage.Length; i++)
        {
            fishCountByStage[i] = 0;
        }

        bool checkAllFishesMax = true;
        
        foreach (GameObject fishObj in fishes)
        {
            FishMovement fishScript = fishObj.GetComponent<FishMovement>();
            if (fishScript != null)
            {
                System.Reflection.FieldInfo field = typeof(FishMovement).GetField("foodEatenCount", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                int eatenCount = 0;
                if (field != null)
                {
                    eatenCount = (int)field.GetValue(fishScript);
                }

                int stageIndex = eatenCount; 
                if (stageIndex > 4) stageIndex = 4; 
                
                fishCountByStage[stageIndex]++;

                if (stageIndex < 4) 
                {
                    checkAllFishesMax = false;
                }
            }
        }

        if (gameTimer >= maxGameTime)
        {
            gameTimer = maxGameTime; 

            if (checkAllFishesMax && fishes.Length > 0)
            {
                isGameWin = true;
                finalFishCount = fishes.Length; 
            }
            else
            {
                isGameOver = true;
            }
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (foodPrefab != null)
            {
                float mousePercentX = (Input.mousePosition.x / Screen.width) - 0.5f;
                float calculatedZ = tankCenterZ - (mousePercentX * tankWidth);
                float calculatedX = tankCenterX - (mousePercentX * tankDepthOffset);
                Vector3 spawnPosition = new Vector3(calculatedX, spawnYHeight, calculatedZ);
                Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
            }
        }

        if (GameObject.FindWithTag("Bug") == null)
        {
            bugSpawnTimer += Time.deltaTime;
            if (bugSpawnTimer >= nextSpawnTime)
            {
                if (bugPrefab != null)
                {
                    Vector3 bugSpawnPos = new Vector3(Random.Range(0.93f, 1.0f), 0.68f, Random.Range(-0.1f, 0.5f));
                    Instantiate(bugPrefab, bugSpawnPos, Quaternion.identity);
                }
                bugSpawnTimer = 0f;
                nextSpawnTime = Random.Range(3.0f, 10.0f);
            }
        }
        else
        {
            bugSpawnTimer = 0f;
        }

        bool areAllFishesFull = true;
        foreach (GameObject fishObj in fishes)
        {
            FishMovement fishScript = fishObj.GetComponent<FishMovement>();
            if (fishScript != null && fishScript.isFull == false)
            {
                areAllFishesFull = false;
                break;
            }
        }

        if (areAllFishesFull)
        {
            allFullTimer += Time.deltaTime; 
            if (allFullTimer >= 5.0f) showAddButton = true; 
        }
        else
        {
            allFullTimer = 0f;
            showAddButton = false;
        }

        if (showAddButton && Input.GetButtonDown("Submit"))
        {
            if (fishPrefabs != null && fishPrefabs.Length > 0)
            {
                int randomIndex = Random.Range(0, fishPrefabs.Length);
                Vector3 spawnPos = new Vector3(tankCenterX, 0.7f, Random.Range(-0.1f, 0.5f));
                Instantiate(fishPrefabs[randomIndex], spawnPos, Quaternion.identity);
                
                allFullTimer = 0f;
                showAddButton = false;
            }
        }
    }

    private void OnGUI()
    {
        if (skin != null) GUI.skin = skin;

        int sw = Screen.width;
        int sh = Screen.height;

        string stageStatusText = string.Format(
            "Lv.1 Fish: {0}\nLv.2 Fish: {1}\nLv.3 Fish: {2}\nLv.4 Fish: {3}\nLv.5 Fish: {4}",
            fishCountByStage[0], fishCountByStage[1], fishCountByStage[2], fishCountByStage[3], fishCountByStage[4]
        );
        
        Rect statusRect = new Rect(0, 0, 100, 90);
        GUI.Label(statusRect, stageStatusText, "nowstate");

        float remainingTime = Mathf.Max(0f, maxGameTime - gameTimer);
        string timerText = "Time Left: " + remainingTime.ToString("F1") + "s";
        
        Rect timerRect = new Rect(sw - 200, sh - 60, 160, 40);
        GUI.Label(timerRect, timerText, "Message");

        if (isGameWin)
        {
            GUI.Box(new Rect(0, 0, sw, sh), "");
            Rect winRect = new Rect(sw / 2 - 250, sh / 2 - 80, 500, 60);
            GUI.Label(winRect, "WIN!", "Message");

            Rect resultRect = new Rect(sw / 2 - 250, sh / 2 - 10, 500, 50);
            GUI.Label(resultRect, "Total Fishes Raised: " + finalFishCount, "Message");

            Rect restartRect = new Rect(sw / 2 - 250, sh / 2 + 50, 500, 50);
            GUI.Label(restartRect, "Press [ SPACEBAR ] to Return Home", "Message");
            return;
        }

        if (isGameOver)
        {
            GUI.Box(new Rect(0, 0, sw, sh), "");
            Rect gameOverRect = new Rect(sw / 2 - 250, sh / 2 - 60, 500, 60);
            GUI.Label(gameOverRect, "GAME OVER", "Message"); 

            Rect restartRect = new Rect(sw / 2 - 250, sh / 2 + 10, 500, 50);
            GUI.Label(restartRect, "Press [ SPACEBAR ] to Return Home", "Message");
            return; 
        }

        if (showAddButton)
        {
            Rect noticeRect = new Rect(sw / 2 - 300, sh / 2 - 50, 600, 100);
            GUI.Label(noticeRect, "Press [ ENTER ] to Add New Fish!", "RoomBigMessage");
        }
    }
}