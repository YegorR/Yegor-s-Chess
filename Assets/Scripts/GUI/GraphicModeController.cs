using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicModeController : MonoBehaviour
{
    public GameObject clientManagerObject;
    private ClientManager clientManager;
    private bool is3DMode = true;

    public void Click()
    {
        is3DMode = !is3DMode;
        clientManager.changeGraphicMode(is3DMode);

        ChangeText();
    }

    void Start()
    {
        clientManager = clientManagerObject.GetComponent<ClientManager>();
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
