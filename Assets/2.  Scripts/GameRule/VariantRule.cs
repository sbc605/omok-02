using UnityEngine;
using System.Collections.Generic;

public class VariantRule : MonoBehaviour
{
    [SerializeField] private GameLogic gameLogic;

    [Header("Phase 1 설정")]
    [SerializeField] private int triggerTurn = 10;
    [SerializeField] private int removeStone = 2; 

    [Tooltip("초기 중앙 흑돌은 제거 대상에서 제외할지")]
    [SerializeField] private bool excludeCenterStone = true;

    private bool phase1Triggered = false;
    private int placementCount = 0; // 착수 횟수(초기 중앙 흑 포함)

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

    // GameLogic에서 한 수 둘 때마다 호출됨(초기 중앙 흑 셋업 끝난 뒤에도 1회 호출)
    private void OnTurnChanged(GameLogic.PlayerType _)
    {
        placementCount++;

        if (!phase1Triggered && placementCount >= triggerTurn)
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

        Debug.Log($"[변이 Rule] Phase1 : 흑/백 각각 {removeStone}개 제거");
    }

    private void RandomRemove(GameLogic gl, GameLogic.StoneType color, int count)
    {
        var stones = GetAllStones(gl, color, excludeCenterStone);

        // 제거 개수 보정
        int toRemove = Mathf.Min(count, stones.Count);
        for (int i = 0; i < toRemove; i++)
        {
            int idx = Random.Range(0, stones.Count);
            var (r, c) = stones[idx];
            // gl.ClearStone(r, c);    //구현 후 GameLogic에 추가 할 함수 
            stones.RemoveAt(idx);
        }
    }

    private List<(int r, int c)> GetAllStones(GameLogic gl, GameLogic.StoneType color, bool excludeCenter)
    {
        var list = new List<(int, int)>();
        int size = gl.boardSize;

        // 중앙 좌표(초기 흑)
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