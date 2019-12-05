using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : IPlayer
{
    private const string RELATIVE_ENGINE_PATH = "/Engine/stockfish_10_x64.exe";
    private readonly UCIAdapter uciAdapter;
    private PlayerColor playerColor;

    public event PlayerActedEventHandler PlayerActedEvent;

    public AIPlayer(PlayerColor playerColor)
    {
        this.playerColor = playerColor;
        uciAdapter = new UCIAdapter(Application.dataPath + RELATIVE_ENGINE_PATH);
        uciAdapter.EngineMovedEvent += OnEngineMoved;
        uciAdapter.Start();
        while(!uciAdapter.IsReady)
        {
            
        }
        UCIThreadAdapter uciThreadAdapter = UCIThreadAdapter.GetInstance();
        uciThreadAdapter.playerActedEvent += OnPlayerAct;

    }
    public void SetGameSituation(GameSituation gameSituation)
    {
        if (((gameSituation.IsWhiteMoving) && (playerColor == PlayerColor.White)) || 
            ((!gameSituation.IsWhiteMoving) && (playerColor == PlayerColor.Black)))
        {
            uciAdapter.SetGameSituation(gameSituation);
        }
    }

    private void OnEngineMoved(Cell from, Cell to)
    {
        PlayerAct playerAct = new PlayerAct()
        {
            Act = PlayerAct.ActType.Move,
            From = from,
            To = to,
            PlayerColor = playerColor
        };
        UCIThreadAdapter uciThreadAdapter = UCIThreadAdapter.GetInstance();
        uciThreadAdapter.SetPlayerAct(playerAct);
    }

    private void OnPlayerAct(PlayerAct playerAct)
    {
        PlayerActedEvent(playerAct);
    }

}
