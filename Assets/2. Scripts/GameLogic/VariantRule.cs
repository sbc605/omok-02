using UnityEngine;
using System.Collections.Generic;

public class VariantRule : MonoBehaviour
{
    [SerializeField] private GameLogic gameLogic;

    [Header("Phase 1 설정")]
    [SerializeField] private int minTriggerTurn = 10;   // 최소 턴
    [SerializeField] private int maxTriggerTurn = 20;   // 최대 턴

    // ▶ UI에서 접근할 수 있도록 public 읽기 전용 프로퍼티
    public int TriggerTurn { get; private set; }        

    [Header("제거될 돌 개수")]
    [SerializeField] private int removeStone = 2;

    [Header("초기 중앙 흑돌은 제거 대상에서 제외할지")]
    [SerializeField] private bool excludeCenterStone = true;

    private bool phase1Triggered = false;
    private int placementCount = 0; // 착수 횟수(초기 중앙 흑 포함)

    // ▶ UI가 “발동 턴”을 알 수 있게 이벤트 추가
    public event System.Action<int> OnPhaseReady;      

    private void OnEnable()
    {
        if (gameLogic != null)
            gameLogic.OnTurnChanged += OnTurnChanged;
    }

    private void OnDisable()
    {
        if (gameLogic != null)
            gameLogic.OnTurnChanged -= OnTurnChanged;
    }

    private void Start()
    {
        // 게임 시작 시  [minTriggerTurn, maxTriggerTurn]  사이에서 무작위 발동 턴 선택
        TriggerTurn = UnityEngine.Random.Range(minTriggerTurn, maxTriggerTurn + 1);

        Debug.Log($"[VariantRule] 이번 게임 변이 발동 턴: {TriggerTurn}");

        // ▶ UI에 “발동 턴”을 알림
        OnPhaseReady?.Invoke(TriggerTurn);
    }

    private void OnTurnChanged(GameLogic.PlayerType _)
    {
        placementCount++;

        if (!phase1Triggered && placementCount >= TriggerTurn)
        {
            phase1Triggered = true;
            TriggerPhase1();
        }
    }

    private void TriggerPhase1()
    {
        // 흑/백 각각 랜덤 n개 제거
        RandomRemove(gameLogic, GameLogic.StoneType.Black, removeStone);
        RandomRemove(gameLogic, GameLogic.StoneType.White, removeStone);

        Debug.Log($"[VariantRule] Phase1 발동: 흑/백 각각 {removeStone}개 제거");
    }

    private void RandomRemove(GameLogic gl, GameLogic.StoneType color, int count)
    {
        var stones = GetAllStones(gl, color, excludeCenterStone);

        int toRemove = Mathf.Min(count, stones.Count);
        for (int i = 0; i < toRemove; i++)
        {
            int idx = Random.Range(0, stones.Count);
            var (r, c) = stones[idx];
            gl.ClearStone(r, c);      // GameLogic에 이미 메서드 있음
            stones.RemoveAt(idx);
        }
    }

    private List<(int r, int c)> GetAllStones(GameLogic gl, GameLogic.StoneType color, bool excludeCenter)
    {
        var list = new List<(int, int)>();
        int size = gl.boardSize;

        int centerR = size / 2;
        int centerC = size / 2;

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                if (gl.GetStone(r, c) == color)
                {
                    if (excludeCenter && r == centerR && c == centerC)
                        continue;
                    list.Add((r, c));
                }
            }
        }
        return list;
    }
}