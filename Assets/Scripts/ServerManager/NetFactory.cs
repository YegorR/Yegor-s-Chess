using UnityEngine;
using System.Collections;

public class NetFactory
{
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
        GameObject networkManagerObject = GameObject.Find("ChessNetworkManager");
        networkManager = networkManagerObject.GetComponent<ChessNetworkManager>();

        if (!isHost)
        {
            networkManager.StartClient();
            return;
        }
        if (playerColor == PlayerColor.None)
        {
            System.Random rand = new System.Random();
            int r = rand.Next(2);
            if (r == 1)
            {
                playerColor = PlayerColor.White;
            }
            else
            {
                playerColor = PlayerColor.Black;
            }
        }
        this.playerColor = playerColor;
        networkManager.NetFactory = this;
        networkManager.StartHost();
    }

    public void SetPlayer(GameObject player)
    {
        NetPlayer netPlayer = player.GetComponent<NetPlayer>();
        if (playersCount == 0)
        {
            netPlayer.RpcInitialize((int)playerColor);
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
            netPlayer.RpcInitialize((int)(playerColor == PlayerColor.White ? PlayerColor.Black : PlayerColor.White));
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
            serverManager = new ServerManager();
            serverManager.Initialize(whitePlayerObject.GetComponent<NetPlayer>(), blackPlayerObject.GetComponent<NetPlayer>());
        }
    }
}
