using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UCIThreadAdapter : MonoBehaviour
{
    public event PlayerActedEventHandler playerActedEvent;
    private static UCIThreadAdapter singleton;
    private PlayerAct playerAct;
    
    public static UCIThreadAdapter GetInstance()
    {
        return singleton;
    }

    void Awake()
    {
        singleton = this;
    }

    public void SetPlayerAct(PlayerAct playerAct)
    {
        this.playerAct = playerAct;
    }
    // Update is called once per frame
    void Update()
    {
        if (playerActedEvent != null)
        {
            if (this.playerAct != null)
            {
                PlayerAct playerAct = this.playerAct;
                this.playerAct = null;
                playerActedEvent(playerAct);
            }
        }
    }
}
