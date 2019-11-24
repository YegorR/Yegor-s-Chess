using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentExitsController : GUIComponent
{
    public override void Start()
    {
        base.Start();
    }

    public void Click()
    {
        clientManager.OpponentsExitsNotified();
    }
}
