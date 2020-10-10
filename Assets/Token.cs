using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour
{
    private int x;
    private int y;
    private string side;
    private List<Move> moves = new List<Move>();
    private bool king = false;

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

    public string getSide()
    {
        return side;
    }
    public void setSide(string side)
    {
        this.side = side;
    }

    public List<Move> getMoves()
    {
        return moves;
    }
    public void addMove(Move move)
    {
        moves.Add(move);
    }
    public void clearMoves()
    {
        moves.Clear();
    }
    public int getMovesNum()
    {
        return moves.Count;
    }


    public bool getKing()
    {
        return king;
    }
    public void promote()
    {
        king = true;
        gameObject.transform.Find("crown").gameObject.SetActive(true);
    }

    public void select(bool select)
    {
        gameObject.transform.Find("selected").gameObject.SetActive(select);
    }

}
