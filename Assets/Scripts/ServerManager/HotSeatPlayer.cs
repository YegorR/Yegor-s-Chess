using UnityEngine;
using System.Collections;

public class HotSeatPlayer : IPlayer
{
    private bool isActEventSet = false;
    public event PlayerActedEventHandler PlayerActedEvent
    {
        add
        {
            if(!isActEventSet)
            {
                m_ActEvent += value;
                isActEventSet = true;
            }
        }
        remove
        {
            m_ActEvent -= value;
            isActEventSet = false;
        }
    }

    private event PlayerActedEventHandler m_ActEvent;

    private ClientManager clientManager;
    private PlayerColor playerColor;

    public HotSeatPlayer()
    {
        this.playerColor = PlayerColor.White;
        clientManager = ClientManager.getInstance();
        clientManager.ActEvent += OnPlayerAct;
    }

    public void SetGameSituation(GameSituation gameSituation)
    {
        clientManager.SetGameSituation(gameSituation);
        clientManager.Block(!gameSituation.IsWhiteMoving, PlayerColor.White);
        clientManager.Block(gameSituation.IsWhiteMoving,PlayerColor.Black);   
    }

    private void OnPlayerAct(PlayerAct playerAct)
    {
        if (playerAct.Act == PlayerAct.ActType.Exit)
        {
            return;
        }
        m_ActEvent(playerAct);
    }
}
