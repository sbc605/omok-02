using UnityEngine;

public class OmokController : MonoBehaviour
{
    [SerializeField] private Omok omokPrefab;
    private int boardSize = 15; // 15x15 오목판

    private Omok[,] board; // 오목판 상태 저장
    private Omok.MarkerType currentTurn = Omok.MarkerType.Black; // 현재 턴 (흑부터 시작)
        

    // 보드 초기화
    private void InitBoard()
    {
        board = new Omok[boardSize, boardSize];

        float spacing = 1f; // 오목칸 간격
        Vector2 startPos = new Vector2();

    }

    public void OnBlockClicked(int row, int col)
    {
        if (board[row, col].GetMarker() != Omok.MarkerType.None)
            return; // 이미 돌이 있으면 무시

        board[row, col].SetMarker(currentTurn); // 현재 턴에 맞는 돌 놓기

        // 턴 변경
        currentTurn = (currentTurn == Omok.MarkerType.Black) ? Omok.MarkerType.White : Omok.MarkerType.Black;
    }
}
