using UnityEngine;

public class OmokController : MonoBehaviour
{
    [SerializeField] private Omok omokPrefab;
    private int boardSize = 15; // 15x15 ������
    private int cellSize = 1; // ���� ����

    private Omok[,] board; // ������ ���� ����
    private Omok.MarkerType currentTurn = Omok.MarkerType.Black; // ���� �� (����� ����)

    private void Start()
    {
        board = new Omok[boardSize, boardSize];
    }

    private void Update()
    {
        
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
