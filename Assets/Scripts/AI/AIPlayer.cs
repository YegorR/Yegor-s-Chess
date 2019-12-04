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
    }
    public void SetGameSituation(GameSituation gameSituation)
    {
        uciAdapter.SetGameSituation(gameSituation);
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
        PlayerActedEvent(playerAct);
    }
}
