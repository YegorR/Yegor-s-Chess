using UnityEngine;
using System.Collections;

public class Piece3D : Piece
{

    private Plane plane;
    private bool isDragged = false;
    private Vector3 deltaDrag;

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
        }

        if (Input.GetMouseButton(1))
        {
            Move(cell);
            OnMouseUp();
        }
    }

    private void OnMouseUp()
    {
        if (!isDragged)
        {
            return;
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
 