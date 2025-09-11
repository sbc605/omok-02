using UnityEngine;

public class OmokController : MonoBehaviour
{
    [SerializeField] private Omok omokPrefab;
    private int boardSize = 15; // 15x15 ������

    private Omok[,] board; // ������ ���� ����
    private Omok.MarkerType currentTurn = Omok.MarkerType.Black; // ���� �� (����� ����)
        

    // ���� �ʱ�ȭ
    private void InitBoard()
    {
        board = new Omok[boardSize, boardSize];

        float spacing = 1f; // ����ĭ ����
        Vector2 startPos = new Vector2();

    }

    public void OnBlockClicked(int row, int col)
    {
        if (board[row, col].GetMarker() != Omok.MarkerType.None)
            return; // �̹� ���� ������ ����

        board[row, col].SetMarker(currentTurn); // ���� �Ͽ� �´� �� ����

        // �� ����
        currentTurn = (currentTurn == Omok.MarkerType.Black) ? Omok.MarkerType.White : Omok.MarkerType.Black;
    }
}
