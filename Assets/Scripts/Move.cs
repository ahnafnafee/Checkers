using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private Vector2 boardOffset = new Vector2(-4.0f, -4.0f);
    private Vector2 pieceOffset = new Vector2(0.5f, 0.5f);
    private Piece capture;
    private int priority = 0;
    protected int X = 0;
    protected int Y = 0;

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
    public void MoveObj(int x, int y)
    {
        X = x;
        Y = y;
        transform.localPosition = (new Vector2(x, y) + boardOffset + pieceOffset)/10;
    }
    public void SetVal(int xVal, int yVal)
    {
        X = xVal;
        Y = yVal;
    }
    public int GetX()
    {
        return X;
    }
    public int GetY()
    {
        return Y;
    }
}