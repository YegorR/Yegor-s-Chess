using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicModeController : MonoBehaviour
{
    public GameObject clientManagerObject;
    private ClientManager clientManager;
    private bool is3DMode = true;

    public Sprite image2D;
    public Sprite image3D;

    public void Click()
    {
        is3DMode = !is3DMode;
        clientManager.changeGraphicMode(is3DMode);

        changeSprite();
    }

    void Start()
    {
        clientManager = clientManagerObject.GetComponent<ClientManager>();
        changeSprite();
    }

    private void changeSprite()
    {
        Sprite currentSprite = is3DMode ? image3D : image2D;
        GetComponent<Image>().sprite = currentSprite;
    }
}
