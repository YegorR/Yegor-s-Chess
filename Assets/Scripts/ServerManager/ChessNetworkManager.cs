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
}
