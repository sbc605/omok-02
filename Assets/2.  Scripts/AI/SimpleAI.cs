using UnityEngine;

public class SimpleAI : MonoBehaviour
{
    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty difficulty = Difficulty.Easy;

    public Vector2Int lastMove = new Vector2Int(-1, -1);

    public Vector2Int GetNextMove(GameLogic.StoneType[,] board)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                return GetRandomMove(board);
            case Difficulty.Normal:
                return GetNearbyMove(board);
            case Difficulty.Hard:
                return GetBestMove(board);
            default:
                return GetRandomMove(board);
        }
    }

    // Easy : 완전 랜덤
    private Vector2Int GetRandomMove(GameLogic.StoneType[,] board)
    {
        int size = board.GetLength(0);
        while (true)
        {
            int row = Random.Range(0, size);
            int col = Random.Range(0, size);
            if (board[row, col] == GameLogic.StoneType.None)
                return new Vector2Int(row, col);
        }
    }

    // Normal : 마지막 착수 주변에서 랜덤
    private Vector2Int GetNearbyMove(GameLogic.StoneType[,] board)
    {
        int size = board.GetLength(0);

        if (lastMove.x >= 0 && lastMove.y >= 0)
        {
            for (int attempt = 0; attempt < 50; attempt++)
            {
                int row = lastMove.x + Random.Range(-2, 3);
                int col = lastMove.y + Random.Range(-2, 3);
                if (row >= 0 && row < size && col >= 0 && col < size &&
                    board[row, col] == GameLogic.StoneType.None)
                {
                    return new Vector2Int(row, col);
                }
            }
        }
        return GetRandomMove(board);
    }

    // Hard : 점수 기반
    private Vector2Int GetBestMove(GameLogic.StoneType[,] board)
    {
        int size = board.GetLength(0);
        Vector2Int bestMove = new Vector2Int(-1, -1);
        int bestScore = int.MinValue;

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                if (board[r, c] != GameLogic.StoneType.None) continue;

                int score = EvaluatePosition(board, r, c);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = new Vector2Int(r, c);
                }
            }
        }
        return bestMove.x >= 0 ? bestMove : GetRandomMove(board);
    }

    private int EvaluatePosition(GameLogic.StoneType[,] board, int row, int col)
    {
        int score = 0;
        Vector2Int[] dirs = {
            new Vector2Int(1,0), new Vector2Int(0,1),
            new Vector2Int(1,1), new Vector2Int(1,-1)
        };

        foreach (var d in dirs)
        {
            int myCount = CountConsecutive(board, row, col, d, GameLogic.StoneType.White);
            int oppCount = CountConsecutive(board, row, col, d, GameLogic.StoneType.Black);

            score += myCount * 10;
            score += oppCount * 15; // 막는 걸 더 중요시
        }

        return score;
    }

    private int CountConsecutive(GameLogic.StoneType[,] board, int row, int col, Vector2Int dir, GameLogic.StoneType type)
    {
        int size = board.GetLength(0);
        int count = 0;

        int r = row + dir.x;
        int c = col + dir.y;

        while (r >= 0 && r < size && c >= 0 && c < size && board[r, c] == type)
        {
            count++;
            r += dir.x;
            c += dir.y;
        }

        return count;
    }
}
