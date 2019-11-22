using UnityEngine;
using System.Collections;

public delegate void ActedHandler(PlayerAct playerAct);
public interface IPlayer
{
    void SetGameSituation(GameSituation gameSituation);
    event ActedHandler OnActEvent;
}
