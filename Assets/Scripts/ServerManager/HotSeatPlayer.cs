using UnityEngine;
using System.Collections;

public class HotSeatPlayer : IPlayer
{
    public event ActedHandler OnActEvent;

    private ClientManager clientManager;
    private PlayerColor playerColor;
    private bool isActive = false;

    public HotSeatPlayer(PlayerColor playerColor)
    {
        this.playerColor = playerColor;
        clientManager = ClientManager.getInstance();
        clientManager.ActEvent += OnPlayerAct;
    }

    public void SetGameSituation(GameSituation gameSituation)
    {
        if ((gameSituation.IsWhiteMoving && (playerColor == PlayerColor.White)) || 
            (!gameSituation.IsWhiteMoving && (playerColor == PlayerColor.Black))) {
            isActive = true;
            clientManager.SetGameSituation(gameSituation);
        }
        else
        {
            isActive = false;
        }
        
    }

    private void OnPlayerAct(PlayerAct playerAct)
    {
        if (isActive)
        {
            OnActEvent(playerAct);
        }
    }
}
