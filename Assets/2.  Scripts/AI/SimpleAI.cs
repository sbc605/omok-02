using UnityEngine;

public class SimpleAI : MonoBehaviour
{
    // ���̵��� ���� ����
    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty difficulty = Difficulty.Easy;

    // ���� ��� ��ǥ ���� (�ӽ�)
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
