using UnityEngine;

public class OmokController : MonoBehaviour
{
    [SerializeField] private Omok omokPrefab;
    [SerializeField] private float cellSize = 0.32f; // 셀 간격
    private Vector2 boardOrigin = new Vector2(0, -0.24f); // 보드 시작 위치(중심)
    private int boardSize = 15;
    private Omok[,] board; // 오목판 상태 저장

    private Omok.MarkerType currentTurn = Omok.MarkerType.Black; // 현재 턴 (흑부터 시작)

    private Vector2Int? selectedCell = null; // 임시 선택된 칸

    private void Start()
    {
        board = new Omok[boardSize, boardSize];

        // 전체 크기
        float totalSize = (boardSize - 1) * cellSize;

        // 시작 좌표(boardOrigin 기준)
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

    // 임시 선택
    public void OnCellClicked(int row, int col)
    {
        // 이미 착수된 곳 표시 불가
        if (board[row, col].GetMarker() != Omok.MarkerType.None)
            return;

        // 이전 선택 해제
        if (selectedCell.HasValue)
        {
            var prev = selectedCell.Value;
            if (board[prev.x, prev.y].GetMarker() != Omok.MarkerType.None)
                board[prev.x, prev.y].SetMarker(Omok.MarkerType.None);
        }

        // 임시 표시
        board[row, col].SetMarker(currentTurn);
        selectedCell = new Vector2Int(row, col);
    }

    // 착수버튼 연결
    public void ConfirmMove()
    {
        if (!selectedCell.HasValue) return; // 선택 없으면 무시

        var cell = selectedCell.Value;       

        // 턴 교체
        currentTurn = (currentTurn == Omok.MarkerType.Black)
            ? Omok.MarkerType.White
            : Omok.MarkerType.Black;

        selectedCell = null; // 선택 초기화
    }
}
