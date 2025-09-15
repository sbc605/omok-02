using UnityEngine;


public class PlayerState : MonoBehaviour
{
    public enum PlayerType { None, PlayerA, PlayerB }
    public PlayerType playerType = PlayerType.None;

    private bool isMyTurn;


}
