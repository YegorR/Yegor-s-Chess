using UnityEngine;
using UnityEngine.Networking;

public class ServerManager
{
    private IPlayer whitePlayer, blackPlayer;
    private ChessGame chessGame;

    public ServerManager(IPlayer whitePlayer, IPlayer blackPlayer)
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
        else if (playerAct.Act == PlayerAct.ActType.Exit)
        {
            Exit(playerAct);
        }
    }

    private void Move(PlayerAct playerAct)
    {
        GameSituation gameSituation = chessGame.MakeMove(playerAct.From, playerAct.To);
        whitePlayer.SetGameSituation(gameSituation);
        blackPlayer.SetGameSituation(gameSituation);
    }

    private void Exit(PlayerAct playerAct)
    {
        IPlayer player;
        if (playerAct.PlayerColor == PlayerColor.White)
        {
            player = blackPlayer;
        }
        else
        {
            player = whitePlayer;
        }
        GameSituation gameSituation = new GameSituation
        {
            GameStatus = GameStatus.OpponentExits
        };
        player.SetGameSituation(gameSituation);
    }
}
