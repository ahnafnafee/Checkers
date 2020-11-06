using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Square
{
    private List<Square> captures = new List<Square>();
    private int priority = 0;

    public List<Square> getCaptures()
    {
        return captures;
    }
    public void addCapture(Square piece)
    {
        captures.Add(piece);
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
