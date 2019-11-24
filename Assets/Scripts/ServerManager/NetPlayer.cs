using UnityEngine;
using UnityEngine.Networking;

public class NetPlayer : NetworkBehaviour, IPlayer
{
    [SerializeField] private GameObject clientManagerPrefab;
    private ClientManager clientManager;
    private PlayerColor playerColor;

    public event ActedHandler OnActEvent;

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
        clientManager.MoveIsMadeEvent += MoveIsMade;
    }

    private void MoveIsMade(Cell from, Cell to)
    {
        if (!isLocalPlayer) return;
        PlayerAct playerAct = new PlayerAct
        {
            Act = PlayerAct.ActType.Move,
            From = from,
            To = to
        };
        CmdAct(SerializedPlayerAct.Serialize(playerAct));
    }

    [Command]
    private void CmdAct(SerializedPlayerAct serializedPlayerAct)
    {
        PlayerAct playerAct = SerializedPlayerAct.Deserealize(serializedPlayerAct);
        OnActEvent(playerAct);
    }
}
