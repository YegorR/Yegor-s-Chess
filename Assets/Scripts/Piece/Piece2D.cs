using UnityEngine;
using System.Collections;

public class Piece2D : Piece
{
    public Sprite whitePawn;
    public Sprite blackPawn;
    public Sprite whiteRook;
    public Sprite blackRook;
    public Sprite whiteKnight;
    public Sprite blackKnight;
    public Sprite whiteBishop;
    public Sprite blackBishop;
    public Sprite whiteQueen;
    public Sprite blackQueen;
    public Sprite whiteKing;
    public Sprite blackKing;

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
            if (value == playerColor)
            {
                return;
            }

            playerColor = value;
            changeSprite();
        }
        get
        {
            return playerColor;
        }
    }

    public override ChessPieceType ChessPieceType
    {
        set
        {
            if (value == chessPieceType)
            {
                return;
            }

            chessPieceType = value;
            changeSprite();
        }
        get
        {
            return chessPieceType;
        }
    }

    private void changeSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = null;
        if (PlayerColor == PlayerColor.White)
        {
            switch (ChessPieceType)
            {
                case ChessPieceType.Pawn:
                    sprite = whitePawn;
                    break;
                case ChessPieceType.Rook:
                    sprite = whiteRook;
                    break;
                case ChessPieceType.Knight:
                    sprite = whiteKnight;
                    break;
                case ChessPieceType.Bishop:
                    sprite = whiteBishop;
                    break;
                case ChessPieceType.Queen:
                    sprite = whiteQueen;
                    break;
                case ChessPieceType.King:
                    sprite = whiteKing;
                    break;
                default:
                    return;
            }
        }
        else if (PlayerColor == PlayerColor.Black)
        {
            switch (ChessPieceType)
            {
                case ChessPieceType.Pawn:
                    sprite = blackPawn;
                    break;
                case ChessPieceType.Rook:
                    sprite = blackRook;
                    break;
                case ChessPieceType.Knight:
                    sprite = blackKnight;
                    break;
                case ChessPieceType.Bishop:
                    sprite = blackBishop;
                    break;
                case ChessPieceType.Queen:
                    sprite = blackQueen;
                    break;
                case ChessPieceType.King:
                    sprite = blackKing;
                    break;
                default:
                    return;
            }
        }
        spriteRenderer.sprite = sprite;
    }

    protected override Vector3 TransformCellToVector3(Cell cell)
    {
        return startBoardPoint + new Vector3(cell.Vertical, cell.Horizontal, 0) * deltaCell + delta;
    }

    protected override Cell TransformVectorToCell(Vector3 vector)
    {
        Vector3 v = vector - startBoardPoint;
        float ver = v.x / deltaCell;
        float hor = v.y / deltaCell;
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
        transform.Translate(0, 0, -1);
        Camera camera = Camera.main;
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = camera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            plane = new Plane(Vector3.back, hit.point);
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
            coverUnderPiece.transform.position = startBoardPoint + new Vector3(localCell.Vertical, localCell.Horizontal, 0) * deltaCell +
                new Vector3(5, 5, 0.1f);
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
        transform.Translate(0, 0, 1);
        Cell moveCell = TransformVectorToCell(transform.position);
        if ((moveCell == null) || (!AllowedMoves.Contains(moveCell)))
        {
            Move(cell);
            return;
        }
        NotifyMove(cell, moveCell);
    }
}
