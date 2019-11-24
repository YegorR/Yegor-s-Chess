using UnityEngine;
using UnityEngine.Networking;
public class ChessNetworkManager : NetworkManager
{
    public NetFactory NetFactory{get; set;}

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        NetFactory.SetPlayer(player);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (NetFactory.PlayersCount == 20 ){
            conn.Disconnect();
            return;
        }
        base.OnServerConnect(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        NetworkServer.DestroyPlayersForConnection(conn);
        GameObject.Find("ClientManager").GetComponent<ClientManager>().Disconnect();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        //base.OnClientDisconnect(conn);
        GameObject.Find("ClientManager").GetComponent<ClientManager>().Disconnect();
    }

    public void OnDisable()
    {
        StopServer();
        StopClient();
    }
}
