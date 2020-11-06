using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : Square
{
    private string side;
    private bool king = false;

    public string getSide()
    {
        return side;
    }
    public void setSide(string side)
    {
        this.side = side;
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
