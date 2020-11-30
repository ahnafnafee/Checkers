using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private readonly Vector2 boardOffset = new Vector2(-4.0f, -4.0f);
    private bool king = false;
    private List<Move> moves = new List<Move>();
    private readonly Vector2 pieceOffset = new Vector2(0.5f, 0.5f);
    private int player;
    private int priority = 0;

    private int x = 0;
    private int y = 0;

    public int GetPlayer()
    {
        return player;
    }

    [PunRPC]
    public void SetPlayer(int player)
    {
        this.player = player;
    }

    public bool GetKing()
    {
        return king;
    }

    [PunRPC]
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
        var prio = move.GetPriority();
        if (prio > priority) //Force capture
        {
            foreach (var m in moves)
            {
                Destroy(m.gameObject);
            }
            moves.Clear();
            priority = prio;
        }

        if (prio >= priority)
            moves.Add(move);
        else
            Destroy(move.gameObject);
    }

    public void ClearMoves()
    {
        foreach (var m in moves)
        {
            Destroy(m.gameObject);
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
        return x;
    }

    [PunRPC]
    public void Move(int xVal, int yVal)
    {
        x = xVal;
        y = yVal;
        transform.localPosition = (new Vector2(xVal, yVal) + boardOffset + pieceOffset)/10;
    }

    public void SetVal(int xVal, int yVal)
    {
        x = xVal;
        y = yVal;
    }

    public int GetY()
    {
        return y;
    }
    
    [PunRPC]
    public void DestroyPiece()
    {
        Destroy(gameObject);
    }

    [PunRPC]
    public void SetParent(int pvID)
    {
        PhotonView pv = PhotonView.Find(pvID);
        transform.parent = pv.gameObject.transform.Find("Pieces").transform;
    }
}