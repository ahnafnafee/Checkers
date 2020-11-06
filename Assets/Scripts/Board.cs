﻿using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Board : NetworkBehaviour
{
    public GameObject highlightPrefab;
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;

    private Piece[,] pieces = new Piece[8, 8]; //Grid
    private Piece selected; //Selected piece (null if none selected)

    private List<Move> highlights = new List<Move>(); //List of all possible moves

    private Vector2 boardOffset = new Vector2(-4.0f, -4.0f);
    private Vector2 pieceOffset = new Vector2(0.5f, 0.5f);

    private Vector2 mouseOver;

    // Start is called before the first frame update
    void Start()
    {
        CreateBoard();
        //Set player1 and player2 color
        List<Move> test = new List<Move>();

    }

    [Client]
    void Update()
    {
        // if (!hasAuthority)
        // {
        //     Debug.Log("NO AUTH");
        //     return;
        // }

        CmdMovePiece();

    }

    [Command]
    private void CmdMovePiece()
    {
        RpcMove();
    }
    
    [ClientRpc]
    private void RpcMove()
    {
        UpdateMouseOver();

        int x = (int)mouseOver.x;
        int y = (int)mouseOver.y;

        if (Input.GetMouseButtonDown(0))
        {
            if (selected == null) { //No pieces are selected
                SelectPiece(x, y);
            }
            else //A piece is already selected
            {
                selected.select(false); 
                if (!SelectPiece(x, y)) //If not selecting another piece
                {
                    
                    Move selectedMove = CheckValid(x, y);
                    if (selectedMove != null)
                    {
                        Piece p = pieces[selected.getX(), selected.getY()];
                        Debug.Log("Moved piece " + p.getX() + " " + p.getY() + " to " + x + " " + y);
                        MovePiece(p, selectedMove);

                        if (y == 7)//Promote the piece
                            p.promote();
                    }
                    //After moving the piece
                    ClearHighlights();
                    FindMoves();
                }
            }
            DebugBoard();
        }
    }

    //Check if the selected move is in the list of valid moves for the selected piece
    private Move CheckValid(int x, int y)
    {
        for (int i = 0; i < highlights.Count; i++)
        {
            if (highlights[i].getX() == x && highlights[i].getY() == y)
                return highlights[i];
        }
        return null;
    }

    //Get mouse location
    private void UpdateMouseOver()
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int)(hit.point.x - boardOffset.x);
            mouseOver.y = (int)(hit.point.y - boardOffset.y);
        }
        else
        {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }

    //Create all pieces
    private void CreateBoard()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 8; x += 2)
                CreatePiece(x + y % 2, y, "player");
        }

        CreatePiece(1 , 3, "opponent");
        CreatePiece(3 , 5, "opponent");

        /*for (int y = 5; y < 8; y++)
        {
            for (int x = 0; x < 8; x += 2)
                CreatePiece(x + y % 2, y, "opponent");
        }*/

        FindMoves();
    }

    //Create a piece at x,y
    private void CreatePiece(int x, int y, string side)
    {
        GameObject go;

        //Change to check side ////////Need a method to change the prefab assigned to each player
        if (side == "player")
            go = Instantiate(whitePiecePrefab, transform, true);
        else
            go = Instantiate(blackPiecePrefab, transform, true);
        Piece p = go.GetComponent<Piece>();
        p.setX(x);
        p.setY(y);
        p.setSide(side);
        pieces[x, y] = p;

        MoveGameObject(p, x, y);
    }

    //Select the piece return true if a piece is selected
    private bool SelectPiece(int x, int y)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7)
            return false;
        Piece p = pieces[x, y];
        if (p == null) //if the selected square contains a piece
            return false;
        if (p.getSide() == "opponent") //if not the player's piece
            return false;

        ClearHighlights();
        
        Debug.Log("Selected " + x + " " + y);

        selected = p;

        if (selected.getMovesNum() > 0) //highlight piece if move is possible
        {
            selected.select(true);
            DisplayMoves();
            return true;
        }
        else //deselect piece if piece has no possible moves
        {
            selected.select(false);
            selected = null;
            return false;
        }
    }

    ///
    /// Send this message to server 
    /// p contains selected piece
    /// move contains the destination x,y and pieces to capture
    ///
    //Move the selected piece to x,y 

    private void MovePiece(Piece p, Move move)
    {
        int x = move.getX();
        int y = move.getY();
        Debug.Log(move);
        pieces[p.getX(), p.getY()] = null;
        p.setX(x);
        p.setY(y);
        pieces[x, y] = p;
        selected = null;

        bool multiCapture = false;
        List<Square> captures = move.getCaptures();
        Debug.Log("Cap num "+ x + " " + y +" "+captures.Count);
        for (int i = 0; i < captures.Count; i++)
        {
            int cX = captures[i].getX();
            int cY = captures[i].getY();
            Destroy(pieces[cX,cY].gameObject);
            multiCapture = true;
        }
        MoveGameObject(p, x, y);

        /*if (multiCapture)
        {
            SelectPiece(x, y);
        }*/
        //Select and find moves again if a capturing move is available let player move again if not end turn

    }

    private void MoveGameObject(Square go, int x, int y)
    {
        go.transform.position = (Vector2.right * x) + (Vector2.up * y) + boardOffset + pieceOffset;
    }


    //Display all possible moves of selected piece
    private void DisplayMoves()
    {
        List<Move> moves = selected.getMoves();
        for (int i = 0; i < selected.getMovesNum(); i++)
        {
            GameObject go = Instantiate(highlightPrefab, transform, true);
            Move h = go.GetComponent<Move>();

            int x = moves[i].getX();
            int y = moves[i].getY();
            List<Square> captures = moves[i].getCaptures();
            h.setX(x);
            h.setY(y);
            h.setCapture(captures);
            highlights.Add(h);

            MoveGameObject(h, x, y);
        }
    }

    //Clear highlighted squares
    private void ClearHighlights()
    {
        for (int i = 0; i < highlights.Count; i++)
        {
            Destroy(highlights[i].gameObject);
        }
        highlights.Clear();
    }


    //Find all possible moves for all pieces
    private void FindMoves()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece p = pieces[i, j];
                if (p != null)
                {

                    p.clearMoves();

                    //move forwards
                    CheckDirection(p, i, j, -1, 1);
                    CheckDirection(p, i, j, 1, 1);

                    if (p.getKing())
                    {
                        //move backwards if the piece is a king
                        CheckDirection(p, i, j, -1, -1);
                        CheckDirection(p, i, j, 1, -1);
                    }
                    
                }
                
            }
            //Remove moves with lower prioriy to force capturing
        }
    }

    private void CheckDirection(Piece p, int x, int y, int dx, int dy)
    {
        Move m = new Move();
        int adjSquare = CheckSquare(x + dx, y + dy);
        int jumpSquare = CheckSquare(x + 2 * dx, y + 2 * dy);

        if (adjSquare == 0)
        {
            m.setX(x + dx);
            m.setY(y + dy);
            m.setPriority(0);
            p.addMove(m);
        }
        else if (adjSquare == 1 && jumpSquare == 0)
        {
            m.setX(x + 2 * dx);
            m.setY(y + 2 * dy);
            m.setPriority(1);
            m.addCapture(pieces[x + dx, y + dy]);
            p.addMove(m);
        }
    }

    //Check what is on square at (x,y)
    private int CheckSquare(int x, int y)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7) //out of board
            return -1;
        if (pieces[x, y] == null) //no piece
            return 0;
        if (pieces[x, y].getSide() == "player") //player's piece
            return -1;
        return 1; //opponent's piece
    }

    //Display the current board layout in console
    private void DebugBoard()
    {
        string str = "";
        for (int j = 7; j >= 0; j--)
        {
            for (int i = 0; i < 8; i++)
            {
                if (pieces[i, j] != null)
                    str += "P";
                else
                    str += "O";
            }
            str += "\n";
        }
        Debug.Log(str);
    }
}
