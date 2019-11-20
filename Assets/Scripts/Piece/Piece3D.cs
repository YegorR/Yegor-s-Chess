using UnityEngine;
using UnityEngine.EventSystems;

public class Piece3D : Piece
{

    public Mesh pawn;
    public Mesh rook;
    public Mesh knight;
    public Mesh bishop;
    public Mesh queen;
    public Mesh king;

    public Material whiteMaterial;
    public Material blackMaterial;

    private PlayerColor playerColor = PlayerColor.White;
    private ChessPieceType chessPieceType = ChessPieceType.Pawn;

    private Plane plane;
    private bool isDragged = false;
    private Vector3 deltaDrag;

    private GameObject coverUnderPiece = null;
    private Cell cellNow = null;

    public GameObject wrongMoveObject;
    public GameObject possibleMoveObject;


    public override PlayerColor PlayerColor
    {
        set
        {
            if (playerColor == value)
            {
                return;
            }
            playerColor = value;
            changeColor();
            if (chessPieceType == ChessPieceType.Knight)
            {
                transform.Rotate(new Vector3(0, 180, 0));
            }
        }
        get
        {
            return playerColor;
        }
    }

    public override ChessPieceType ChessPieceType { 
        get
        {
            return chessPieceType;
        }
        set
        {
            if (value == chessPieceType)
            {
                return;
            }
            if (((value == ChessPieceType.Knight) || (chessPieceType == ChessPieceType.Knight)) && (playerColor == PlayerColor.White))
            {
                transform.Rotate(new Vector3(0, 180, 0));
            }
            chessPieceType = value;
            changePiece();
       
        }
    }

    private void changeColor()
    {
        GetComponent<Renderer>().material = (playerColor == PlayerColor.White) ? whiteMaterial : blackMaterial;
    }

    private void changePiece()
    {
        Mesh mesh;
        switch(chessPieceType)
        {
            case ChessPieceType.Pawn:
                mesh = pawn;
                break;
            case ChessPieceType.Knight:
                mesh = knight;
                break;
            case ChessPieceType.Bishop:
                mesh = bishop;
                break;
            case ChessPieceType.Rook:
                mesh = rook;
                break;
            case ChessPieceType.Queen:
                mesh = queen;
                break;
            case ChessPieceType.King:
                mesh = king;
                break;
            default:
                return;
        }
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    protected override Vector3 TransformCellToVector3(Cell cell)
    {
        return startBoardPoint + new Vector3(cell.Vertical, 0, cell.Horizontal) * deltaCell + delta;
    }

    protected override Cell TransformVectorToCell(Vector3 vector)
    {
        Vector3 v = vector - startBoardPoint;
        float ver = v.x / deltaCell;
        float hor = v.z / deltaCell;
        if ((ver <= 0) || (hor <= 0))
        {
            return null;
        }
        int verInt = (int)ver;
        int horInt = (int)hor;
        if ((verInt >= 8) || (horInt >= 8))
        {
            return null;
        }
        return new Cell(verInt, horInt);
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
        if (EventSystem.current.IsPointerOverGameObject())
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

    protected void manageCoverUnderPiece()
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
 