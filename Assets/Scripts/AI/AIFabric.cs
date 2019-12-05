using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFabric
{
    public AIFabric()
    {
        PlayerColor playerColor = SceneData.PlayerColor;
        AIPlayer aiPlayer = new AIPlayer(playerColor == PlayerColor.White ? PlayerColor.Black : PlayerColor.White);
        HumanPlayer humanPlayer = new HumanPlayer(playerColor);
       
        IPlayer whitePlayer; IPlayer blackPlayer;
        if(playerColor == PlayerColor.White)
        {
            whitePlayer = humanPlayer;
            blackPlayer = aiPlayer;
        }
        else
        {
            whitePlayer = aiPlayer;
            blackPlayer = humanPlayer;
        }

        ServerManager serverManager = new ServerManager(whitePlayer, blackPlayer);
    }
}
