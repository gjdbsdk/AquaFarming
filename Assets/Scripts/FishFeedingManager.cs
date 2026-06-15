using UnityEngine;
// [씬 전환 필수] 홈 화면 이동을 위해 유니티 씬 관리자를 불러옵니다.
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

    [Header("--- 물고기 종류별 프리팹 배열 ---")]
    public GameObject[] fishPrefabs; 

    private float allFullTimer = 0f; 
    private bool showAddButton = false; 

    [Header("--- 🚨 [제안서] 물벌레 스폰 시스템 ---")]
    public GameObject bugPrefab; 
    private float bugSpawnTimer = 0f;
    private float nextSpawnTime = 0f; 

    [Header("--- 🚨 타이머 및 게임 상태 변수 ---")]
    private float gameTimer = 0f;          
    private float maxGameTime = 10.0f;     
    private bool isGameOver = false;       
    private bool isGameWin = false;        
    private int finalFishCount = 0;        

    // 단계별 물고기 수를 실시간으로 저장할 배열 (인덱스 0~4 사용 -> 1단계~5단계)
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("AquaFarming-Room"); 
            }
            return; 
        }

        gameTimer += Time.deltaTime;

        // 실시간 물고기 탐색 및 단계별 카운팅 세팅
        GameObject[] fishes = GameObject.FindGameObjectsWithTag("Fish");

        if (fishes.Length == 0)
        {
            isGameOver = true;
            return;
        }

        // 매 프레임마다 단계별 물고기 수를 0으로 초기화하고 새로 셉니다!
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
                // 리플렉션 기법을 이용해 private 변수인 foodEatenCount 값을 안전하게 가져옵니다.
                System.Reflection.FieldInfo field = typeof(FishMovement).GetField("foodEatenCount", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                int eatenCount = 0;
                if (field != null)
                {
                    eatenCount = (int)field.GetValue(fishScript);
                }

                // 밥 먹은 수(0개~5개)를 기준으로 단계를 맵핑합니다.
                int stageIndex = eatenCount; 
                if (stageIndex > 4) stageIndex = 4; // 최대 5단계(인덱스 4)로 제한
                
                // 해당 단계의 물고기 카운트를 1 올립니다.
                fishCountByStage[stageIndex]++;

                // 모두 5단계(만랩) 상태인지 체크하는 기존 승리 규칙 유지
                if (fishScript.isFull == false) 
                {
                    checkAllFishesMax = false;
                }
            }
        }

        // 30초 타임아웃 판정
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

        // --- 기존 플레이 로직 (사료 스폰 & 물벌레 타이머) ---
        if (Input.GetMouseButtonDown(0))
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

        // 배부름 보상 버튼 타이머 감시
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
    }

    private void OnGUI()
    {
        if (skin != null) GUI.skin = skin;

        int sw = Screen.width;
        int sh = Screen.height;

        // =======================================================================
        // 🚨 [변경 사항] 단계별 물고기 수: 왼쪽 위 들여쓰기 + "각 단계별 엔터(개행) 처리"
        // =======================================================================
        // \n 문자를 넣어서 계단식 리스트 형태로 깔끔하게 떨어지도록 구성했습니다.
        string stageStatusText = string.Format(
            "1단계 물고기: {0}마리\n2단계 물고기: {1}마리\n3단계 물고기: {2}마리\n4단계 물고기: {3}마리\n5단계 물고기: {4}마리",
            fishCountByStage[0], fishCountByStage[1], fishCountByStage[2], fishCountByStage[3], fishCountByStage[4]
        );
        
        // 여백 들여쓰기(X: 40, Y: 40)를 주고 세로 크기(Height)를 넉넉하게 150으로 늘려 글자가 잘리지 않게 합니다.
        Rect statusRect = new Rect(0, 0, 210, 110);
        GUI.Label(statusRect, stageStatusText, "nowstate");


        // =======================================================================
        // 🚨 [변경 사항] 남은 시간 타이머: "오른쪽 아래(우하단)" 구석 배치
        // =======================================================================
        float remainingTime = Mathf.Max(0f, maxGameTime - gameTimer);
        string timerText = "Time Left: " + remainingTime.ToString("F1") + "s";
        
        // 오른쪽 끝에서 180픽셀, 아래쪽 끝에서 60픽셀 여백 공간에 딱 달라붙게 배치합니다.
        Rect timerRect = new Rect(sw - 180, sh - 60, 160, 40);
        GUI.Label(timerRect, timerText, "Message");


        // =======================================================================
        // 🚨 엔딩 및 보상 화면 렌더링 (기존 상태 유지)
        // =======================================================================
        
        // GAME WIN UI
        if (isGameWin)
        {
            GUI.Box(new Rect(0, 0, sw, sh), "");
            Rect winRect = new Rect(sw / 2 - 250, sh / 2 - 80, 500, 60);
            GUI.Label(winRect, "🎉 WIN! 🎉", "Message");

            Rect resultRect = new Rect(sw / 2 - 250, sh / 2 - 10, 500, 50);
            GUI.Label(resultRect, "Total Fishes Raised: " + finalFishCount, "Message");

            Rect restartRect = new Rect(sw / 2 - 250, sh / 2 + 50, 500, 50);
            GUI.Label(restartRect, "Press [ SPACEBAR ] to Return Home", "Message");
            return;
        }

        // GAME OVER UI
        if (isGameOver)
        {
            GUI.Box(new Rect(0, 0, sw, sh), "");
            Rect gameOverRect = new Rect(sw / 2 - 250, sh / 2 - 60, 500, 60);
            GUI.Label(gameOverRect, "GAME OVER", "Message"); 

            Rect restartRect = new Rect(sw / 2 - 250, sh / 2 + 10, 500, 50);
            GUI.Label(restartRect, "Press [ SPACEBAR ] to Return Home", "Message");
            return; 
        }

        // 물고기 추가 보상 버튼 (화면 하단 중앙 정렬 유지)
        if (showAddButton)
        {
            Rect buttonRect = new Rect(sw / 2 - 100, sh - 80, 200, 50);
            if (GUI.Button(buttonRect, "Add New Fish (배부름 보상)"))
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
    }
}