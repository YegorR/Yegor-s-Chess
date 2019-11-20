using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public GameObject boardPrefab;
    public GameObject TEMP_chessGamePrefab;

    private Board board;
    private ChessGame chessGame;

    void Start()
    {
        GameObject boardObject = Instantiate(boardPrefab);
        board = boardObject.GetComponent<Board>();
        board.MoveIsMadeEvent += MoveIsMade;

        GameObject chessObject = Instantiate(TEMP_chessGamePrefab);
        chessGame = chessObject.GetComponent<ChessGame>();

        board.InitializeBoard(chessGame.InitializeBoard());
    }

    private void MoveIsMade(Cell from, Cell to)
    {
        GameSituation gameSituation = chessGame.MakeMove(from, to);
        board.SetGameSituation(gameSituation);
    }

    public void changeGraphicMode(bool is3dMode)
    {
        board.GraphicMode = is3dMode;
        Camera.main.GetComponent<CameraScript>().Is3D = is3dMode;
    }
}
