using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{

    public Vector3 startBoardPoint;
    public float deltaCell;
    public Vector3 delta;



    protected Cell cell;

    public event MoveIsMadeEventHandler MoveIsMadeEvent;


    public bool Block
    {
        set; get;
    } = true;

    public ISet<Cell> AllowedMoves
    {
        set; get;
    }

    public abstract ChessPieceType ChessPieceType{
        set; get;
    }

    public abstract PlayerColor PlayerColor
    {
        set; get;
    }

    protected abstract Vector3 TransformCellToVector(Cell cell);

    protected abstract Cell TransformVectorToCell(Vector3 vector);

    public void Move(Cell cell)
    {
        transform.position = TransformCellToVector(cell);
        this.cell = cell;
    }

    protected void NotifyMove(Cell from, Cell to)
    {
        MoveIsMadeEvent?.Invoke(from, to);
    }
}
