using UnityEngine;
using UnityEngine.Networking;

public class NetPlayer : NetworkBehaviour, IPlayer
{

    private ClientManager clientManager;
    private PlayerColor playerColor;

    public event ActedHandler OnActEvent;

    public void SetGameSituation(GameSituation gameSituation)
    {
        RpcSetGameSituation(gameSituation);
    }

    [ClientRpc]
    private void RpcSetGameSituation(GameSituation gameSituation)
    {
        if (!isLocalPlayer) return;

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
        clientManager.SetGameSituation(gameSituation);
    }

    public void Initialize(PlayerColor playerColor)
    {
        this.playerColor = playerColor;
        clientManager = new ClientManager();
        clientManager.MoveIsMadeEvent += MoveIsMade;
    }

    private void MoveIsMade(Cell from, Cell to)
    {
        PlayerAct playerAct = new PlayerAct
        {
            Act = PlayerAct.ActType.Move,
            From = from,
            To = to
        };
        CmdAct(playerAct);
    }

    [Command]
    private void CmdAct(PlayerAct playerAct)
    {
        OnActEvent(playerAct);
    }
}
