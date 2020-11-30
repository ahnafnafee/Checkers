using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Square
{
    private readonly Vector2 boardOffset = new Vector2(-4.0f, -4.0f);
    private Piece capture;
    private readonly Vector2 pieceOffset = new Vector2(0.5f, 0.5f);


    public Piece GetCapture()
    {
        return capture;
    }

    public void SetCapture(Piece pieces)
    {
        capture = pieces;
    }

    public void MoveObj(int xVal, int yVal)
    {
        X = xVal;
        Y = yVal;
        transform.localPosition = (new Vector2(xVal, yVal) + boardOffset + pieceOffset) / 10;
    }
}