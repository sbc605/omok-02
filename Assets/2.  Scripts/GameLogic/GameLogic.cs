using UnityEngine;

public class GameLogic : MonoBehaviour
{
   // 몇 줄짜리 보드인지 (15x15나 19x19)
   [SerializeField] private int boardSize = 15;
   // 현재 돌 배치 상태
   private StoneType[,] board;   // StoneType.None / Black / White
   
   // 바둑돌 관련
   public enum StoneType { None, Black, White }
   public GameObject blackstone; // 바둑돌 오브젝트 
   public GameObject whitestone; // 바둑돌 오브젝트
   private StoneType currentTurn = StoneType.Black; // 흑 선공
   
   public BlockController blockController;
   public enum PlayerType{ player, CPU }; //플레이어 or AI

   public GameObject Gomokuboard; // 바둑판 오브젝트
   public enum GameResult { None, Win, Lose, Draw }
  
   void Start() //보드 배열 초기화 및 흑돌 중앙 착수 시작
   {
      board = new StoneType[boardSize, boardSize];
      currentTurn = StoneType.Black;

      // 중앙 좌표 계산
      int center = boardSize / 2;

      // 가운데 자동 착수 (PlaceStone을 그대로 호출)
      PlaceStone(center, center);

      // 첫 수가 자동으로 흑이 두어졌으니 턴을 백으로 넘김
      currentTurn = StoneType.White;
   }
   public bool PlaceStone(int row, int col) // 착수
   {
      // 범위/중복 체크
      if (row < 0 || row >= boardSize || col < 0 || col >= boardSize) return false;
      if (board[row, col] != StoneType.None) return false;

      // 보드 데이터 갱신
      board[row, col] = currentTurn;

      // 실제 오브젝트 배치
      GameObject prefab = (currentTurn == StoneType.Black) ? blackstone : whitestone;
      Instantiate(prefab, blockController.GetWorldPosition(row, col), Quaternion.identity, Gomokuboard.transform);

      // 승리 판정
      if (CheckWin(row, col))
      {
         EndGame(GameResult.Win);
         return true;
      }

      // 무승부
      if (IsBoardFull())
      {
         EndGame(GameResult.Draw);
         return true;
      }

      // 턴 전환
      currentTurn = (currentTurn == StoneType.Black) ? StoneType.White : StoneType.Black;
      return true;
   }
   
   private bool CheckWin(int r, int c) // 승부 여부 체크
   {
      Vector2Int[] dirs = { new Vector2Int(1,0), new Vector2Int(0,1),
         new Vector2Int(1,1), new Vector2Int(1,-1) };

      foreach (var d in dirs)
      {
         int count = 1;
         count += CountStones(r, c, d.x, d.y);
         count += CountStones(r, c, -d.x, -d.y);
         if (count >= 5) return true;
      }
      return false;
   }
   

   private int CountStones(int r, int c, int dx, int dy)
   {
      int cnt = 0;
      int nr = r + dx, nc = c + dy;
      while (nr >= 0 && nr < boardSize && nc >= 0 && nc < boardSize &&
             board[nr, nc] == currentTurn)
      {
         cnt++;
         nr += dx; nc += dy;
      }
      return cnt;
   }
   private bool IsBoardFull() // 무승부(꽉찼을 때)
   {
      for (int r = 0; r < boardSize; r++)
      {
         for (int c = 0; c < boardSize; c++)
         {
            if (board[r, c] == StoneType.None) return false;
         }
      }
      return true;
   }
   private void EndGame(GameResult result)
   {
      // UI 출력, 게임 오버 패널 띄우기 등
      Debug.Log($"Game Over : {result}");
      // 필요하다면 입력 막기, 재시작 버튼 활성화 등
   }
}
