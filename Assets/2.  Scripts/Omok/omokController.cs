using UnityEngine;

public class OmokController : MonoBehaviour
{
    [SerializeField] private Omok omokPrefab;
    [SerializeField] private float cellSize = 0.32f; // �� ����
    private Vector2 boardCenter = Vector2.zero; // ������ �߾� ��ġ
    private Omok[,] board; // ������ ���� ����

    private Omok.MarkerType currentTurn = Omok.MarkerType.Black; // ���� �� (����� ����)
        
    private Vector2Int? selectedCell = null; // �ӽ� ���õ� ĭ

    // ��ġ�� ȣ��
    public void SelectCell(int row, int col)
    {
        // �̹� ������ �� ǥ�� �Ұ�
        if (board[row, col].GetMarker() != Omok.MarkerType.None)
            return;

        // ���� ���� ����
        if (selectedCell.HasValue)
        {
            var prev = selectedCell.Value;
            board[prev.x, prev.y].SetMarker(Omok.MarkerType.None);
        }

        // �ӽ� ����
        board[row, col].SetMarker(currentTurn == Omok.MarkerType.Black ? Omok.MarkerType.Black : Omok.MarkerType.White);

        selectedCell = new Vector2Int(row, col);
    }

    // ������ư Ŭ��
    public void ConfirmMove(int row, int col)
    {
        if (!selectedCell.HasValue) return; // ���� ������ ����

        var cell = selectedCell.Value;
        var marker = board[cell.x, cell.y].GetMarker();

        if (marker == Omok.MarkerType.None) return; // �� ĭ�̸� ��ȿ

        // ���⼭ Ȯ�� �� �� ��ȯ
        currentTurn = (currentTurn == Omok.MarkerType.Black)
            ? Omok.MarkerType.White
            : Omok.MarkerType.Black;

        selectedCell = null; // ���� �ʱ�ȭ
    }

    private int boardSize = 13;

    private void Start()
    {
        board = new Omok[boardSize, boardSize];

        // ���� ��ü ũ��
        float totalSize = (boardSize - 1) * cellSize;

        // ���� �� ������ ���
        float startX = boardCenter.x - totalSize / 2f;
        float startY = boardCenter.y + totalSize / 2f;


        for (int r = 0; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                float x = startX + c * cellSize;
                float y = startY - r * cellSize;
                Vector3 pos = new Vector3(x, y, -1);

                // Prefab ����
                Omok cell = Instantiate(omokPrefab, pos, Quaternion.identity, transform);

                // �ʱ�ȭ
                cell.InitMarker(r, c, this);

                // �迭�� ���
                board[r, c] = cell;
            }
        }
    }

    // --- Ŭ���� �ٷ� �� �α� ---
    public void OnCellClicked(int row, int col)
    {
        // �̹� ���� ������ ����
        if (board[row, col].GetMarker() != Omok.MarkerType.None)
            return;

        // ���� ���� �� ����
        board[row, col].SetMarker(currentTurn);

        // �� ��ȯ
        currentTurn = (currentTurn == Omok.MarkerType.Black)
            ? Omok.MarkerType.White
            : Omok.MarkerType.Black;
    }
}
