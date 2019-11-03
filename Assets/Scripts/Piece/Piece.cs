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

    public ChessPieceType ChessPieceType{
        set; get;
    }

    public PlayerColor PlayerColor
    {
        set; get;
    }


    void Start()
    {
        
    }

   
    void Update()
    {

    }

    protected Vector3 TransformCellToVector3(Cell cell)
    {
        return startBoardPoint + new Vector3(cell.Vertical, 0, cell.Horizontal) * deltaCell + delta;
    }

    protected Cell TransformVectorToCell(Vector3 vector) {
        Vector3 v = vector - startBoardPoint;
        float ver = v.x / deltaCell;
        float hor = v.z / deltaCell;
        if ((ver <= 0) || (hor <= 0))
        {
            return null;
        }
        int verInt = (int)ver;
        int horInt = (int)hor;
        if ((verInt >= 8 ) || (horInt >= 8))
        {
            return null;
        }
        return new Cell(verInt, horInt);
    }

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
