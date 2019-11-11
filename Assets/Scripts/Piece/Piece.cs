using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{

    public Vector3 startBoardPoint;
    public float deltaCell;
    public Vector3 delta;

    public GameObject wrongMoveObject;
    public GameObject possibleMoveObject;

    protected Cell cell;

    public delegate void MoveIsMade(Cell from, Cell to);
    public event MoveIsMade MoveIsMadeEvent;

    private Plane plane;
    private bool isDragged = false;
    private Vector3 deltaDrag;

    private GameObject coverUnderPiece = null;
    private Cell cellNow = null;

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

    void OnMouseDown()
    {
        if (Block)
        {
            return;
        }
        if (Input.GetMouseButton(1))
        {
            return;
        }
        Camera camera = Camera.main;
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = camera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            plane = new Plane(Vector3.up, hit.point);
            isDragged = true;
            deltaDrag = hit.point - transform.position;
        }
          
    }

    void OnMouseDrag()
    {
    
        if (!isDragged)
        {
            return;
        }
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 newPosition = ray.GetPoint(distance) - deltaDrag;
            transform.position = newPosition;
            manageCoverUnderPiece();
        }

        if (Input.GetMouseButton(1))
        {
            Move(cell);
            OnMouseUp();
        }
    }

    private void manageCoverUnderPiece()
    {
        Cell localCell = TransformVectorToCell(transform.position);

        if (localCell != cellNow)
        {
            if (coverUnderPiece != null)
            {
                Destroy(coverUnderPiece);
                coverUnderPiece = null;
            }
            cellNow = localCell;
            if ((cellNow == null) || (cellNow.Equals(cell)))
            {
                return;
            }

            if (AllowedMoves.Contains(cellNow))
            {
                coverUnderPiece = Instantiate(possibleMoveObject);
            }
            else
            {
                coverUnderPiece = Instantiate(wrongMoveObject);
            }
            coverUnderPiece.transform.position = startBoardPoint + new Vector3(localCell.Vertical, 0, localCell.Horizontal) * deltaCell +
                new Vector3(5, 0.001f, 5);
        }
    }

    void OnMouseUp()
    {
        if (!isDragged)
        {
            return;
        }

        if (coverUnderPiece != null)
        {
            Destroy(coverUnderPiece);
            coverUnderPiece = null;
        }
        isDragged = false;
        Cell moveCell = TransformVectorToCell(transform.position);
        if ((moveCell == null) || (!AllowedMoves.Contains(moveCell)))
        {
            Move(cell);
            return;
        }
        NotifyMove(cell, moveCell);
    }
}
