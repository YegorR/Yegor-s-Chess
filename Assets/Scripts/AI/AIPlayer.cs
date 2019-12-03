using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : IPlayer
{
    public event PlayerActedEventHandler PlayerActedEvent;

    public void SetGameSituation(GameSituation gameSituation)
    {
        throw new System.NotImplementedException();
    }
}
