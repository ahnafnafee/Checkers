using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Square : MonoBehaviourPun
{
    private Vector2 boardOffset = new Vector2(-4.0f, -4.0f);
    private Vector2 pieceOffset = new Vector2(0.5f, 0.5f);

    protected int X = 0;
    protected int Y = 0;

    public int GetX()
    {
        return X;
    }
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

    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    
    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
