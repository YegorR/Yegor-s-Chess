using System;
using System.Collections.Generic;

public class ChessGame
{
    private ChessSituation situation = new ChessSituation();
      

    public GameSituation MakeMove(Cell from, Cell to)
    {
        return situation.makeMove(from, to);
    }

    public GameSituation InitializeBoard()
    {
        return situation.InitializeBoard();
    }
}

class ChessSituation : ICloneable
{
    private static (ChessPieceType, PlayerColor) VOID_CELL = (ChessPieceType.None, PlayerColor.None);
    internal (ChessPieceType, PlayerColor)[,] piecesLocation;
    internal bool isWhiteMoving = true;
    internal Dictionary<Cell, ISet<Cell>> allowedMoves;
    internal Dictionary<ChessPieceType, int> eatenBlackPieces = new Dictionary<ChessPieceType, int>();
    internal Dictionary<ChessPieceType, int> eatenWhitePieces = new Dictionary<ChessPieceType, int>();


    //Контроль легальности рокировки
    internal bool isWhiteQueenRookMoved = false;
    internal bool isWhiteKingRookMoved = false;

    internal bool isBlackQueenRookMoved = false;
    internal bool isBlackKingRookMoved = false;

    internal bool isWhiteKingMoved = false;
    internal bool isBlackKingMoved = false;

    //Контроль взятия на проходе
    internal Cell aislePawnCell = null;

    internal ChessSituation()
    {
        InitiazeEatenPieces();
    }

    private void InitiazeEatenPieces()
    {
        eatenBlackPieces[ChessPieceType.Pawn] = 0;
        eatenBlackPieces[ChessPieceType.Rook] = 0;
        eatenBlackPieces[ChessPieceType.Knight] = 0;
        eatenBlackPieces[ChessPieceType.Bishop] = 0;
        eatenBlackPieces[ChessPieceType.Queen] = 0;
        eatenBlackPieces[ChessPieceType.King] = 0;
        eatenBlackPieces[ChessPieceType.None] = 0;

        eatenWhitePieces[ChessPieceType.Pawn] = 0;
        eatenWhitePieces[ChessPieceType.Rook] = 0;
        eatenWhitePieces[ChessPieceType.Knight] = 0;
        eatenWhitePieces[ChessPieceType.Bishop] = 0;
        eatenWhitePieces[ChessPieceType.Queen] = 0;
        eatenWhitePieces[ChessPieceType.King] = 0;
        eatenWhitePieces[ChessPieceType.None] = 0;
    }

