using UnityEngine;

public class OmokController : MonoBehaviour
{
    [SerializeField] private PlayerState playerState;
    [SerializeField] private Omok omokPrefab;
    [SerializeField] private GameLogic gameLogic;
    [SerializeField] private float cellSize = 0.32f; // 셀 간격
    private Vector2 boardOrigin = new Vector2(0, -0.24f); // 보드 시작 위치(중심)

    private Omok[,] board; // 오목판 상태 저장 
    private int? selectedRow = null;
    private int? selectedCol = null;
    
    private void OnDisable() // 턴마다 갱신 - 이재현
    {
        gameLogic.OnTurnChanged -= UpdateForbiddenMarkers;
    }
    
    private void Start()
    {
        int boardSize = gameLogic.boardSize; // 게임로직에서 보드 크기 가져오기
        board = new Omok[boardSize, boardSize];
        
        gameLogic.OnTurnChanged += UpdateForbiddenMarkers; // 턴마다 갱신 - 이재현

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

        // 중앙에 자동 흑돌 착수
        int centerRow = boardSize / 2;
        int centerCol = boardSize / 2;
        board[centerRow, centerCol].SetMarker(Omok.MarkerType.Black);
    }

    // 좌표 클릭시 UI 표시
    public void OnCellClicked(int row, int col)
    {
        // 내 턴이 아닌 경우 무시
        if (playerState.GetPlayerType() != gameLogic.currentPlayer)
        {
            Debug.Log("상대방의 턴입니다.");
            return;
        }

        // 이미 돌이 있는 곳은 무시
        if (gameLogic.GetStone(row, col) != GameLogic.StoneType.None)
        {
            Debug.Log("이미 돌이 있는 곳입니다.");
            return;
        }

        // 이전 선택 셀이 비어있는 경우 None으로 되돌림
        if (selectedRow.HasValue && selectedCol.HasValue)
        {
            var prevCell = board[selectedRow.Value, selectedCol.Value];
            if (prevCell.GetMarker() != Omok.MarkerType.None)
                prevCell.SetMarker(Omok.MarkerType.None);
        }

        // 새 좌표 생성
        selectedRow = row;
        selectedCol = col;

        // selector 표시
        board[row, col].SetMarker(Omok.MarkerType.Preview);
    }

    // 착수 확인
    public void ConfirmMove()
    {
        if (!selectedRow.HasValue || !selectedCol.HasValue) return;

        // 내 턴이 맞는지 확인
        if (playerState.GetPlayerType() != gameLogic.currentPlayer)
        {
            Debug.Log("상대방의 턴입니다.");
            return;
        }

        int row = selectedRow.Value;
        int col = selectedCol.Value;

        if (gameLogic.PlaceStone(row, col)) // 게임로직에서 착수 시도
        {
            // 착수 성공 시 오목판에 표시
            var stone = gameLogic.GetStone(row, col);
            var markerType = (stone == GameLogic.StoneType.Black) ? Omok.MarkerType.Black : Omok.MarkerType.White;

            board[row, col].SetMarker(markerType);
        }

        // 자리 비움
        selectedRow = null;
        selectedCol = null;
    }

    private void UpdateForbiddenMarkers(GameLogic.PlayerType currentPlayer) // 금수마크출력 - 이재현
    {
        // 모든 비어 있는 칸을 None으로 초기화
        for (int r = 0; r < board.GetLength(0); r++)
        {
            for (int c = 0; c < board.GetLength(1); c++)
            {
                if (gameLogic.GetStone(r, c) == GameLogic.StoneType.None)
                    board[r, c].SetMarker(Omok.MarkerType.None);
            }
        }

        // 흑 차례일 때만 금수 위치를 표시
        if (gameLogic.GetCurrentTurn() == GameLogic.StoneType.Black)
        {
            var forbiddenList = gameLogic.GetAllForbiddenPositions();
            foreach (var (r, c) in forbiddenList)
            {
                board[r, c].SetMarker(Omok.MarkerType.Forbidden);
            }
        }
    }
}
