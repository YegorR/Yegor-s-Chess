using System.Collections.Generic;

public class GameSituation
{
    private (ChessPieceType, PlayerColor)[,] piecesLocation;
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
            return status;
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

public struct SerializedGameSituation
{
    //public int[,] piecesLocationChessType;
    public int[] piecesLocationChessType;
    //public int[,] piecesLocationColor;
    public int[] piecesLocationColor;
    public bool isWhiteMoving;
    public int status;
    //public int[,] allowedMovesKey;
    public int[] allowedMovesKeyVertical;
    public int[] allowedMovesKeyHorizontal;
    //public int[][,] allowedMovesValues;
    public int[] allowedMovesValuesVertical;
    public int[] allowedMovesValuesHorizontal;

    public int[] allowedMovesValuesBoards;

    static public SerializedGameSituation Serialize(GameSituation gameSituation)
    {
        SerializedGameSituation result = new SerializedGameSituation();

        result.piecesLocationChessType = new int[64];
        result.piecesLocationColor = new int[64];
        int k = 0;
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                //result.piecesLocationChessType[i, j] = (int)gameSituation.PiecesLocation[i, j].Item1;
                //result.piecesLocationColor[i, j] = (int)gameSituation.PiecesLocation[i, j].Item2;
                result.piecesLocationChessType[k] = (int)gameSituation.PiecesLocation[i, j].Item1;
                result.piecesLocationColor[k] = (int)gameSituation.PiecesLocation[i, j].Item2;
                k++;
            }
        }

        result.isWhiteMoving = gameSituation.IsWhiteMoving;
        result.status = (int)gameSituation.GameStatus;

        //result.allowedMovesKey = new int[gameSituation.AllowedMoves.Keys.Count, 2];
        //result.allowedMovesValues = new int[gameSituation.AllowedMoves.Values.Count][,];
        result.allowedMovesKeyVertical = new int[gameSituation.AllowedMoves.Keys.Count];
        result.allowedMovesKeyHorizontal = new int[gameSituation.AllowedMoves.Keys.Count];

        int valuesCount = 0;
        foreach(Cell key in gameSituation.AllowedMoves.Keys)
        {
            valuesCount += gameSituation.AllowedMoves[key].Count;
        }

        result.allowedMovesValuesVertical = new int[valuesCount];
        result.allowedMovesValuesHorizontal = new int[valuesCount];
        result.allowedMovesValuesBoards = new int[gameSituation.AllowedMoves.Keys.Count];

        k = 0;
        int valuesIter = 0;
        foreach (Cell cell in gameSituation.AllowedMoves.Keys)
        {
            result.allowedMovesKeyVertical[k] = cell.Vertical;
            result.allowedMovesKeyHorizontal[k] = cell.Horizontal;

            //result.allowedMovesValues[k] = new int[gameSituation.AllowedMoves[cell].Count, 2];
            //int j = 0;
            //foreach (Cell cellValue in gameSituation.AllowedMoves[cell])
            //{
            //    result.allowedMovesValues[k][j, 0] = cellValue.Vertical;
            //    result.allowedMovesValues[k][j, 1] = cellValue.Horizontal;
            //    j++;
            //}
            foreach(Cell cellValue in gameSituation.AllowedMoves[cell])
            {
                result.allowedMovesValuesVertical[valuesIter] = cellValue.Vertical;
                result.allowedMovesValuesHorizontal[valuesIter] = cellValue.Horizontal;
                valuesIter++;
            }
            result.allowedMovesValuesBoards[k] = valuesIter;
            k++;
        }

        return result;
    }

    static public GameSituation Deserealize(SerializedGameSituation serialized)
    {
        var piecesLocation = new (ChessPieceType, PlayerColor)[8, 8];

        int k = 0;
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                piecesLocation[i, j] = ((ChessPieceType)serialized.piecesLocationChessType[k], 
                    (PlayerColor)serialized.piecesLocationColor[k]);
                k++;
            }
        }
        bool isWhiteMoving = serialized.isWhiteMoving;
        GameStatus status = (GameStatus)serialized.status;

        Dictionary<Cell, ISet<Cell>> allowedMoves = new Dictionary<Cell, ISet<Cell>>();
        /*for(int i = 0; i < (serialized.allowedMovesKey.Length / 2); ++i)
        {
            int vertical = serialized.allowedMovesKey[i, 0];
            int horizontal = serialized.allowedMovesKey[i, 1];
            Cell cellKey = new Cell(vertical, horizontal);

            HashSet<Cell> set = new HashSet<Cell>();
            for (int j = 0; j < (serialized.allowedMovesValues[i].Length / 2); ++j)
            {
                vertical = serialized.allowedMovesValues[i][j, 0];
                horizontal = serialized.allowedMovesValues[i][j, 1];
                Cell cellValue = new Cell(vertical, horizontal);
                set.Add(cellValue);
            }
            allowedMoves.Add(cellKey, set);
        }*/
        k = 0;
        for (int i = 0; i < serialized.allowedMovesKeyVertical.Length; ++i)
        {
            Cell key = new Cell(serialized.allowedMovesKeyVertical[i], serialized.allowedMovesKeyHorizontal[i]);
            ISet<Cell> valueSet = new HashSet<Cell>();
            for(; k < serialized.allowedMovesValuesBoards[i]; ++k)
            {
                Cell cellValue = new Cell(serialized.allowedMovesValuesVertical[k], serialized.allowedMovesValuesHorizontal[k]);
                valueSet.Add(cellValue);
            }
            allowedMoves.Add(key, valueSet);
        }

        return new GameSituation(piecesLocation, isWhiteMoving, status, allowedMoves);
    }
}