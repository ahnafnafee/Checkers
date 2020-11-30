using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private readonly Vector2 boardOffset = new Vector2(-4.0f, -4.0f);
    private Piece capture;
    private readonly Vector2 pieceOffset = new Vector2(0.5f, 0.5f);
    private int priority = 0;
    private int x = 0;
    private int y = 0;

    public Piece GetCapture()
    {
        return capture;
    }

    public void SetCapture(Piece pieces)
    {
        capture = pieces;
    }

    public int GetPriority()
    {
        return priority;
    }

    public void SetPriority(int priorityVal)
    {
        priority = priorityVal;
    }

    public void MoveObj(int xVal, int yVal)
    {
        x = xVal;
        y = yVal;
        transform.localPosition = (new Vector2(xVal, yVal) + boardOffset + pieceOffset) / 10;
    }

    public void SetVal(int xVal, int yVal)
    {
        x = xVal;
        y = yVal;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }
}