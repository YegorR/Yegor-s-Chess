using System.Collections.Generic;

public class GameSituation{
    public (ChessPieceType, PlayerColor)[,] PiecesLocation
    {
        set; get;
    }

    public bool IsWhiteMoving
    {
        set; get;
    }

    public GameStatus GameStatus
    {
        set; get;
    }

    public Dictionary<Cell, ISet<Cell>> AllowedMoves
    {
        set; get;
    }

    public bool isWhiteKingCastlingPossible
    {
        set; get;
    }

    public bool isWhiteQueenCastlingPossible
    {
        set; get;
    }

    public bool isBlackKingCastlingPossible
    {
        set; get;
    }

    public bool isBlackQueenCastlingPossible
    {
        set; get;
    }

    public Cell aislePawnCell
    {
        set; get;
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

    public bool isWhiteKingCastlingPossible;
    public bool isWhiteQueenCastlingPossible;
    public bool isBlackKingCastlingPossible;
    public bool isBlackQueenCastlingPossible;

    public int aislePawnVertical;
    public int aislePawnHorizontal;

    static public SerializedGameSituation Serialize(GameSituation gameSituation)
    {
        SerializedGameSituation result = new SerializedGameSituation();
        result.status = (int)gameSituation.GameStatus;
        if (gameSituation.GameStatus == GameStatus.OpponentExits)
        {
            return result;
        }

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

        result.isWhiteKingCastlingPossible = gameSituation.isWhiteKingCastlingPossible;
        result.isWhiteQueenCastlingPossible = gameSituation.isWhiteQueenCastlingPossible;
        result.isBlackKingCastlingPossible = gameSituation.isBlackKingCastlingPossible;
        result.isBlackQueenCastlingPossible = gameSituation.isBlackQueenCastlingPossible;

        if (gameSituation.aislePawnCell != null)
        {
            result.aislePawnVertical = gameSituation.aislePawnCell.Vertical;
            result.aislePawnHorizontal = gameSituation.aislePawnCell.Horizontal;

        }
        else
        {
            result.aislePawnHorizontal = -1;
            result.aislePawnVertical = -1;
        }
        

        return result;
    }

    static public GameSituation Deserealize(SerializedGameSituation serialized)
    {
        GameStatus status = (GameStatus)serialized.status;

        if (status == GameStatus.OpponentExits)
        {
            return new GameSituation()
            {
                GameStatus = status
            };
        }

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

        GameSituation gameSituation = new GameSituation()
        {
            PiecesLocation = piecesLocation,
            AllowedMoves = allowedMoves,
            IsWhiteMoving = isWhiteMoving,
            GameStatus = status,
            isWhiteKingCastlingPossible = serialized.isWhiteKingCastlingPossible,
            isWhiteQueenCastlingPossible = serialized.isWhiteQueenCastlingPossible,
            isBlackKingCastlingPossible = serialized.isBlackKingCastlingPossible,
            isBlackQueenCastlingPossible = serialized.isBlackQueenCastlingPossible
        };
        if (serialized.aislePawnVertical != -1)
        {
            gameSituation.aislePawnCell = new Cell(serialized.aislePawnVertical, serialized.aislePawnHorizontal);
        }
        return gameSituation;
    }
}