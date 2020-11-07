using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : Square
{
    private int player;
    private bool king = false;
    private List<Move> moves = new List<Move>();
    private int priority = 0;

    public int getPlayer()
    {
        return player;
    }
    public void setPlayer(int player)
    {
        this.player = player;
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
        int prio = move.getPriority();
        if (prio > priority) //Force capture
        {
            foreach (Move m in moves)
                Destroy(m.gameObject);
            moves.Clear();
            priority = prio;
        }
        if (prio >= priority)
            moves.Add(move);
        else
            Destroy(move.gameObject);
    }
    public void clearMoves()
    {
        foreach (Move move in moves)
            Destroy(move.gameObject);

        moves.Clear();
        priority = 0;
    }
    public int getMovesNum()
    {
        return moves.Count;
    }

    public int getPriority()
    {
        return priority;
    }


}
