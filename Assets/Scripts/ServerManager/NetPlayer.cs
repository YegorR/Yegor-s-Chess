using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetPlayer : NetworkBehaviour, IPlayer
{
    private ClientManager clientManager;
    private PlayerColor playerColor;

    public event PlayerActedEventHandler PlayerActedEvent;

    public void SetGameSituation(GameSituation gameSituation)
    {
        RpcSetGameSituation(SerializedGameSituation.Serialize(gameSituation));
    }

    [ClientRpc]
    private void RpcSetGameSituation(SerializedGameSituation serializedGameSituation)
    {
        if (!isLocalPlayer) return;
        GameSituation gameSituation = SerializedGameSituation.Deserealize(serializedGameSituation);
        clientManager.SetGameSituation(gameSituation);

        if (gameSituation.GameStatus == GameStatus.OpponentExits)
        {
            clientManager.Block(true, PlayerColor.White);
            clientManager.Block(true, PlayerColor.Black);
            return;
        }    
        if ((playerColor == PlayerColor.White) && (gameSituation.IsWhiteMoving)) {
            clientManager.Block(false, PlayerColor.White);
        }
        else if ((playerColor == PlayerColor.Black) && (!gameSituation.IsWhiteMoving))
        {
            clientManager.Block(false, PlayerColor.Black);
        }
        else
        {
            clientManager.Block(true, playerColor);
        }
    }

    [ClientRpc]
    public void RpcInitialize(int playerColorInt)
    {
        if (!isLocalPlayer) return;
        PlayerColor playerColor = (PlayerColor)playerColorInt;
        this.playerColor = playerColor;
        clientManager = GameObject.Find("ClientManager").GetComponent<ClientManager>();
        clientManager.ActEvent += PlayerActed;
    }

    private void PlayerActed(PlayerAct playerAct)
    {
        playerAct.PlayerColor = playerColor;
        CmdAct(SerializedPlayerAct.Serialize(playerAct));
    }

    [Command]
    private void CmdAct(SerializedPlayerAct serializedPlayerAct)
    {
        PlayerAct playerAct = SerializedPlayerAct.Deserealize(serializedPlayerAct);
        PlayerActedEvent(playerAct);
    }
}
