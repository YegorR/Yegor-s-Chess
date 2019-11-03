using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Board : MonoBehaviour
{
    public GameObject chessGameObject;
    private ChessGame chessGame;


    private Dictionary<Cell, GameObject> pieces = new Dictionary<Cell, GameObject>();


    public GameObject Pawn3D;
    public GameObject Bishop3D;
    public GameObject Knight3D;
    public GameObject Rook3D;
    public GameObject Queen3D;
    public GameObject King3D;

    public Material whiteMaterial;
    public Material blackMaterial;

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
                Piece pieceScript = newPiece.GetComponent<Piece3D>();
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
        GameObject piece;
        switch (chessPieceType)
        {
            case ChessPieceType.Pawn:
                piece = Instantiate(Pawn3D);
                break;
            case ChessPieceType.Rook:
                piece = Instantiate(Rook3D);
                break;
            case ChessPieceType.Knight:
                piece = Instantiate(Knight3D);
                break;
            case ChessPieceType.Bishop:
                piece = Instantiate(Bishop3D);
                break;
            case ChessPieceType.Queen:
                piece = Instantiate(Queen3D);
                break;
            case ChessPieceType.King:
                piece = Instantiate(King3D);
                break;
            default:
                throw new System.Exception("Wrong piece");
        }

        Renderer renderer = piece.GetComponent<Renderer>();
        if (playerColor == PlayerColor.White)
        {
            renderer.material = whiteMaterial;
        }
        else
        {
            renderer.material = blackMaterial;
        }
        Piece script = piece.GetComponent<Piece3D>();
        script.Move(cell);
        script.MoveIsMadeEvent += MoveIsMade;
        script.ChessPieceType = chessPieceType;
        script.PlayerColor = playerColor;
        return piece;
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
                    Piece pieceScriptObject = pieceObject.GetComponent<Piece3D>();
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
                    Piece pieceScriptObject = pieceObject.GetComponent<Piece3D>();
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

    // Update is called once per frame
    void Update()
    {

    }
}