    internal GameSituation InitializeBoard()
    {
        piecesLocation = new (ChessPieceType, PlayerColor)[8, 8];
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                piecesLocation[i, j] = (ChessPieceType.None, PlayerColor.None);
            }
        }

        for (int i = 0; i < 8; ++i)
        {
            piecesLocation[i, 1] = (ChessPieceType.Pawn, PlayerColor.White);
            piecesLocation[i, 6] = (ChessPieceType.Pawn, PlayerColor.Black);
        }

        piecesLocation[0, 0] = (ChessPieceType.Rook, PlayerColor.White);
        piecesLocation[7, 0] = (ChessPieceType.Rook, PlayerColor.White);
        piecesLocation[0, 7] = (ChessPieceType.Rook, PlayerColor.Black);
        piecesLocation[7, 7] = (ChessPieceType.Rook, PlayerColor.Black);

        piecesLocation[1, 0] = (ChessPieceType.Knight, PlayerColor.White);
        piecesLocation[6, 0] = (ChessPieceType.Knight, PlayerColor.White);
        piecesLocation[1, 7] = (ChessPieceType.Knight, PlayerColor.Black);
        piecesLocation[6, 7] = (ChessPieceType.Knight, PlayerColor.Black);

        piecesLocation[2, 0] = (ChessPieceType.Bishop, PlayerColor.White);
        piecesLocation[5, 0] = (ChessPieceType.Bishop, PlayerColor.White);
        piecesLocation[2, 7] = (ChessPieceType.Bishop, PlayerColor.Black);
        piecesLocation[5, 7] = (ChessPieceType.Bishop, PlayerColor.Black);

        piecesLocation[3, 0] = (ChessPieceType.Queen, PlayerColor.White);
        piecesLocation[4, 0] = (ChessPieceType.King, PlayerColor.White);
        piecesLocation[3, 7] = (ChessPieceType.Queen, PlayerColor.Black);
        piecesLocation[4, 7] = (ChessPieceType.King, PlayerColor.Black);

        ISet<Cell> attackedPosition = AttackedPosition();
        CalcAllowedMoves(attackedPosition);

        return new GameSituation()
        {
            PiecesLocation = piecesLocation,
            IsWhiteMoving = isWhiteMoving,
            GameStatus = GameStatus.Normal,
            AllowedMoves = allowedMoves
        };
    }


    internal GameSituation makeMove(Cell from, Cell to)
    {
        //1 Проверить, что сделанный ход действительно был правильным, иначе кинуть исключение
        if ((!allowedMoves.ContainsKey(from)) || ((!allowedMoves[from].Contains(to))))
        {
            throw new System.Exception("Запрещённый ход!");
        }

        //2 Сделать собственно ход
        DoMove(from, to);
        isWhiteMoving = !isWhiteMoving;

        //3 Проверить ситуацию на шах
        ISet<Cell> attackedPosition = AttackedPosition();
        bool check = attackedPosition.Contains(FindKing());

        //4 Проверить возможные ходы
        CalcAllowedMoves(attackedPosition);

        //5 Сделать вывод о статусе игры
        GameStatus gameStatus;
        if (check)
        {
            if (allowedMoves.Count == 0)
            {
                gameStatus = GameStatus.Checkmate;
            } else
            {
                gameStatus = GameStatus.Check;
            }
        } else
        {
            if (allowedMoves.Count == 0)
            {
                gameStatus = GameStatus.Draw;
            } else
            {
                gameStatus = GameStatus.Normal;
            }
        }

        return new GameSituation()
        {
            PiecesLocation = piecesLocation,
            IsWhiteMoving = isWhiteMoving,
            GameStatus = gameStatus,
            AllowedMoves = allowedMoves
        };
    }

    internal void DoMove(Cell from, Cell to)
    {
        ChessPieceType piece = piecesLocation[from.Vertical, from.Horizontal].Item1;
        if (piece == ChessPieceType.None)
        {
            throw new System.Exception("Ошибка в doMove - piece=None");
        }

        // Рокировка
        if (piece == ChessPieceType.King)
        {
            if (isWhiteMoving)
            {
                if (!isWhiteKingMoved)
                {
                    isWhiteKingMoved = true;
                    if (to.Vertical == 2)
                    {
                        piecesLocation[0, 0] = VOID_CELL;
                        piecesLocation[4, 0] = VOID_CELL;
                        piecesLocation[2, 0] = (ChessPieceType.King, PlayerColor.White);
                        piecesLocation[3, 0] = (ChessPieceType.Rook, PlayerColor.White);
                        isWhiteQueenRookMoved = true;
                        return;
                    }
                    if (to.Vertical == 6)
                    {
                        piecesLocation[7, 0] = VOID_CELL;
                        piecesLocation[4, 0] = VOID_CELL;
                        piecesLocation[6, 0] = (ChessPieceType.King, PlayerColor.White);
                        piecesLocation[5, 0] = (ChessPieceType.Rook, PlayerColor.White);
                        isWhiteKingRookMoved = true;
                        return;
                    }
                }
            }
            else
            {
                if (!isBlackKingMoved)
                {
                    isBlackKingMoved = true;
                    if (to.Vertical == 2)
                    {
                        piecesLocation[0, 7] = VOID_CELL;
                        piecesLocation[4, 7] = VOID_CELL;
                        piecesLocation[2, 7] = (ChessPieceType.King, PlayerColor.Black);
                        piecesLocation[3, 7] = (ChessPieceType.Rook, PlayerColor.Black);
                        isBlackQueenRookMoved = true;
                        return;
                    }
                    if (to.Vertical == 6)
                    {
                        piecesLocation[7, 7] = VOID_CELL;
                        piecesLocation[4, 7] = VOID_CELL;
                        piecesLocation[6, 7] = (ChessPieceType.King, PlayerColor.Black);
                        piecesLocation[5, 7] = (ChessPieceType.Rook, PlayerColor.Black);
                        isBlackKingRookMoved = true;
                        return;
                    }
                }
            }
        }

        if (piece == ChessPieceType.Rook)
        {
            if (isWhiteMoving)
            {
                if (from.Equals(new Cell(0, 0)))
                {
                    isWhiteQueenRookMoved = true;
                }
                else if (from.Equals(new Cell(7, 0)))
                {
                    isWhiteKingRookMoved = true;
                }
            }
            else
            {
                if (from.Equals(new Cell(0, 7)))
                {
                    isBlackQueenRookMoved = true;
                }
                else if (from.Equals(new Cell(7, 7)))
                {
                    isBlackKingRookMoved = true;
                }
            }
        }

        // Взятие на проходе
        if (piece == ChessPieceType.Pawn)
        {
            if ((aislePawnCell != null) && (aislePawnCell.Equals(to)))
            {
                piecesLocation[from.Vertical, from.Horizontal] = VOID_CELL;
                if (isWhiteMoving)
                {
                    piecesLocation[to.Vertical, to.Horizontal] = (ChessPieceType.Pawn, PlayerColor.White);
                    piecesLocation[to.Vertical, to.Horizontal - 1] = VOID_CELL;
                    eatenBlackPieces[ChessPieceType.Pawn] += 1;
                }
                else
                {
                    piecesLocation[to.Vertical, to.Horizontal] = (ChessPieceType.Pawn, PlayerColor.Black);
                    piecesLocation[to.Vertical, to.Horizontal + 1] = VOID_CELL;
                    eatenWhitePieces[ChessPieceType.Pawn] += 1;
                }
                return;
            }
            if (aislePawnCell != null)
            {
                aislePawnCell = null;
            }
            if (Math.Abs(from.Horizontal - to.Horizontal) == 2)
            {
                aislePawnCell = isWhiteMoving ? new Cell(from.Vertical, from.Horizontal + 1) : new Cell(from.Vertical, from.Horizontal - 1);
            }
        }
        else
        {
            aislePawnCell = null;
        }
 
        piecesLocation[from.Vertical, from.Horizontal] = VOID_CELL;
        if (isWhiteMoving)
        {
            eatenBlackPieces[piecesLocation[to.Vertical, to.Horizontal].Item1] += 1;
            piecesLocation[to.Vertical, to.Horizontal] = (piece, PlayerColor.White);
        }
        else
        {
            eatenWhitePieces[piecesLocation[to.Vertical, to.Horizontal].Item1] += 1;
            piecesLocation[to.Vertical, to.Horizontal] = (piece, PlayerColor.Black);
        }
    }

    private Cell FindKing()
    {
        PlayerColor myColor = isWhiteMoving ? PlayerColor.White : PlayerColor.Black;
    
        int vKing = -1;
        int hKing = -1;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var piece = piecesLocation[i, j];
                if (piece == (ChessPieceType.King, myColor))
                {
                    vKing = i;
                    hKing = j;
                    break;
                }
            }
            if (vKing >= 0)
            {
                break;
            }
        }

        if (vKing < 0)
        {
            throw new System.Exception("Отсутсвует король на поле");
        }
        return new Cell(vKing, hKing);
    }

    internal ISet<Cell> AttackedPosition()
    {
        PlayerColor foeColor = isWhiteMoving ? PlayerColor.Black : PlayerColor.White;

        isWhiteMoving = !isWhiteMoving;
        ISet<Cell> attackedCells = new HashSet<Cell>();

        for(int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                if (piecesLocation[i, j].Item2 == foeColor)
                {
                    Cell pos = new Cell(i, j);
                    switch(piecesLocation[i, j].Item1)
                    {
                        case ChessPieceType.Rook:
                            attackedCells.UnionWith(CalcRookMoves(pos));
                            break;
                        case ChessPieceType.Bishop:
                            attackedCells.UnionWith(CalcBishopMoves(pos));
                            break;
                        case ChessPieceType.Queen:
                            attackedCells.UnionWith(CalcQueenMoves(pos));
                            break;
                        case ChessPieceType.Knight:
                            attackedCells.UnionWith(CalcKnightMoves(pos));
                            break;
                        case ChessPieceType.Pawn:
                            attackedCells.UnionWith(CalcPawnMoves(pos));
                            break;
                        case ChessPieceType.King:
                            attackedCells.UnionWith(CalcKingMoves(pos));
                            break;
                        default:
                            throw new System.Exception("Неопределённая фигура");
                    }
                }
            }
        }
        isWhiteMoving = !isWhiteMoving;

        return attackedCells;
    }

    private ISet<Cell> CalcRookMoves(Cell pos)
    {
        ISet<Cell> moves = new HashSet<Cell>();
        for (int ver = pos.Vertical + 1; ver < 8; ver++)
        {
            if (piecesLocation[ver, pos.Horizontal] == VOID_CELL)
            {
                moves.Add(new Cell(ver, pos.Horizontal));
                continue;
            }
            if (piecesLocation[ver, pos.Horizontal].Item2 == (isWhiteMoving? PlayerColor.Black : PlayerColor.White))
            {
                moves.Add(new Cell(ver, pos.Horizontal));
                break;
            }
            else
            {
                break;
            }
        }
        for (int ver = pos.Vertical - 1; ver >= 0; ver--)
        {
            if (piecesLocation[ver, pos.Horizontal] == VOID_CELL)
            {
                moves.Add(new Cell(ver, pos.Horizontal));
                continue;
            }
            if (piecesLocation[ver, pos.Horizontal].Item2 == (isWhiteMoving ? PlayerColor.Black : PlayerColor.White))
            {
                moves.Add(new Cell(ver, pos.Horizontal));
                break;
            }
            else
            {
                break;
            }
        }
        for (int hor = pos.Horizontal + 1; hor < 8; hor++)
        {
            if (piecesLocation[pos.Vertical, hor] == VOID_CELL)
            {
                moves.Add(new Cell(pos.Vertical, hor));
                continue;
            }
            else if (piecesLocation[pos.Vertical, hor].Item2 == (isWhiteMoving ? PlayerColor.Black : PlayerColor.White))
            {
                moves.Add(new Cell(pos.Vertical, hor));
                break;
            }
            else
            {
                break;
            }
        }
        for (int hor = pos.Horizontal - 1; hor >= 0; hor--)
        {
            if (piecesLocation[pos.Vertical, hor] == VOID_CELL)
            {
                moves.Add(new Cell(pos.Vertical, hor));
                continue;
            }
            else if (piecesLocation[pos.Vertical, hor].Item2 == (isWhiteMoving ? PlayerColor.Black : PlayerColor.White))
            {
                moves.Add(new Cell(pos.Vertical, hor));
                break;
            }
            else
            {
                break;
            }
        }
        return moves;
    }

    private ISet<Cell> CalcBishopMoves(Cell pos)
    {
        ISet<Cell> moves = new HashSet<Cell>();
        PlayerColor foeColor = isWhiteMoving ? PlayerColor.Black : PlayerColor.White;

        for (int ver = pos.Vertical + 1, hor = pos.Horizontal + 1; (ver < 8) && (hor < 8); ver++, hor++)
        {
            if (piecesLocation[ver, hor] == VOID_CELL)
            {
                moves.Add(new Cell(ver, hor));
                continue;
            }
            else if (piecesLocation[ver, hor].Item2 == foeColor)
            {
                moves.Add(new Cell(ver, hor));
                break;
            }
            else
            {
                break;
            }
        }

        for (int ver = pos.Vertical - 1, hor = pos.Horizontal - 1; (ver >= 0) && (hor >= 0); ver--, hor--)
        {
            if (piecesLocation[ver, hor] == VOID_CELL)
            {
                moves.Add(new Cell(ver, hor));
                continue;
            }
            else if (piecesLocation[ver, hor].Item2 == foeColor)
            {
                moves.Add(new Cell(ver, hor));
                break;
            }
            else
            {
                break;
            }
        }

        for (int ver = pos.Vertical + 1, hor = pos.Horizontal - 1; (ver < 8) && (hor >= 0); ver++, hor--)
        {
            if (piecesLocation[ver, hor] == VOID_CELL)
            {
                moves.Add(new Cell(ver, hor));
                continue;
            }
            else if (piecesLocation[ver, hor].Item2 == foeColor)
            {
                moves.Add(new Cell(ver, hor));
                break;
            }
            else {
                break;
            }
        }

        for (int ver = pos.Vertical - 1, hor = pos.Horizontal + 1; (ver >= 0) && (hor < 8); ver--, hor++)
        {
            if (piecesLocation[ver, hor] == VOID_CELL)
            {
                moves.Add(new Cell(ver, hor));
                continue;
            }
            else if (piecesLocation[ver, hor].Item2 == foeColor)
            {
                moves.Add(new Cell(ver, hor));
                break;
            }
            else
            {
                break;
            }
        }

        return moves;
    }

    private ISet<Cell> CalcQueenMoves(Cell pos)
    {
        ISet<Cell> moves = CalcRookMoves(pos);
        moves.UnionWith(CalcBishopMoves(pos));
        return moves;
    }

    private ISet<Cell> CalcKnightMoves(Cell pos)
    {
        ISet<Cell> moves = new HashSet<Cell>();

        void addCell(int ver, int hor)
        {
            if ((ver >= 8) || (ver < 0) || (hor >= 8) || (hor < 0))
            {
                return;
            }
            if ((piecesLocation[ver, hor] == VOID_CELL) || (piecesLocation[ver, hor].Item2 == (isWhiteMoving ? PlayerColor.Black : PlayerColor.White)))
            {
                moves.Add(new Cell(ver, hor));
            }
        }

        addCell(pos.Vertical + 1, pos.Horizontal + 2);
        addCell(pos.Vertical + 1, pos.Horizontal - 2);
        addCell(pos.Vertical - 1, pos.Horizontal + 2);
        addCell(pos.Vertical - 1, pos.Horizontal - 2);
        addCell(pos.Vertical + 2, pos.Horizontal + 1);
        addCell(pos.Vertical + 2, pos.Horizontal - 1);
        addCell(pos.Vertical - 2, pos.Horizontal + 1);
        addCell(pos.Vertical - 2, pos.Horizontal - 1);

        return moves;
    }

    private ISet<Cell> CalcPawnMoves(Cell pos)
    {
        ISet<Cell> moves = new HashSet<Cell>();
        int forward = isWhiteMoving ? 1 : -1;
        
        if ((pos.Horizontal == 7)  || (pos.Horizontal == 0))
        {
            throw new System.Exception("Пешка на последней горизонтали");
        }

        if (piecesLocation[pos.Vertical, pos.Horizontal + forward] == VOID_CELL)
        {
            moves.Add(new Cell(pos.Vertical, pos.Horizontal + forward));

            if (pos.Horizontal == (isWhiteMoving ? 6 : 1)) {
                //TODO: обработка превращения пешки
            }
            if ((pos.Horizontal == (isWhiteMoving ? 1 : 6)) && (piecesLocation[pos.Vertical, pos.Horizontal + 2 * forward] == VOID_CELL))
            {
                moves.Add(new Cell(pos.Vertical, pos.Horizontal + 2 * forward));
            }
        }

        if (pos.Vertical != 0)
        {
            if (piecesLocation[pos.Vertical - 1, pos.Horizontal + forward].Item2 == (isWhiteMoving? PlayerColor.Black : PlayerColor.White))
            {
                moves.Add(new Cell(pos.Vertical - 1, pos.Horizontal + forward));
            }
        }

        if (pos.Vertical != 7)
        {
            if (piecesLocation[pos.Vertical + 1, pos.Horizontal + forward].Item2 == (isWhiteMoving ? PlayerColor.Black : PlayerColor.White))
            {
                moves.Add(new Cell(pos.Vertical + 1, pos.Horizontal + forward));
            }
        }

        if (aislePawnCell == null)
        {
            return moves;
        }

        if ((pos.Vertical == aislePawnCell.Vertical - 1) || (pos.Vertical == aislePawnCell.Vertical + 1)) {
            if (pos.Horizontal == (isWhiteMoving? 4 : 3))
            {
                moves.Add(new Cell(aislePawnCell.Vertical, aislePawnCell.Horizontal));
            }
        }
        return moves;
    }

    private ISet<Cell> CalcKingMoves(Cell pos)
    {
        ISet<Cell> moves = new HashSet<Cell>();
        
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                if ((pos.Vertical + i < 0) || (pos.Vertical + i >= 8))
                {
                    continue;
                }
                if ((pos.Horizontal + j < 0) || (pos.Horizontal + j >= 8))
                {
                    continue;
                }
                if ((i == 0) && (j == 0))
                {
                    continue;
                }
                if ((piecesLocation[pos.Vertical + i, pos.Horizontal + j] == VOID_CELL) ||
                    (piecesLocation[pos.Vertical + i, pos.Horizontal + j].Item2 == (isWhiteMoving ? PlayerColor.Black : PlayerColor.White)))
                {
                    moves.Add(new Cell(pos.Vertical + i, pos.Horizontal + j));
                }
            }
        }
        return moves;
    }

    private ISet<Cell> CalcCastling(ISet<Cell> attackCells)
    {
        ISet<Cell> moves = new HashSet<Cell>();

        if (isWhiteMoving? isWhiteKingMoved : isBlackKingMoved)
        {
            return moves;
        }

        int hor = isWhiteMoving ? 0 : 7;
        bool isKingRookMoved = isWhiteMoving ? isWhiteKingRookMoved : isBlackKingRookMoved;
        bool isQueenRookMoved = isWhiteMoving ? isWhiteQueenRookMoved : isBlackQueenRookMoved;
        
        if (!isQueenRookMoved)
        {
            bool allowed = true;
            for (int i = 4; i >= 2; i--)
            {
                if (attackCells.Contains(new Cell(i, hor)))
                {
                    allowed = false;
                    break;
                }
            }
            if (allowed)
            {
                for (int i = 3; i >= 1; i--)
                {
                    if (piecesLocation[i, hor] != VOID_CELL)
                    {
                        allowed = false;
                        break;
                    }
                }
            }
            if (allowed)
            {
                moves.Add(new Cell(2, hor));
            }
        }
        if (!isKingRookMoved)
        {
            bool allowed = true;
            for (int i = 4; i <= 6; i++)
            {
                if (attackCells.Contains(new Cell(i, hor)))
                {
                    allowed = false;
                    break;
                }
            }
            if (allowed)
            {
                for (int i = 5; i <= 6; i++)
                {
                    if (piecesLocation[i, hor] != VOID_CELL)
                    {
                        allowed = false;
                        break;
                    }
                }
            }
            if (allowed)
            {
                moves.Add(new Cell(6, hor));
            }
        }

        return moves;
    }

    private void CalcAllowedMoves(ISet<Cell> attackedPosition)
    {
        PlayerColor myColor = isWhiteMoving ? PlayerColor.White : PlayerColor.Black;
        allowedMoves = new Dictionary<Cell, ISet<Cell>>();
        ISet<Cell> possibleMoves;
        for(int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                Cell from = new Cell(i, j);
                if (piecesLocation[i, j].Item2 == myColor)
                {
                    switch (piecesLocation[i, j].Item1)
                    {
                        case ChessPieceType.Pawn:
                            possibleMoves = CalcPawnMoves(from);
                            break;
                        case ChessPieceType.Rook:
                            possibleMoves = CalcRookMoves(from);
                            break;
                        case ChessPieceType.Bishop:
                            possibleMoves = CalcBishopMoves(from);
                            break;
                        case ChessPieceType.Knight:
                            possibleMoves = CalcKnightMoves(from);
                            break;
                        case ChessPieceType.Queen:
                            possibleMoves = CalcQueenMoves(from);
                            break;
                        case ChessPieceType.King:
                            possibleMoves = CalcKingMoves(from);
                            possibleMoves.UnionWith(CalcCastling(attackedPosition));
                            break;
                        default:
                            throw new System.Exception("Неизвестная фигура");
                    }
                    foreach(Cell to in possibleMoves)
                    {
                        if (SimulateMove(from, to))
                        {
                            if (!allowedMoves.ContainsKey(from))
                            {
                                allowedMoves[from] = new HashSet<Cell>();
                            }
                            allowedMoves[from].Add(to);
                        }
                    }
                }
            }
        }
    }

    private bool SimulateMove(Cell from, Cell to)
    {
        ChessSituation newSituation = (ChessSituation)this.Clone();
        newSituation.DoMove(from, to);
        return !newSituation.AttackedPosition().Contains(newSituation.FindKing());
    }

    public object Clone()
    {
        ChessSituation clone = (ChessSituation)MemberwiseClone();
        if (aislePawnCell != null)
        {
            clone.aislePawnCell = (Cell)aislePawnCell.Clone();
        }
        clone.piecesLocation = new (ChessPieceType, PlayerColor)[8, 8];
        Array.Copy(piecesLocation, clone.piecesLocation, 64);
        clone.eatenBlackPieces = new Dictionary<ChessPieceType, int>(eatenBlackPieces);
        clone.eatenWhitePieces = new Dictionary<ChessPieceType, int>(eatenWhitePieces);
        clone.InitiazeEatenPieces(); 

        return clone;
    }

}
