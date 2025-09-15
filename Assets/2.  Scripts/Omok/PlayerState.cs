using UnityEngine;


public class PlayerState : MonoBehaviour
{
    private GameLogic.PlayerType playerType;
    private bool isMyTurn;

    public PlayerState(bool isTurn)
    {
        isMyTurn = isTurn;
        playerType = isTurn ? GameLogic.PlayerType.player : GameLogic.PlayerType.CPU;
    }


}
