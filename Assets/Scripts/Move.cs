using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Piece
{
    private Piece capture;
    private int priority = 0;

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
}