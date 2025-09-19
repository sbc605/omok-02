using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    // --- 외부 스크립트 연결 ---
    private GameLogic gameLogic;
    private SimpleAI simpleAI;
    private ClickFXController clickEffect;

    // --- AI 턴 관리 ---
    private bool isAITurnProcessing = false;
    [SerializeField] private float aiThinkTime = 0.5f;

    // --- 보드 상태 관찰용 변수 ---
    private GameLogic.StoneType[,] boardCopy;

    // --- 급수 데이터 ---
    [Header("게임 밸런스 데이터")]
    public List<RankData> rankDataList;
    private RankData currentRank;

    // --- BGM 재생 관련 ---
    [Header("BGM 설정")]
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
        base.Awake();
        bgmPlayer = gameObject.AddComponent<AudioSource>();
        bgmPlayer.loop = true;
    }

    void Start()
    {
        clickEffect = FindFirstObjectByType<ClickFXController>();
    }

    private void OnDestroy()
    {
        if (gameLogic != null)
        {
            gameLogic.OnTurnChanged -= HandleTurnChange;
            gameLogic.OnGameEnded -= HandleGameEnd;
        }
    }

    void Update()
    {
        if (gameLogic == null) return;
        if (gameLogic.GetCurrentTurn() == GameLogic.StoneType.Black)
        {
            DetectPlayerMove();
        }
    }

    #endregion

    #region AI & Game Flow

    private void DetectPlayerMove()
    {
        for (int r = 0; r < gameLogic.boardSize; r++)
        {
            for (int c = 0; c < gameLogic.boardSize; c++)
            {
                if (boardCopy[r, c] != gameLogic.GetStone(r, c))
                {
                    if (gameLogic.GetStone(r, c) == GameLogic.StoneType.Black)
                    {
                        simpleAI.lastMove = new Vector2Int(r, c);
                        UpdateBoardCopy();
                        return;
                    }
                }
            }
        }
    }

    private void HandleTurnChange(GameLogic.PlayerType currentPlayer)
    {
        if (currentPlayer == GameLogic.PlayerType.CPU && !isAITurnProcessing)
        {
            isAITurnProcessing = true;
            StartCoroutine(ProcessAITurn());
        }
        else if (currentPlayer == GameLogic.PlayerType.player)
        {
            isAITurnProcessing = false;
        }
    }


    private void HandleGameEnd(GameLogic.GameResult result)
    {
        if (currentRank == null)
        {
            Debug.LogError("현재 급수가 설정되지 않아 보상을 지급할 수 없습니다!");
            return;
        }
        Debug.Log($"게임 결과({result})에 따라 호감도를 조절합니다. 현재 급수: {currentRank.rankName}");
        if (result == GameLogic.GameResult.Win)
        {
            PlayerDataManager.Instance.AddFavorability(currentRank.winBonus);
        }
        else if (result == GameLogic.GameResult.Lose)
        {
            PlayerDataManager.Instance.DecreaseFavorability(currentRank.losePenalty);
        }
    }

    private IEnumerator ProcessAITurn()
    {
        yield return new WaitForSeconds(aiThinkTime);
        if (simpleAI != null && gameLogic != null)
        {
            GameLogic.StoneType[,] currentBoardState = CreateBoardCopy();
            Vector2Int aiMove = simpleAI.GetNextMove(currentBoardState);
            gameLogic.PlaceStone(aiMove.x, aiMove.y);
            UpdateBoardCopy();
        }
        isAITurnProcessing = false;
    }

    #endregion

    #region Board Copy Utilities

    private void InitializeBoardCopy() => boardCopy = CreateBoardCopy();
    private void UpdateBoardCopy() => boardCopy = CreateBoardCopy();
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

    #endregion

    #region Scene & Music Management

    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        // BGM 처리
        AudioClip clipToPlay = null;
        foreach (var sm in sceneMusicList)
        {
            if (sm.sceneName == scene.name) { clipToPlay = sm.musicClip; break; }
        }
        if (clipToPlay != null)
        {
            if (bgmPlayer.clip != clipToPlay)
            {
                bgmPlayer.clip = clipToPlay;
                bgmPlayer.Play();
            }
        }
        else { bgmPlayer.Stop(); }

        // 씬별 로직 처리
        if (scene.name == "Game")
        {
            StartCoroutine(SetupGameScene());
        }
        else if (scene.name == "Main")
        {
            DetermineCurrentRank();
        }
    }

    private void DetermineCurrentRank()
    {
        int currentFavor = PlayerDataManager.Instance.Favorability;
        currentRank = null;

        // 요구 호감도 높은 순으로 정렬된 리스트를 가정하고 순회
        foreach (var rank in rankDataList)
        {
            if (currentFavor >= rank.requiredFavorability)
            {
                currentRank = rank;
                break;
            }
        }

        if (currentRank == null && rankDataList.Count > 0)
        {
            currentRank = rankDataList[rankDataList.Count - 1];
        }

        if (currentRank != null)
        {
            Debug.Log($"현재 플레이어의 급수는 '{currentRank.rankName}' 입니다.");
        }
    }

    private IEnumerator SetupGameScene()
    {
        yield return new WaitForEndOfFrame();

        gameLogic = FindFirstObjectByType<GameLogic>();
        simpleAI = FindFirstObjectByType<SimpleAI>();

        var board = GameObject.Find("Board")?.GetComponent<SpriteRenderer>();
        var resultPanel = GameObject.Find("Result Panel")?.transform;
        if (clickEffect != null && board != null && resultPanel != null)
        {
            // clickEffect.SetGameSceneSprite(board, resultPanel);
        }

        if (gameLogic != null && simpleAI != null)
        {
            // 이벤트 구독
            gameLogic.OnTurnChanged -= HandleTurnChange;
            gameLogic.OnTurnChanged += HandleTurnChange;
            gameLogic.OnGameEnded -= HandleGameEnd;
            gameLogic.OnGameEnded += HandleGameEnd;

            // AI 난이도 설정
            if (currentRank != null)
            {
                simpleAI.difficulty = currentRank.aiDifficulty;
                Debug.Log($"AI 난이도를 '{currentRank.aiDifficulty}'로 설정했습니다.");
            }

            // 보드 복사본 초기화
            InitializeBoardCopy();

            // 씬 시작 직후 AI 차례라면 바로 실행
            if (gameLogic.currentPlayer == GameLogic.PlayerType.CPU && !isAITurnProcessing)
            {
                isAITurnProcessing = true;
                StartCoroutine(ProcessAITurn());
            }
        }
        else
        {
            Debug.LogError("Game 씬에서 GameLogic 또는 SimpleAI 오브젝트를 찾을 수 없습니다!");
        }
    }

    public void LoadMainScene() => SceneManager.LoadScene("Main");

    public void LoadGameScene()
    {
        if (currentRank == null) { DetermineCurrentRank(); }

        if (currentRank == null)
        {
            Debug.LogError("현재 급수가 설정되지 않아 입장료를 계산할 수 없습니다!");
            return;
        }

        // ▼▼▼ DecreaseFavorability의 두 번째 인자로 false를 넘겨줍니다. ▼▼▼
        if (PlayerDataManager.Instance.DecreaseFavorability(currentRank.entryFee, false))
        {
            // 'false'를 넘겨주면, 호감도가 메모리에서는 차감되지만 파일에는 아직 저장되지 않습니다.
            SceneManager.LoadScene("Game");
        }
        else
        {
            Debug.Log("호감도가 부족하여 게임을 시작할 수 없습니다.");
        }
    }
    public void LoadSettingScene() => SceneManager.LoadScene("Setting");

    public void RestartGame()
    {
        Debug.Log("게임을 재시작합니다.");
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    #endregion
}