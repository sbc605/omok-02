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

    // --- BGM 재생 관련 ---
    [Header("BGM 설정")]
    private AudioSource bgmPlayer;
    [System.Serializable]
    public class SceneMusic { public string sceneName; public AudioClip musicClip; }
    public List<SceneMusic> sceneMusicList;

    public bool IsGameOver() => isGameOver;

    #region Unity Lifecycle & Scene Management

    protected override void Awake()
    {
        base.Awake();
        bgmPlayer = gameObject.AddComponent<AudioSource>();
        bgmPlayer.loop = true;
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
        // BGM 처리
        AudioClip clipToPlay = null;
        foreach (var sm in sceneMusicList)
        {
            if (sm.sceneName == scene.name) { clipToPlay = sm.musicClip; break; }
        }
        if (clipToPlay != null)
        {
            if (bgmPlayer.clip != clipToPlay) { bgmPlayer.clip = clipToPlay; bgmPlayer.Play(); }
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

    // ▼▼▼ OmokController가 호출할 함수 (새로 추가 또는 수정) ▼▼▼
    public void OnPlayerMoveConfirmed(Vector2Int move)
    {
        // OmokController로부터 플레이어가 둔 수를 보고받아 AI의 lastMove를 업데이트
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

        if (currentRank == null) return;

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
            GameLogic.StoneType[,] currentBoardState = CreateBoardCopy(); // 보드 복사본 생성
            Vector2Int aiMove = simpleAI.GetNextMove(currentBoardState);
            gameLogic.PlaceStone(aiMove.x, aiMove.y);
            // AI가 둔 수까지는 관찰할 필요 없으므로 보드 복사본 업데이트 로직 삭제
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
        // 1. 함수가 호출되었는지 확인
        Debug.Log("--- LoadGameScene 함수 시작 ---");

        if (currentRank == null)
        {
            Debug.Log("현재 급수(currentRank)가 null이므로, 급수 재계산을 시도합니다.");
            DetermineCurrentRank();
        }

        // 2. 급수 결정이 잘 되었는지 확인
        if (currentRank == null)
        {
            Debug.LogError("급수 데이터가 없거나, 플레이어의 호감도에 맞는 급수를 찾지 못했습니다. Inspector의 Rank Data List를 확인해주세요.");
            return;
        }

        // 3. 현재 상태를 모두 출력해서 확인
        Debug.Log($"현재 급수: {currentRank.rankName}");
        Debug.Log($"필요한 입장료 (호감도): {currentRank.entryFee}");
        Debug.Log($"현재 보유 호감도: {PlayerDataManager.Instance.Favorability}");

        // 4. 입장료가 충분한지 확인
        if (PlayerDataManager.Instance.DecreaseFavorability(currentRank.entryFee, false))
        {
            Debug.Log("호감도가 충분하여 게임 씬으로 이동합니다.");
            SceneManager.LoadScene("Game");
        }
        else
        {
            Debug.LogError("호감도가 부족하여 게임을 시작할 수 없습니다!");
            // TODO: "호감도가 부족합니다" UI 팝업 띄우기
        }
    }
    public void LoadSettingScene() => SceneManager.LoadScene("Setting");
    public void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    private void DetermineCurrentRank()
    {
        // 1. PlayerDataManager에서 현재 호감도를 가져옵니다.
        int currentFavor = PlayerDataManager.Instance.Favorability;
        currentRank = null; // 우선 null로 초기화

        // 2. Rank Data List를 순회하며 조건에 맞는 급수를 찾습니다.
        //    (Inspector에서 리스트가 요구 호감도 높은 순으로 정렬되어 있어야 합니다.)
        foreach (var rank in rankDataList)
        {
            if (currentFavor >= rank.requiredFavorability)
            {
                currentRank = rank;
                break; // 가장 적합한 높은 등급을 찾았으므로 즉시 종료
            }
        }

        // 3. 만약 루프를 다 돌아도 맞는 급수를 못 찾았다면(예: 리스트가 비었거나 순서가 잘못된 경우)
        //    가장 마지막에 있는 급수(가장 낮은 등급)를 기본값으로 설정합니다. (안전장치)
        if (currentRank == null && rankDataList.Count > 0)
        {
            currentRank = rankDataList[rankDataList.Count - 1];
        }

        // 4. 결정된 급수를 로그로 출력합니다.
        if (currentRank != null)
        {
            Debug.Log($"현재 플레이어의 급수는 '{currentRank.rankName}' 입니다.");
        }
    }
    public RankData GetCurrentRank() => currentRank;

    #endregion
}