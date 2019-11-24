using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GUIComponent : MonoBehaviour
{
    [SerializeField] private GameObject ClientManagerObject;
    protected ClientManager clientManager;
    public virtual void Start()
    {
        clientManager = ClientManagerObject.GetComponent<ClientManager>();
    }
}
