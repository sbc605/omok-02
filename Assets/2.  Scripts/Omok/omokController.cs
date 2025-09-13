using UnityEngine;

public class OmokController : MonoBehaviour
{
    [SerializeField] private Omok omokPrefab;
    [SerializeField] private float cellSize = 0.32f; // 셀 간격
    private Vector2 boardCenter = Vector2.zero; // 오목판 중앙 위치
    private Omok[,] board; // 오목판 상태 저장

    private Omok.MarkerType currentTurn = Omok.MarkerType.Black; // 현재 턴 (흑부터 시작)
        
    private Vector2Int? selectedCell = null; // 임시 선택된 칸

    // 터치시 호출
    public void SelectCell(int row, int col)
    {
        // 이미 착수된 곳 표시 불가
        if (board[row, col].GetMarker() != Omok.MarkerType.None)
            return;

        // 이전 선택 해제
        if (selectedCell.HasValue)
        {
            var prev = selectedCell.Value;
            board[prev.x, prev.y].SetMarker(Omok.MarkerType.None);
        }

        // 임시 선택
        board[row, col].SetMarker(currentTurn == Omok.MarkerType.Black ? Omok.MarkerType.Black : Omok.MarkerType.White);

        selectedCell = new Vector2Int(row, col);
    }

    // 착수버튼 클릭
    public void ConfirmMove(int row, int col)
    {
        if (!selectedCell.HasValue) return; // 선택 없으면 무시

        var cell = selectedCell.Value;
        var marker = board[cell.x, cell.y].GetMarker();

        if (marker == Omok.MarkerType.None) return; // 빈 칸이면 무효

        // 여기서 확정 → 턴 전환
        currentTurn = (currentTurn == Omok.MarkerType.Black)
            ? Omok.MarkerType.White
            : Omok.MarkerType.Black;

        selectedCell = null; // 선택 초기화
    }

    private int boardSize = 13;

    private void Start()
    {
        board = new Omok[boardSize, boardSize];

        // 보드 전체 크기
        float totalSize = (boardSize - 1) * cellSize;

        // 왼쪽 위 시작점 계산
        float startX = boardCenter.x - totalSize / 2f;
        float startY = boardCenter.y + totalSize / 2f;


        for (int r = 0; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                float x = startX + c * cellSize;
                float y = startY - r * cellSize;
                Vector3 pos = new Vector3(x, y, -1);

                // Prefab 생성
                Omok cell = Instantiate(omokPrefab, pos, Quaternion.identity, transform);

                // 초기화
                cell.InitMarker(r, c, this);

                // 배열에 등록
                board[r, c] = cell;
            }
        }
    }

    // --- 클릭시 바로 돌 두기 ---
    public void OnCellClicked(int row, int col)
    {
        // 이미 돌이 있으면 무시
        if (board[row, col].GetMarker() != Omok.MarkerType.None)
            return;

        // 현재 턴의 돌 놓기
        board[row, col].SetMarker(currentTurn);

        // 턴 전환
        currentTurn = (currentTurn == Omok.MarkerType.Black)
            ? Omok.MarkerType.White
            : Omok.MarkerType.Black;
    }
}
