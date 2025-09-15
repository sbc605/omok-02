using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    private GameLogic.PlayerType playerType;
    private bool isMyTurn;
    [SerializeField] private Image Notyourturn;
    [SerializeField] private Image Notmyturn;

    private void Start()
    {
        
    }

    // 턴 변경시 호출
    public void UpdateUI(GameLogic.PlayerType currentTurn)
    {
        // 내 턴이면 착수버튼 활성화
        if (currentTurn == playerType) // 내 턴
        {
            Notmyturn.gameObject.SetActive(false);
            Notyourturn.gameObject.SetActive(true);
        }
        else
        {
            Notmyturn.gameObject.SetActive(true);
            Notyourturn.gameObject.SetActive(false);
        }
    }
}
