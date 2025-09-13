using UnityEngine;

public class OmokController : MonoBehaviour
{
    [SerializeField] private Omok omokPrefab;
    [SerializeField] private float cellSize = 0.32f; // �� ����
    private Vector2 boardOrigin = new Vector2(0, -0.24f); // ���� ���� ��ġ(�߽�)
    private int boardSize = 15;
    private Omok[,] board; // ������ ���� ����

    private Omok.MarkerType currentTurn = Omok.MarkerType.Black; // ���� �� (����� ����)

    private Vector2Int? selectedCell = null; // �ӽ� ���õ� ĭ

    private void Start()
    {
        board = new Omok[boardSize, boardSize];

        // ��ü ũ��
        float totalSize = (boardSize - 1) * cellSize;

        // ���� ��ǥ(boardOrigin ����)
        float startX = boardOrigin.x - totalSize / 2f;
        float startY = boardOrigin.y + totalSize / 2f;

        for (int r = 0; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                float x = startX + c * cellSize;
                float y = startY - r * cellSize;
                Vector3 pos = new Vector3(x, y, -1);

                Omok cell = Instantiate(omokPrefab, pos, Quaternion.identity, transform);
                cell.InitMarker(r, c, this);
                board[r, c] = cell;
            }
        }
    }

    // �ӽ� ����
    public void OnCellClicked(int row, int col)
    {
        // �̹� ������ �� ǥ�� �Ұ�
        if (board[row, col].GetMarker() != Omok.MarkerType.None)
            return;

        // ���� ���� ����
        if (selectedCell.HasValue)
        {
            var prev = selectedCell.Value;
            if (board[prev.x, prev.y].GetMarker() != Omok.MarkerType.None)
                board[prev.x, prev.y].SetMarker(Omok.MarkerType.None);
        }

        // �ӽ� ǥ��
        board[row, col].SetMarker(currentTurn);
        selectedCell = new Vector2Int(row, col);
    }

    // ������ư ����
    public void ConfirmMove()
    {
        if (!selectedCell.HasValue) return; // ���� ������ ����

        var cell = selectedCell.Value;       

        // �� ��ü
        currentTurn = (currentTurn == Omok.MarkerType.Black)
            ? Omok.MarkerType.White
            : Omok.MarkerType.Black;

        selectedCell = null; // ���� �ʱ�ȭ
    }
}
