using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessGame: MonoBehaviour
{
    private Situation situation;

    public void Awake()
    {
        situation.InitializeBoard();
    }    

    public GameSituation MakeMove(Cell from, Cell to)
    {
        return situation.makeMove(from, to);
    }
}

class Situation
{
    private (ChessPieceType, PlayerColor) VOID_CELL = (ChessPieceType.None, PlayerColor.None);
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

    internal void InitializeBoard()
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

        piecesLocation[3, 0] = (ChessPieceType.Quenn, PlayerColor.White);
        piecesLocation[4, 0] = (ChessPieceType.King, PlayerColor.White);
        piecesLocation[3, 7] = (ChessPieceType.Quenn, PlayerColor.Black);
        piecesLocation[4, 7] = (ChessPieceType.Bishop, PlayerColor.Black);
    }


    internal GameSituation makeMove(Cell from, Cell to)
    {
        //1 Проверить, что сделанный ход действительно был правильным, иначе кинуть исключение
        if ((!allowedMoves.ContainsKey(from)) || ((!allowedMoves[from].Contains(to))))
        {
            throw new System.Exception("Запрещённый ход!");
        }

        //2 Сделать собственно ход
        doMove(from, to);

        //3 Проверить ситуацию на шах

        //4 Проверить возможные ходы
        throw new System.Exception("makeMove пока не функционирует");
    }

    internal void doMove(Cell from, Cell to)
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
                    if (to.Vertical == 2)
                    {
                        piecesLocation[0, 0] = VOID_CELL;
                        piecesLocation[4, 0] = VOID_CELL;
                        piecesLocation[2, 0] = (ChessPieceType.King, PlayerColor.White);
                        piecesLocation[3, 0] = (ChessPieceType.Rook, PlayerColor.White);
                        isWhiteKingMoved = true;
                        isWhiteQueenRookMoved = true;
                        return;
                    }
                    if (to.Vertical == 6)
                    {
                        piecesLocation[7, 0] = VOID_CELL;
                        piecesLocation[4, 0] = VOID_CELL;
                        piecesLocation[6, 0] = (ChessPieceType.King, PlayerColor.White);
                        piecesLocation[5, 0] = (ChessPieceType.Rook, PlayerColor.White);
                        isWhiteKingMoved = true;
                        isWhiteKingRookMoved = true;
                        return;
                    }
                }
            }
            else
            {
                if (!isBlackKingMoved)
                {
                    if (to.Vertical == 2)
                    {
                        piecesLocation[0, 7] = VOID_CELL;
                        piecesLocation[4, 7] = VOID_CELL;
                        piecesLocation[2, 7] = (ChessPieceType.King, PlayerColor.Black);
                        piecesLocation[3, 7] = (ChessPieceType.Rook, PlayerColor.Black);
                        isBlackKingMoved = true;
                        isBlackQueenRookMoved = true;
                        return;
                    }
                    if (to.Vertical == 6)
                    {
                        piecesLocation[7, 7] = VOID_CELL;
                        piecesLocation[4, 7] = VOID_CELL;
                        piecesLocation[6, 7] = (ChessPieceType.King, PlayerColor.Black);
                        piecesLocation[5, 7] = (ChessPieceType.Rook, PlayerColor.Black);
                        isBlackKingMoved = true;
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
                piecesLocation[to.Vertical, to.Horizontal] = VOID_CELL;
                aislePawnCell = null;
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
        }
        aislePawnCell = null;

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

    internal bool IsCheck()
    {
        //1 Найти короля
        int vKing = -1;
        int hKing = -1;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var piece = piecesLocation[i, j];
                if (piece == (ChessPieceType.King, isWhiteMoving ? PlayerColor.White : PlayerColor.Black))
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

        //2 Для каждой вражеской фигуры определить накрываемые клетки
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
        }
        return moves;
    }
}
