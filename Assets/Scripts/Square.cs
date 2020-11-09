using System.Collections;
using System.Collections.Generic;
using Mirror;
using Photon.Pun;
using UnityEngine;

public class Square : MonoBehaviour
{
    private Vector2 boardOffset = new Vector2(-4.0f, -4.0f);
    private Vector2 pieceOffset = new Vector2(0.5f, 0.5f);

    protected int x = 0;
    protected int y = 0;
    
    // PhotonView PV;

    void Awake()
    {
        // PV = GetComponent<PhotonView>();
    }
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


}
