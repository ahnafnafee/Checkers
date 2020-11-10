using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Square
{
    private Piece capture;
    private int priority = 0;

    public Piece getCapture()
    {
        return capture;
    }
    public void setCapture(Piece pieces)
    {
        capture = pieces;
    }
    public int getPriority()
    {
        return priority;
    }
    public void setPriority(int priority)
    {
        this.priority = priority;
    }
}