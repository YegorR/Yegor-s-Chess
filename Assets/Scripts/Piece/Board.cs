using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Board : MonoBehaviour
{
    public GameObject chessGameObject;
    private ChessGame chessGame;

    private Dictionary<Cell, GameObject> pieces = new Dictionary<Cell, GameObject>();

    public GameObject Piece3D;
    public GameObject Piece2D;

    private bool is3dMode = true;

    public bool GraphicMode
    {
        get
        {
            return is3dMode;
        }
        set
        {
            if (value == is3dMode)
            {
                return;
            }

            is3dMode = value;
            if (!is3dMode)
            {
                transform.Rotate(new Vector3(-90, 0, 0));
            }
            else
            {
                transform.Rotate(new Vector3(90, 0, 0));
            }
        }
    }

    void Start()
    {
        chessGame = chessGameObject.GetComponent<ChessGame>();
        GameSituation gameSituation = chessGame.InitializeBoard();
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                Cell pieceCell = new Cell(i, j);
                var piece = gameSituation.PiecesLocation[i, j];
                if (piece.Item1 == ChessPieceType.None)
                {
                    continue;
                }
                GameObject newPiece = CreatePiece(piece.Item1, piece.Item2, pieceCell);
                pieces.Add(pieceCell, newPiece);
                Piece pieceScript = newPiece.GetComponent<Piece>();
                if (gameSituation.AllowedMoves.ContainsKey(pieceCell))
                {
                    pieceScript.AllowedMoves = gameSituation.AllowedMoves[pieceCell];
                }
                else
                {
                    pieceScript.AllowedMoves = new HashSet<Cell>();
                }
                pieceScript.Block = (piece.Item2 == PlayerColor.Black);
            }
        }
        

    }

    private GameObject CreatePiece(ChessPieceType chessPieceType, PlayerColor playerColor, Cell cell)
    {
        GameObject pieceObject;
        if (is3dMode)
        {
            pieceObject = Instantiate(Piece3D);
        }
        else
        {
            pieceObject = Instantiate(Piece2D);
        }

        Piece piece = pieceObject.GetComponent<Piece>();

        piece.ChessPieceType = chessPieceType;
        piece.PlayerColor = playerColor;

       
        piece.Move(cell);
        piece.MoveIsMadeEvent += MoveIsMade;
        piece.ChessPieceType = chessPieceType;
        piece.PlayerColor = playerColor;
        return pieceObject;
    }

    void MoveIsMade(Cell from, Cell to)
    {
        GameSituation gameSituation = chessGame.MakeMove(from, to);
        PlayerColor myColor = gameSituation.IsWhiteMoving ? PlayerColor.White : PlayerColor.Black;

        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                Cell cell = new Cell(i, j);
                var piece = gameSituation.PiecesLocation[i, j];
                if (pieces.ContainsKey(cell))
                {
                    GameObject pieceObject = pieces[cell];
                    Piece pieceScriptObject = pieceObject.GetComponent<Piece>();
                    if ((pieceScriptObject.PlayerColor == piece.Item2) && (pieceScriptObject.ChessPieceType == piece.Item1))
                    {
                        pieceScriptObject.Block = (myColor != piece.Item2);
                        if (gameSituation.AllowedMoves.ContainsKey(cell))
                        {
                            pieceScriptObject.AllowedMoves = gameSituation.AllowedMoves[cell];
                        }
                        else
                        {
                            pieceScriptObject.AllowedMoves = new HashSet<Cell>();
                        }
                        continue;
                    }
                    else
                    {
                        pieces.Remove(cell);
                        Destroy(pieceObject);
                    }
                }
                if (!pieces.ContainsKey(cell))
                {
                    if (piece.Item1 == ChessPieceType.None)
                    {
                        continue;
                    }
                    GameObject pieceObject = CreatePiece(piece.Item1, piece.Item2, cell);
                    Piece pieceScriptObject = pieceObject.GetComponent<Piece>();
                    pieceScriptObject.Block = (myColor != piece.Item2);
                    if (gameSituation.AllowedMoves.ContainsKey(cell))
                    {
                        pieceScriptObject.AllowedMoves = gameSituation.AllowedMoves[cell];
                    } 
                    else
                    {
                        pieceScriptObject.AllowedMoves = new HashSet<Cell>();
                    }
                    
                    pieces.Add(cell, pieceObject);
                    continue;
                }
            }
        }
    }
}
