using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private Vector2 boardOffset = new Vector2(-4.0f, -4.0f);
    private bool king = false;
    private List<Move> moves = new List<Move>();
    private Vector2 pieceOffset = new Vector2(0.5f, 0.5f);
    private int player;
    private int priority = 0;

    protected int X = 0;
    protected int Y = 0;

    public int GetPlayer()
    {
        return player;
    }
    
    public void SetPlayer(int player)
    {
        this.player = player;
    }

    public bool GetKing()
    {
        return king;
    }
    
    public void Promote()
    {
        king = true;
        gameObject.transform.Find("crown").gameObject.SetActive(true);
    }

    public void Select(bool select)
    {
        gameObject.transform.Find("selected").gameObject.SetActive(select);
    }

    public void HighlightPiece(bool select)
    {
        gameObject.transform.Find("movable").gameObject.SetActive(select);
    }

    public List<Move> GetMoves()
    {
        return moves;
    }

    public void AddMove(Move move)
    {
        int prio = move.GetPriority();
        if (prio > priority) //Force capture
        {
            foreach (Move m in moves)
            {
                DestroyImmediate(m.gameObject);
            }
            moves.Clear();
            priority = prio;
        }

        if (prio >= priority)
            moves.Add(move);
        else
            DestroyImmediate(move.gameObject);
    }

    public void ClearMoves()
    {
        foreach (Move m in moves)
        {
            DestroyImmediate(m.gameObject);
        }
        moves.Clear();
        priority = 0;
    }

    public int GetMovesNum()
    {
        return moves.Count;
    }

    public int GetPriority()
    {
        return priority;
    }

    public int GetX()
    {
        return X;
    }
    
    public void Move(int x, int y)
    {
        X = x;
        Y = y;
        transform.localPosition = (new Vector2(x, y) + boardOffset + pieceOffset)/10;
    }

    public void SetVal(int xVal, int yVal)
    {
        X = xVal;
        Y = yVal;
    }

    public int GetY()
    {
        return Y;
    }
    
    public void DestroyPiece()
    {
        DestroyImmediate(gameObject);
    }
}