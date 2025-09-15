using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class OmokController : MonoBehaviour
{
    [SerializeField] private Omok omokPrefab;
    [SerializeField] private GameLogic gameLogic;
    [SerializeField] private float cellSize = 0.32f; // 셀 간격
    public Vector2 boardOrigin = new Vector2(0, -0.24f); // 보드 시작 위치(중심)

    private Omok[,] board; // 오목판 상태 저장    
    private int? selectedRow = null;
    private int? selectedCol = null;

    private void Start()
    {
        int boardSize = gameLogic.boardSize; // 게임로직에서 보드 크기 가져오기
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

        // 시작시 중앙 흑돌 착수
        int centerRow = boardSize / 2;
        int centerCol = boardSize / 2;
        board[centerRow, centerCol].SetMarker(Omok.MarkerType.Black);
    }

    // 좌표 클릭시 임시 선택 표시
    public void OnCellClicked(int row, int col)
    {
        // 이전 선택 지우기
        if (selectedRow.HasValue && selectedCol.HasValue)
        {
            var prevCell = board[selectedRow.Value, selectedCol.Value];
            if (prevCell.GetMarker() != Omok.MarkerType.None)
                prevCell.SetMarker(Omok.MarkerType.None);
        }

        // 새 선택 표시
        selectedRow = row;
        selectedCol = col;

        // selector 표시
        board[row, col].SetMarker(Omok.MarkerType.Preview);
    }

    // 착수 확정
    public void ConfirmMove()
    {
        if (!selectedRow.HasValue || !selectedCol.HasValue) return; // 선택된 셀이 없음 

        int row = selectedRow.Value;
        int col = selectedCol.Value;

        if (gameLogic.PlaceStone(row, col)) // 게임로직에서 착수 성공
        {
            // 착수 성공 시 오목판에도 표시
            var stone = gameLogic.GetStone(row, col);
            var markerType = (stone == GameLogic.StoneType.Black) ? Omok.MarkerType.Black : Omok.MarkerType.White;

            board[row, col].SetMarker(markerType);
        }

        // 선택 해제
        selectedRow = null;
        selectedCol = null;
    }
}
