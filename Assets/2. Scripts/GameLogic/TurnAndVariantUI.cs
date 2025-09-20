using UnityEngine;
using TMPro;

public class TurnAndVariantUI : MonoBehaviour
{
    [SerializeField] private GameLogic gameLogic;
    [SerializeField] private VariantRule variantRule;
    [SerializeField] private TextMeshProUGUI turnText;       // 현재 턴
    [SerializeField] private TextMeshProUGUI variantText;    // 변이 발동 턴

    private void OnEnable()
    {
        if (gameLogic != null)
            gameLogic.OnTurnChanged += UpdateTurn;

        if (variantRule != null)
            variantRule.OnPhaseReady += UpdateVariantInfo;   
    }

    private void OnDisable()
    {
        if (gameLogic != null)
            gameLogic.OnTurnChanged -= UpdateTurn;

        if (variantRule != null)
            variantRule.OnPhaseReady -= UpdateVariantInfo;
    }

    private void Start()
    {
        UpdateTurn(gameLogic.currentPlayer);
        // 게임 시작 시 초기 표시 (아직 발동 턴을 모른다면 빈 문자열 or "??")
        if (variantRule != null)
            variantText.text = $"변이 발동 : {variantRule.TriggerTurn} 턴"; 
    }

    private void UpdateTurn(GameLogic.PlayerType _)
    {
        turnText.text = $"현재 턴 : {gameLogic.turnCount} 턴";
    }

    private void UpdateVariantInfo(int triggerTurn)
    {
        variantText.text = $"변이 발동 : {triggerTurn} 턴";
    }
}