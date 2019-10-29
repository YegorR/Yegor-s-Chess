using System.Collections.Generic;

public class GameSituation
{
    private (ChessPieceType, PlayerColor)[,] piecesLocation;;
    private bool isWhiteMoving;
    private GameStatus status;
    private Dictionary<Cell, ISet<Cell>> allowedMoves;

    public GameSituation((ChessPieceType, PlayerColor)[,] piecesLocation, bool isWhiteMoving, GameStatus status, Dictionary<Cell, ISet<Cell>> allowedMoves)
    {
        this.piecesLocation = piecesLocation;
        this.isWhiteMoving = isWhiteMoving;
        this.status = status;
        this.allowedMoves = allowedMoves;
    }

    public (ChessPieceType, PlayerColor)[,] PiecesLocation
    {
        get
        {
            return piecesLocation;
        }
    }

    public bool IsWhiteMoving
    {
        get
        {
            return isWhiteMoving;
        }
    }

    public GameStatus GameStatus
    {
        get
        {
            return GameStatus;
        }
    }

    public Dictionary<Cell, ISet<Cell>> AllowedMoves
    {
        get
        {
            return allowedMoves;
        }
    }
}
