using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{

    public Vector3 startBoardPoint;
    public float deltaCell;
    public Vector3 delta;



    protected Cell cell;

    public delegate void MoveIsMade(Cell from, Cell to);
    public event MoveIsMade MoveIsMadeEvent;

    

    public bool Block
    {
        set; get;
    } = false;

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

    protected abstract Vector3 TransformCellToVector3(Cell cell);

    protected abstract Cell TransformVectorToCell(Vector3 vector);

    public void Move(Cell cell)
    {
        transform.position = TransformCellToVector3(cell);
        this.cell = cell;
    }

    protected void NotifyMove(Cell from, Cell to)
    {
        MoveIsMadeEvent?.Invoke(from, to);
    }
}
