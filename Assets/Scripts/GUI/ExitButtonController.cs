using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButtonController : GUIComponent
{
    public override void Start()
    {
        base.Start();
    }

    public void Click()
    {
        clientManager.ExitButtonPressed();
    }
}
