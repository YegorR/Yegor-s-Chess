using UnityEngine;
using System.Collections;

public class NetFactory : MonoBehaviour
{
    [SerializeField] private GameObject serverManagerPrefab;
    private ServerManager serverManager;

    private ChessNetworkManager networkManager;
    private bool isHost;
    private PlayerColor playerColor;

    private int playersCount;
    public int PlayersCount { get; }

    private GameObject whitePlayerObject, blackPlayerObject;

    public void Initialize(bool isHost, PlayerColor playerColor)
    {
        this.isHost = isHost;
        this.playerColor = playerColor;
        networkManager = GameObject.Find("ChessNetworkManager").GetComponent<ChessNetworkManager>();

        if (!isHost)
        {
            networkManager.StartClient();
            return;
        }
        networkManager.NetFactory = this;
        networkManager.StartHost();
    }

    public void SetPlayer(GameObject player)
    {
        NetPlayer netPlayer = player.GetComponent<NetPlayer>();
        if (playersCount == 0)
        {
            netPlayer.Initialize(playerColor);
            if (playerColor == PlayerColor.White)
            {
                whitePlayerObject = player;
            }
            else
            {
                blackPlayerObject = player;
            }
        }
        else if (playersCount == 1)
        {
            netPlayer.Initialize(playerColor == PlayerColor.White ? PlayerColor.Black : PlayerColor.White);
            if (playerColor == PlayerColor.White)
            {
                blackPlayerObject = player;
            }
            else
            {
                whitePlayerObject = player;
            }
        }

        
        playersCount++;
        if (playersCount == 2)
        {
            serverManager = Instantiate(serverManagerPrefab).GetComponent<ServerManager>();
            serverManager.Initialize(whitePlayerObject.GetComponent<NetPlayer>(), blackPlayerObject.GetComponent<NetPlayer>());
        }
    }
}
