using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicModeController : MonoBehaviour
{
    [SerializeField] private GameObject ClientManagerObject;
    private ClientManager clientManager;
    private bool is3DMode = true;

    public void Click()
    {
        is3DMode = !is3DMode;
        clientManager.ChangeGraphicMode(is3DMode);

        ChangeText();
    }

    void Start()
    {
        clientManager = ClientManagerObject.GetComponent<ClientManager>();
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
