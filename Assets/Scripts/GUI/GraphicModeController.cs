using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicModeController : GUIComponent
{
    
    private bool is3DMode = true;

    public void Click()
    {
        is3DMode = !is3DMode;
        clientManager.ChangeGraphicMode(is3DMode);

        ChangeText();
    }

    public override void Start()
    {
        base.Start();
        ChangeText();
    }


    private void ChangeText()
    {
        Text text = GetComponentInChildren<Text>();
        if (is3DMode)
        {
            text.text = "3D";
        }
        else
        {
            text.text = "2D";
        }
    }
}
