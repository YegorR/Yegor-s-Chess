using UnityEngine;
using System.Collections;

public class HumanPlayer : IPlayer
{
    private ClientManager clientManager;
    private PlayerColor playerColor;

    public event PlayerActedEventHandler PlayerActedEvent;

    public HumanPlayer(PlayerColor playerColor)
    {
        this.playerColor = playerColor;
        clientManager = ClientManager.getInstance();
        clientManager.ActEvent += OnPlayerMoved;
    }

    public void SetGameSituation(GameSituation gameSituation)
    {
        clientManager.SetGameSituation(gameSituation);
        if ((playerColor == PlayerColor.White) && (gameSituation.IsWhiteMoving))
        {
            clientManager.Block(false, PlayerColor.White);
        }
        else if ((playerColor == PlayerColor.Black) && (!gameSituation.IsWhiteMoving))
        {
            clientManager.Block(false, PlayerColor.Black);
        }
        else
        {
            clientManager.Block(true, PlayerColor.White);
            clientManager.Block(true, PlayerColor.Black);
        }
    }

    private void OnPlayerMoved(PlayerAct playerAct)
    {
        clientManager.Block(true, PlayerColor.White);
        clientManager.Block(true, PlayerColor.Black);
        PlayerActedEvent(playerAct);
    }
}
