using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject piece;
    public GameObject newPiece;
    void Start()
    {
        newPiece = Instantiate(piece);
        Piece3D thePiece = newPiece.GetComponent<Piece3D>();
        thePiece.Move(new Cell(0, 0));
        thePiece.AllowedMoves = new HashSet<Cell>();
        thePiece.AllowedMoves.Add(new Cell(0, 1));
        thePiece.AllowedMoves.Add(new Cell(1, 0));
        thePiece.AllowedMoves.Add(new Cell(1, 1));
        thePiece.MoveIsMadeEvent += MoveIsMade;
    }

    void MoveIsMade(Cell from, Cell to)
    {
        Piece3D thePiece = newPiece.GetComponent<Piece3D>();
        thePiece.Move(to);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
