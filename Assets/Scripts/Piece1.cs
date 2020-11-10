using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Piece1 : MonoBehaviour
{
    private Vector2 boardOffset = new Vector2(-4.0f, -4.0f);
    private bool king = false;
    private List<Move1> moves = new List<Move1>();
    private Vector2 pieceOffset = new Vector2(0.5f, 0.5f);
    private int player;
    private int priority = 0;

    protected int X = 0;
    protected int Y = 0;

    // public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    // {
    //     if (targetView != base.photonView)
    //     {
    //         return;
    //     }
    //
    //     base.photonView.TransferOwnership(requestingPlayer);
    // }
    //
    // public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    // {
    //     if (targetView != base.photonView)
    //     {
    //         return;
    //     }
    //
    //     // base.photonView.TransferOwnership(previousOwner);
    // }

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

    public List<Move1> GetMoves()
    {
        return moves;
    }

    public void AddMove(Move1 move)
    {
        int prio = move.GetPriority();
        if (prio > priority) //Force capture
        {
            foreach (Move1 m in moves)
                Destroy(m.gameObject);
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
        foreach (Move1 move in moves)
            Destroy(move.gameObject);

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

    // private void OnMouseDown()
    // {
    //     base.photonView.RequestOwnership();
    // }


    public int GetX()
    {
        return X;
    }

    [PunRPC]
    public void Move(int x, int y)
    {
        X = x;
        Y = y;
        transform.position = new Vector2(x, y) + boardOffset + pieceOffset;
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
    
    [PunRPC]
    public void DestroyPiece()
    {
        Destroy(gameObject);
    }

    // private void Awake()
    // {
    //     PhotonNetwork.AddCallbackTarget(this);
    // }
    //
    // private void OnDestroy()
    // {
    //     PhotonNetwork.RemoveCallbackTarget(this);
    // }
}