using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    [SerializeField] private GameLogic gameLogic;
    [SerializeField] private GameLogic.PlayerType playerType;
    [SerializeField] private Image Notyourturn; // 내턴
    [SerializeField] private Image Notmyturn; // 상대턴

    private void Start()
    {
        if (gameLogic != null)
        {
            gameLogic.OnTurnChanged += UpdateUI; // 턴 변경시 UI 업데이트
            UpdateUI(gameLogic.currentPlayer); // 초기 UI 설정
        }
    }

    private void OnDestroy()
    {
        if (gameLogic != null)
        {
            gameLogic.OnTurnChanged -= UpdateUI; // 이벤트 해제
        }
    }

    public GameLogic.PlayerType GetPlayerType() => playerType;

    // 턴 변경시 호출
    public void UpdateUI(GameLogic.PlayerType currentTurnplayer)
    {
        // 내 턴이면 착수버튼 활성화
        bool isMyTurn = (currentTurnplayer == playerType);
        Notmyturn.gameObject.SetActive(!isMyTurn);
        Notyourturn.gameObject.SetActive(isMyTurn);
    }
}
