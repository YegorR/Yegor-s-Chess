using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : MonoBehaviour
{
    private IPlayer whitePlayer, blackPlayer;
    private ChessGame chessGame;

    public void Initialize(IPlayer whitePlayer, IPlayer blackPlayer)
    {
        this.whitePlayer = whitePlayer;
        this.blackPlayer = blackPlayer;
        whitePlayer.OnActEvent += SetPlayerAct;
        blackPlayer.OnActEvent += SetPlayerAct;

        chessGame = new ChessGame();
        GameSituation gameSituation = chessGame.InitializeBoard();
        whitePlayer.SetGameSituation(gameSituation);
        blackPlayer.SetGameSituation(gameSituation);
    }

    public void SetPlayerAct(PlayerAct playerAct)
    {
        if (playerAct.Act == PlayerAct.ActType.Move)
        {
            Move(playerAct);
        }
    }

    private void Move(PlayerAct playerAct)
    {
        GameSituation gameSituation = chessGame.MakeMove(playerAct.From, playerAct.To);
        whitePlayer.SetGameSituation(gameSituation);
        blackPlayer.SetGameSituation(gameSituation);
    }
}
