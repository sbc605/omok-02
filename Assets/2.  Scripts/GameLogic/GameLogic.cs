using UnityEngine;
using System;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour
{
    // ★ 현재 턴 수 (1턴 = 흑 첫 수부터 시작)
    public int turnCount = 1;

    public int boardSize = 15; // 몇 줄짜리 보드인지 (15x15나 19x19)
    private StoneType[,] board;   // StoneType.None / Black / White

    public enum StoneType { None, Black, White }
    private StoneType currentTurn = StoneType.Black; // 흑 선공

    public enum PlayerType { player, CPU }; //플레이어 or AI
    public PlayerType currentPlayer;
    public event Action<PlayerType> OnTurnChanged; // 턴 변경시 호출
    // ★ 금수 좌표 전체를 알리는 이벤트
    public event Action<List<(int r,int c)>> OnForbiddenPositionsChanged;

    public enum GameResult { None, Win, Lose, Draw }

    void Start()
    {
        board = new StoneType[boardSize, boardSize];
        currentTurn = StoneType.Black;
        currentPlayer = PlayerType.player;

        int centerRow = boardSize / 2;
        int centerCol = boardSize / 2;
        board[centerRow, centerCol] = StoneType.Black;

        // 턴 변경
        currentTurn = StoneType.White;
        currentPlayer = PlayerType.CPU;
        OnTurnChanged?.Invoke(currentPlayer);
    }

    public bool PlaceStone(int row, int col)
    {
        // 범위 / 중복 체크
        if (row < 0 || row >= boardSize || col < 0 || col >= boardSize) return false;
        if (board[row, col] != StoneType.None) return false;

        // 금수 검사 : 흑만 적용
        if (currentTurn == StoneType.Black && IsForbiddenMove(row, col))
        {
            Debug.Log("금수 위치입니다.");
            // ★ 금수 좌표 갱신(모든 금수 위치를 계산해 이벤트로 알림)
            OnForbiddenPositionsChanged?.Invoke(GetAllForbiddenPositions());
            return false;
        }

        board[row, col] = currentTurn;

        if (CheckWin(row, col))
        {
            EndGame(GameResult.Win);
            return true;
        }

        if (IsBoardFull())
        {
            EndGame(GameResult.Draw);
            return true;
        }

        // 턴 전환
        currentTurn = (currentTurn == StoneType.Black) ? StoneType.White : StoneType.Black;
        currentPlayer = (currentPlayer == PlayerType.player) ? PlayerType.CPU : PlayerType.player;
        turnCount++;

        OnTurnChanged?.Invoke(currentPlayer);

        // ★ 흑 차례가 되면 그 시점의 금수 좌표 전체를 다시 알림
        if (currentTurn == StoneType.Black)
            OnForbiddenPositionsChanged?.Invoke(GetAllForbiddenPositions());

        return true;
    }

    // board, currentTurn 접근용
    public StoneType GetStone(int row, int col) => board[row, col];
    public StoneType GetCurrentTurn() => currentTurn;

    // ───────── 금수 판정 ─────────
    private bool IsForbiddenMove(int row, int col)
    {
        board[row, col] = StoneType.Black;

        bool overline     = CreatesOverline(row, col);
        int openThreeCnt  = CountOpenThree(row, col);
        int openFourCnt   = CountOpenFour (row, col);

        board[row, col] = StoneType.None;
        return overline || openThreeCnt >= 2 || openFourCnt >= 2;
    }

    // ★ 현재 보드에서 흑 금수 좌표 전체 반환
    public List<(int r,int c)> GetAllForbiddenPositions()
    {
        var list = new List<(int,int)>();
        if (currentTurn != StoneType.Black) return list; // 흑 차례일 때만 계산
        for (int r = 0; r < boardSize; r++)
            for (int c = 0; c < boardSize; c++)
                if (board[r,c] == StoneType.None && IsForbiddenMove(r,c))
                    list.Add((r,c));
        return list;
    }

    private bool CreatesOverline(int r, int c)
    {
        Vector2Int[] dirs = { new Vector2Int(1,0), new Vector2Int(0,1),
                              new Vector2Int(1,1), new Vector2Int(1,-1) };

        foreach (var d in dirs)
        {
            int count = 1;
            count += CountContinuous(r, c, d.x, d.y, StoneType.Black);
            count += CountContinuous(r, c,-d.x,-d.y, StoneType.Black);
            if (count >= 6) return true; // 장목
        }
        return false;
    }

    private int CountOpenThree(int r, int c)
    {
        // 열린 "3" 패턴(○●●●○)을 세는 간단한 예시
        // 실제 대회 규칙에 맞게 세분화 가능
        return CountOpenPattern(r, c, 3);
    }

    private int CountOpenFour(int r, int c)
    {
        // 열린 "4" 패턴(○●●●●○) 카운트
        return CountOpenPattern(r, c, 4);
    }

    private int CountOpenPattern(int r, int c, int targetLen)
    {
        Vector2Int[] dirs = { new Vector2Int(1,0), new Vector2Int(0,1),
                              new Vector2Int(1,1), new Vector2Int(1,-1) };
        int cnt = 0;
        foreach (var d in dirs)
        {
            int forward = CountContinuous(r, c, d.x, d.y, StoneType.Black);
            int back    = CountContinuous(r, c,-d.x,-d.y, StoneType.Black);
            int len     = forward + back + 1;

            if (len == targetLen)
            {
                bool openFront = IsEmpty(r + (forward + 1) * d.x,
                                         c + (forward + 1) * d.y);
                bool openBack  = IsEmpty(r - (back + 1) * d.x,
                                         c - (back + 1) * d.y);
                if (openFront && openBack) cnt++;
            }
        }
        return cnt;
    }

    private bool IsEmpty(int r, int c)
    {
        return r >= 0 && r < boardSize && c >= 0 && c < boardSize &&
               board[r, c] == StoneType.None;
    }

    private int CountContinuous(int r, int c, int dx, int dy, StoneType color)
    {
        int count = 0;
        int nr = r + dx, nc = c + dy;
        while (nr >= 0 && nr < boardSize && nc >= 0 && nc < boardSize &&
               board[nr, nc] == color)
        {
            count++;
            nr += dx; nc += dy;
        }
        return count;
    }

    // ───────── 기존 승패 로직 ─────────
    
    private bool CheckWin(int r, int c) // 승리 판독
    {
        Vector2Int[] dirs = { new Vector2Int(1,0), new Vector2Int(0,1),
                              new Vector2Int(1,1), new Vector2Int(1,-1) };

        foreach (var d in dirs)
        {
            int count = 1;
            count += CountContinuous(r, c, d.x, d.y, currentTurn);
            count += CountContinuous(r, c,-d.x,-d.y, currentTurn);
            if (count >= 5) return true;
        }
        return false;
    }

    private bool IsBoardFull() // 무승부 판독
    {
        for (int r = 0; r < boardSize; r++)
            for (int c = 0; c < boardSize; c++)
                if (board[r, c] == StoneType.None) return false;
        return true;
    }

    private void EndGame(GameResult result)
    {
        // UI 출력, 게임 오버 패널 띄우기 등
        Debug.Log($"Game Over : {result}");
        // 필요하다면 입력 막기, 재시작 버튼 활성화 등
    }
}