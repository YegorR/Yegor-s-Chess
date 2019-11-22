using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager
{
    public GameObject boardPrefab;
    public event MoveIsMadeDelegate MoveIsMadeEvent;

    private Board board;

    public ClientManager()
    {
        GameObject boardObject = GameObject.Instantiate(boardPrefab);
        board = boardObject.GetComponent<Board>();
        board.MoveIsMadeEvent += MoveIsMade;
    }

    private void MoveIsMade(Cell from, Cell to)
    {
        MoveIsMadeEvent(from, to);
    }

    public void ChangeGraphicMode(bool is3dMode)
    {
        board.GraphicMode = is3dMode;
        Camera.main.GetComponent<CameraScript>().Is3D = is3dMode;
    }

    public void Block(bool isBlock, PlayerColor playerColor)
    {
        board.Block(isBlock, playerColor);
    }

    public void SetGameSituation(GameSituation gameSituation)
    {
        board.SetGameSituation(gameSituation);
    }
}