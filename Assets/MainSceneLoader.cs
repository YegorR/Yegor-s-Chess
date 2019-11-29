using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MainSceneLoader : MonoBehaviour
{
    void Start()
    {
        NetFactory netFactory;
        switch (SceneData.Type)
        {
            case SceneData.GameType.Host:
                netFactory = new NetFactory();
                netFactory.Initialize(true, SceneData.PlayerColor);
                break;
            case SceneData.GameType.Client:
                GameObject.Find("ChessNetworkManager").GetComponent<NetworkManager>().networkAddress = SceneData.Host;
                netFactory = new NetFactory();
                netFactory.Initialize(false, PlayerColor.None);
                break;
            case SceneData.GameType.HotSeat:
                HotSeatFactory hotSeatFactory = new HotSeatFactory();
                break;
        }
    }
}
