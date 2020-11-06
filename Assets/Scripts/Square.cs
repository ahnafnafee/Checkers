using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Square : NetworkBehaviour
{
    protected int x;
    protected int y;

    public int getX()
    {
        return x;
    }
    public void setX(int x)
    {
        this.x = x;
    }
    public int getY()
    {
        return y;
    }
    public void setY(int y)
    {
        this.y = y;
    }

}
