using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : Square
{
    private string side;
    private bool king = false;
    private List<Move> moves = new List<Move>();

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

    public List<Move> getMoves()
    {
        return moves;
    }
    public void addMove(Move move)
    {
        moves.Add(move);

        //remove lower priority moves
    }
    public void clearMoves()
    {
        moves.Clear();
    }
    public int getMovesNum()
    {
        return moves.Count;
    }


}
