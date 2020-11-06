using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Square : MonoBehaviour
{
    private Vector2 boardOffset = new Vector2(-4.0f, -4.0f);
    private Vector2 pieceOffset = new Vector2(0.5f, 0.5f);

    protected int x = 0;
    protected int y = 0;

    public int getX()
    {
        return x;
    }
    public void move(int x, int y)
    {
        this.x = x;
        this.y = y;
        transform.position = new Vector2(x, y) + boardOffset + pieceOffset;
    }
    public int getY()
    {
        return y;
    }



    public int GetPriority(){ return 0; }
    public void SetPriority(int priority){}
    public List<Square> getCaptures()
    {
        return new List<Square>;
    }
    public void addCapture(Square piece)
    {
    }
    public void setCapture(List<Square> pieces)
    {
    }
}
