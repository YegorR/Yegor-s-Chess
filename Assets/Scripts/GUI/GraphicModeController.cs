using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicModeController : MonoBehaviour
{
    public GameObject clientManagerObject;
    private ClientManager clientManager;
    private bool is3DMode = true;

    public void Click()
    {
        is3DMode = !is3DMode;
        clientManager.changeGraphicMode(is3DMode);
    }

    void Start()
    {
        clientManager = clientManagerObject.GetComponent<ClientManager>();
    }
}
