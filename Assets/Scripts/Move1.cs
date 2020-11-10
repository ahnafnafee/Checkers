using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move1 : Piece1
{
    private Piece1 capture;
    private int priority = 0;

    public Piece1 GetCapture()
    {
        return capture;
    }
    public void SetCapture(Piece1 pieces)
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
}
