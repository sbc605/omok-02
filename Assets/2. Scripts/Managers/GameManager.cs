using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    // --- 외부 스크립트 연결 ---
    private GameLogic gameLogic;
    private SimpleAI simpleAI;
    private ResultPanelController resultPanelController;
    private Timer timer;
    private TurnAndVariantUI turnAndVariantUI;

    // --- AI 턴 관리 ---
    private bool isAITurnProcessing = false;
    [SerializeField] private float aiThinkTime = 0.5f;
    private bool isGameOver = false;

    // --- 급수 데이터 ---
    [Header("게임 밸런스 데이터")]
    public List<RankData> rankDataList;
    private RankData currentRank;

    // --- BGM 재생 관련 (SoundManager로 이전 예정) ---
    [System.Serializable]
    public class SceneMusic { public string sceneName; public AudioClip musicClip; }
    public List<SceneMusic> sceneMusicList;

    public bool IsGameOver() => isGameOver;

    #region Unity Lifecycle & Scene Management

    // ▼▼▼ 주석을 해제하여 싱글톤과 BGM 플레이어 초기화 로직을 복구합니다 ▼▼▼
    protected override void Awake()
    {
        base.Awake();
    }

    private void OnDestroy()
    {
        if (gameLogic != null)
        {
            gameLogic.OnTurnChanged -= HandleTurnChange;
            gameLogic.OnGameEnded -= HandleGameEnd;
        }
    }

    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
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

    private IEnumerator SetupGameScene()
    {
        isGameOver = false;
        yield return new WaitForEndOfFrame();

        gameLogic = FindFirstObjectByType<GameLogic>();
        simpleAI = FindFirstObjectByType<SimpleAI>();
        resultPanelController = FindFirstObjectByType<ResultPanelController>(FindObjectsInactive.Include);
        timer = FindFirstObjectByType<Timer>();
        turnAndVariantUI = FindFirstObjectByType<TurnAndVariantUI>();

        if (gameLogic != null && simpleAI != null)
        {
            gameLogic.OnTurnChanged -= HandleTurnChange;
            gameLogic.OnTurnChanged += HandleTurnChange;
            gameLogic.OnGameEnded -= HandleGameEnd;
            gameLogic.OnGameEnded += HandleGameEnd;

            // ▼▼▼ Game 씬에서도 currentRank가 비어있으면 다시 한번 설정 (안전장치) ▼▼▼
            if (currentRank == null)
            {
                DetermineCurrentRank();
            }

            if (currentRank != null)
            {
                simpleAI.difficulty = currentRank.aiDifficulty;
            }

            if (gameLogic.currentPlayer == GameLogic.PlayerType.CPU && !isAITurnProcessing)
            {
                isAITurnProcessing = true;
                StartCoroutine(ProcessAITurn());
            }
        }
    }

    #endregion

    #region Game Flow Handlers
    
    public void OnPlayerMoveConfirmed(Vector2Int move)
    {
        simpleAI.lastMove = move;
    }

    private void HandleTurnChange(GameLogic.PlayerType currentPlayer)
    {
        if (isGameOver) return;
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
        isGameOver = true;
        if (timer != null) timer.StopTimer();

        if (turnAndVariantUI != null)
        {
            turnAndVariantUI.HideUI();
        }

        if (currentRank == null) return;

        if (result == GameLogic.GameResult.Win)
            PlayerDataManager.Instance.AddFavorability(currentRank.winBonus);
        else if (result == GameLogic.GameResult.Lose)
            PlayerDataManager.Instance.DecreaseFavorability(currentRank.losePenalty);

        if (resultPanelController != null)
            resultPanelController.ShowResult(result);
    }

    private IEnumerator ProcessAITurn()
    {
        yield return new WaitForSeconds(aiThinkTime);
        if (simpleAI != null && gameLogic != null)
        {
            GameLogic.StoneType[,] currentBoardState = CreateBoardCopy();
            Vector2Int aiMove = simpleAI.GetNextMove(currentBoardState);
            gameLogic.PlaceStone(aiMove.x, aiMove.y);
        }
        isAITurnProcessing = false;
    }

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

    #region Public Methods for UI
    public void LoadMainScene() => SceneManager.LoadScene("Main");
    public void LoadGameScene()
    {
        if (currentRank == null) { DetermineCurrentRank(); }
        if (currentRank == null) { Debug.LogError("급수 데이터 확인 필요"); return; }
        if (PlayerDataManager.Instance.DecreaseFavorability(currentRank.entryFee, false))
        {
            SceneManager.LoadScene("Game");
        }
        else { Debug.Log("호감도 부족"); }
    }
    public void LoadSettingScene() => SceneManager.LoadScene("Setting");
    public void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    private void DetermineCurrentRank()
    {
        int currentFavor = PlayerDataManager.Instance.Favorability;
        currentRank = null;
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
    }
    public RankData GetCurrentRank() => currentRank;

    #endregion
}