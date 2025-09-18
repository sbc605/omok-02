using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; // List<>를 사용하기 위해 추가

public class GameManager : Singleton<GameManager>
{
    // --- 외부 스크립트 연결 ---
    private GameLogic gameLogic;
    private SimpleAI simpleAI;
    // BlockController는 GameManager가 직접 입력을 처리하지 않게 변경
    // public BlockController blockController; 

    // --- AI 턴 관리 ---
    private bool isAITurnProcessing = false;

    [SerializeField] private float aiThinkTime = 0.5f; // AI생각 시간 변수 

    // --- 보드 상태 관찰용 변수 ---
    private GameLogic.StoneType[,] boardCopy; // GameLogic의 보드를 복사해서 들고 있을 변수

    // --- BGM 재생 관련 ---
    private AudioSource bgmPlayer;
    [System.Serializable]
    public class SceneMusic
    {
        public string sceneName;
        public AudioClip musicClip;
    }
    public List<SceneMusic> sceneMusicList;

    #region Unity Lifecycle Methods
    
    protected override void Awake()
    {
        base.Awake(); // 부모 클래스의 Awake() 실행 (싱글톤 설정)
        bgmPlayer = gameObject.AddComponent<AudioSource>();
        bgmPlayer.loop = true;
    }

    private void Start()
    {
        // gameLogic이 연결되었는지 확인
        if (gameLogic != null)
        {
            // 1. 턴 변경 이벤트를 구독하여 AI 턴 감지
            gameLogic.OnTurnChanged += HandleTurnChange;

            // 2. 현재 보드 상태를 복사하여 초기화 (최초 감시 시작)
            InitializeBoardCopy();
        }
    }

    private void OnDestroy()
    {
        // 씬 전환 또는 오브젝트 파괴 시 이벤트 구독 해제
        if (gameLogic != null)
        {
            gameLogic.OnTurnChanged -= HandleTurnChange;
        }
    }

    void Update()
    {
        // 게임 로직이 없으면 아무것도 안 함
        if (gameLogic == null) return;
        
        // 플레이어 턴일 때만 보드 변화를 감시
        if (gameLogic.GetCurrentTurn() == GameLogic.StoneType.Black)
        {
            DetectPlayerMove();
        }
    }

    #endregion

    #region AI & Game Flow

    // 플레이어의 수를 감지하는 '관찰자' 함수
    private void DetectPlayerMove()
    {
        // 전체 보드를 순회
        for (int r = 0; r < gameLogic.boardSize; r++)
        {
            for (int c = 0; c < gameLogic.boardSize; c++)
            {
                // 내 복사본과 실제 게임 보드의 상태가 다른 곳을 발견하면
                if (boardCopy[r, c] != gameLogic.GetStone(r, c))
                {
                    // 그곳이 바로 플레이어가 새로 돌을 둔 위치
                    GameLogic.StoneType newStone = gameLogic.GetStone(r, c);

                    // 새로 놓인 돌이 플레이어의 돌(흑돌)이 맞는지 확인
                    if (newStone == GameLogic.StoneType.Black)
                    {
                        Debug.Log($"플레이어의 수를 감지했습니다: ({r}, {c})");
                        
                        // AI의 lastMove를 업데이트
                        simpleAI.lastMove = new Vector2Int(r, c);

                        // 내 복사본도 최신 상태로 업데이트하여 중복 감지를 방지
                        UpdateBoardCopy();
                        return; // 한 턴에 하나의 수만 감지하면 되므로 함수 종료
                    }
                }
            }
        }
    }

    // 턴이 변경될 때 호출될 함수 (이벤트 핸들러)
    private void HandleTurnChange(GameLogic.PlayerType currentPlayer)
    {
        // 넘어온 턴이 CPU(AI)의 턴인지 확인
        if (currentPlayer == GameLogic.PlayerType.CPU && !isAITurnProcessing)
        {
            isAITurnProcessing = true;
            StartCoroutine(ProcessAITurn());
        }
        else if(currentPlayer == GameLogic.PlayerType.player)
        {
            isAITurnProcessing = false;
        }
    }

    // AI의 턴을 처리하는 코루틴
    private IEnumerator ProcessAITurn()
    {
        yield return new WaitForSeconds(aiThinkTime); // AI 생각하는 시간

        if (simpleAI != null && gameLogic != null)
        {
            // 현재 보드 상태의 최신 복사본을 만들어서 AI에게 전달
            GameLogic.StoneType[,] currentBoardState = CreateBoardCopy();
            Vector2Int aiMove = simpleAI.GetNextMove(currentBoardState);

            // AI가 계산한 위치에 착수
            gameLogic.PlaceStone(aiMove.x, aiMove.y);
            
            // AI가 둔 수까지 내 복사본에 업데이트
            UpdateBoardCopy();
        }
    }

    #endregion

    #region Board Copy Utilities

    // 게임 시작 시 보드 복사본을 초기화하는 함수
    private void InitializeBoardCopy()
    {
        boardCopy = CreateBoardCopy();
    }

    // 현재 GameLogic의 보드 상태를 그대로 복제하여 반환하는 함수
    private GameLogic.StoneType[,] CreateBoardCopy()
    {
        int boardSize = gameLogic.boardSize;
        GameLogic.StoneType[,] newBoard = new GameLogic.StoneType[boardSize, boardSize];
        for (int r = 0; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                newBoard[r, c] = gameLogic.GetStone(r, c);
            }
        }
        return newBoard;
    }

    // 내 복사본을 현재 GameLogic의 상태로 갱신하는 함수
    private void UpdateBoardCopy()
    {
        boardCopy = CreateBoardCopy();
    }

    #endregion

    #region Scene & Music Management

    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        AudioClip clipToPlay = null;
        foreach (var sm in sceneMusicList)
        {
            if (sm.sceneName == scene.name)
            {
                clipToPlay = sm.musicClip;
                break;
            }
        }

        if (clipToPlay != null)
        {
            if (bgmPlayer.clip != clipToPlay)
            {
                bgmPlayer.clip = clipToPlay;
                bgmPlayer.Play();
            }
        }
        else
        {
            bgmPlayer.Stop();
        }

        // 씬이 Game으로 전환시 컴포넌트 연결 

        if (scene.name == "Game")
        {
            StartCoroutine(SetupGameScene());
        }
    }

    // 코루틴 셋업 게임 씬

    private IEnumerator SetupGameScene()
{
    // 한 프레임 대기 찾지 못하는거 방지용 
    yield return new WaitForEndOfFrame();


    gameLogic = FindFirstObjectByType<GameLogic>();
    simpleAI = FindFirstObjectByType<SimpleAI>();
    
    //정상적으로 찾았는지 확인용 추후 주석
    Debug.Log(gameLogic != null ? "게임로직 오브젝트 찾음" : "실패함");

    // 못 찾았을 경우, 로그 출력
        if (gameLogic == null) Debug.LogError("Game 씬에서 GameLogic 오브젝트를 찾을 수 없습니다!");
    if (simpleAI == null) Debug.LogError("Game 씬에서 SimpleAI 오브젝트를 찾을 수 없습니다!");
    
    // 보드 복사본도 여기서 초기화해줍니다.
    InitializeBoardCopy();
}

    public void LoadMainScene() => SceneManager.LoadScene("Main");
    public void LoadGameScene() => SceneManager.LoadScene("Game");
    public void LoadSettingScene() => SceneManager.LoadScene("Setting");

    public void RestartGame()
    {
        Debug.Log("게임을 재시작합니다.");
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    #endregion
}