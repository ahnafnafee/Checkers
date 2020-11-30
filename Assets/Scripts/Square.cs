using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    protected int Priority;

    protected int X;
    protected int Y;
    
    public int GetPriority()
    {
        return Priority;
    }

    public void SetPriority(int priorityVal)
    {
        Priority = priorityVal;
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
