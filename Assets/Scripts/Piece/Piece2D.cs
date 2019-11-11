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

    PlayerColor playerColor;
    ChessPieceType chessPieceType;
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

}
