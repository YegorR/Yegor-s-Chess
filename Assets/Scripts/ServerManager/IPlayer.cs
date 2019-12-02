using UnityEngine;
using System.Collections;

public delegate void PlayerActedEventHandler(PlayerAct playerAct);
public interface IPlayer
{
    void SetGameSituation(GameSituation gameSituation);
    event PlayerActedEventHandler PlayerActedEvent;
}
