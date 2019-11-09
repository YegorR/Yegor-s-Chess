using UnityEngine;
using System.Collections;

public class Piece3D : Piece
{

    private Plane plane;
    private bool isDragged = false;
    private Vector3 deltaDrag;

    private GameObject coverUnderPiece = null;
    private Cell cellNow = null;

    private void OnMouseDown()
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

    private void OnMouseUp()
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
 