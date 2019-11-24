using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckmateButtonController : GUIComponent
{
    [SerializeField] PlayerColor playerColor;
    public override void Start()
    {
        base.Start();
    }

    public void Click()
    {
        if (playerColor == PlayerColor.White)
        {
            clientManager.NotifyedCheckMateWhite();
        }
        else
        {
            clientManager.NotifyedCheckMateBlack();
        }
    }
}
