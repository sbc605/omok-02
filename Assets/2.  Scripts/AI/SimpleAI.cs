using UnityEngine;

public class SimpleAI : MonoBehaviour
{
    // 난이도별 동작 예시
    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty difficulty = Difficulty.Easy;

    // 난수 기반 좌표 선택 (임시)
    public Vector2Int GetNextMove(GameLogic.StoneType[,] board)
    {
        int size = board.GetLength(0);
        while (true)
        {
            int row = Random.Range(0, size);
            int col = Random.Range(0, size);
            if (board[row, col] == GameLogic.StoneType.None)
            {
                return new Vector2Int(row, col);
            }
        }
    }
}
