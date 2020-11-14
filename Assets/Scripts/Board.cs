using System.Collections.Generic;
using Mirror;
using Photon.Pun;
using UnityEngine;

public class Board : MonoBehaviourPunCallbacks
{
    // TODO: Piece should be selectable by BOTH players
    // TODO: Move and highlight objects should be client side
    // TODO: Fix highlight spawn on other client
    // TODO: Piece array needs to be synced
    // TODO: Implement server side turn manager (Look into PunTurnManager)

    public static Board Instance;
    private PhotonView pv;
    private Piece tPiece;
    private Move sMove;
    
    public GameObject highlightPrefab;
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    public GameObject square;
    public GameObject move;

    private Piece selected; //Selected piece (null if none selected)

    private List<Move> highlights = new List<Move>(); //List of all possible moves

    private Vector2 boardOffset = new Vector2(-4.0f, -4.0f);
    private Vector2 pieceOffset = new Vector2(0.5f, 0.5f);

    private Vector2 mouseOver;

    private int turn = 1; //1 = player 1; 2 = player 2

    //Change player color
    private string player1Color = "White";
    private string player2Color = "Black";

    bool multiCapture = false;
    
    
    void Awake()
    {
        Instance = this;
        pv = GetComponent<PhotonView>();
    }
    
    void Start()
    {
        CreateBoard();
        //Set player1 and player2 color
    }

    // [PunRPC]
    void Update()
    {
        UpdateMouseOver();

        int x = (int)mouseOver.x;
        int y = (int)mouseOver.y;

        if (Input.GetMouseButtonDown(0))
        {
            if (multiCapture)
            {
                Move selectedMove = CheckValid(x, y);
                if (selectedMove != null)
                {
                    sMove = selectedMove;
                    MovePiece();
                }
            }
            else if (selected == null) //No pieces are selected
            {
                SelectPiece(x, y);
            }
            else //A piece is already selected
            {
                selected.Select(false);
                if (!SelectPiece(x, y)) //If not selecting another piece
                {
                    Move selectedMove = CheckValid(x, y);
                    if (selectedMove != null)
                    {
                        sMove = selectedMove;
                        MovePiece();
                    }
                }
            }
            
            //DebugBoard();
        }
    }
    
    private Piece[,] GetActivePieces()
    {
        Piece[,] pieces = new Piece[8, 8];
        Piece p = null;
        foreach(Transform child in transform.Find("Pieces"))
        {
            p = child.GetComponent<Piece>();
            pieces[p.GetX(), p.GetY()] = p;
        }
        return pieces;
    }

    private void MovePiece()
    {
        Piece p = selected;
        Move move = sMove;
        Piece[,] pieces = GetActivePieces();

        int x = move.GetX();
        int y = move.GetY();
        Debug.Log("Moved piece " + p.GetX() + " " + p.GetY() + " to " + x + " " + y);
        
        // NetSynced Move
        PhotonView pView = p.GetComponent<PhotonView>();
        pView.RPC("Move", RpcTarget.All,  x, y);
        
        // p.Move(x, y);

        ClearHighlights();

        //Delete captured piece
        Piece capture = move.GetCapture();
        if (capture != null) { 
            int cX = capture.GetX();
            int cY = capture.GetY();

            pView = capture.GetComponent<PhotonView>();
            pView.RPC("DestroyPiece", RpcTarget.All);


            //clear all moves
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Piece tmpP = pieces[i, j];
                    if (tmpP != null)
                        tmpP.ClearMoves();
                }
            }
            //find additional capture
            findMultiCapture(x, y, x - cX, y - cY);
        }

        if (multiCapture)
        {
            selected.Select(true);
            DisplayMoves();
        }
        else 
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Piece tmpP = pieces[i, j];
                    if (tmpP != null)
                        tmpP.ClearMoves();
                }
            }
            selected.Select(false);
            selected = null;
            turn = (turn == 1) ? 2 : 1;
            //Display turn change
            Debug.Log("Turn " + turn);

            FindMoves();

            //Check winner
            winner();
        }

        //Promote the piece
        if ((p.GetPlayer() == 1 && y == 7) ||
            (p.GetPlayer() == 2 && y == 0))
            p.Promote();
    }
    

    //Check if the selected move is in the list of valid moves for the selected piece
    private Move CheckValid(int x, int y)
    {
        for (int i = 0; i < highlights.Count; i++)
        {
            if (highlights[i].GetX() == x && highlights[i].GetY() == y)
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
    // [PunRPC]
    public void CreateBoard()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("ONLY MASTER");
            
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 8; x += 2)
                    CreatePiece(x + y % 2, y, 1);
            }
            for (int y = 5; y < 8; y++)
            {
                for (int x = 0; x < 8; x += 2)
                    CreatePiece(x + y % 2, y, 2);
            }
            FindMoves();
            
        }
        
        //Multi capture front
        /*CreatePiece(1, 1, 1);
        CreatePiece(2, 2, 2);
        CreatePiece(4, 4, 2);
        CreatePiece(6, 6, 2);
        CreatePiece(2, 4, 2);
        CreatePiece(4, 6, 2);*/

        //multi capture king
        /*CreatePiece(6, 6, 1);
        pieces[6, 6].promote();
        CreatePiece(1, 1, 2);
        CreatePiece(3, 3, 2);
        CreatePiece(5, 5, 2);
        CreatePiece(1, 3, 2);
        CreatePiece(1, 5, 2);
        CreatePiece(3, 5, 2);
        CreatePiece(7, 5, 2);*/

        //promote mid multi capture
        /*CreatePiece(1, 1, 1);
        CreatePiece(3, 1, 1);
        CreatePiece(2, 2, 2);
        CreatePiece(4, 4, 2);
        CreatePiece(6, 6, 2);
        CreatePiece(2, 4, 2);
        CreatePiece(4, 6, 2);*/

        
    }

    //Create a piece at x,y
    private void CreatePiece(int x, int y, int player)
    {
        Piece p;
        if (player == 1)
            p = CreatePiecePrefab(player1Color);
        else
            p = CreatePiecePrefab(player2Color);
        
        // NetSynced Move
        PhotonView pView = p.GetComponent<PhotonView>();
        pView.RPC("Move", RpcTarget.All,  x, y);
        
        // p.Move(x,y);
        
        
        p.SetPlayer(player);
    }

    //Select the piece return true if a piece is selected
    private bool SelectPiece(int x, int y)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7)
            return false;
        Piece p = GetActivePieces()[x, y];
        if (p == null) //if the selected square contains a piece
            return false;
        if (p.GetPlayer() != turn) //if not the player's piece
            return false;

        ClearHighlights();
        
        Debug.Log("Selected " + x + " " + y);

        selected = p;

        if (selected.GetMovesNum() > 0) //highlight piece if move is possible
        {
            selected.Select(true);
            DisplayMoves();
            return true;
        }
        else //deselect piece if piece has no possible moves
        {
            selected.Select(false);
            selected = null;
            return false;
        }
    }

    private void winner()
    {
        int p1Count = 0;
        int p1MovesCount = 0;
        int p2Count = 0;
        int p2MovesCount = 0;
        Piece[,] pieces = GetActivePieces();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece p = pieces[i, j];
                if (p != null)
                {
                    if (p.GetPlayer() == 1)
                    {
                        p1Count += 1;
                        p1MovesCount += p.GetMovesNum();
                    }
                    else
                    {
                        p2Count += 1;
                        p2MovesCount += p.GetMovesNum();
                    }
                }
                    
            }
        }
        //if no pieces left or no available moves
        if (p2Count == 0 || p2MovesCount == 0 && turn == 2)
            Debug.Log("P1 won");
        else if (p1Count == 0 || p1MovesCount == 0 && turn == 1)
            Debug.Log("P2 won");
    }

    

    private void findMultiCapture(int x, int y, int dx, int dy)
    {
        multiCapture = false;
        Piece[,] pieces = GetActivePieces();

        int adjSquareL = CheckSquare(pieces, x - 1, y + dy);
        int jumpSquareL = CheckSquare(pieces, x - 2, y + 2 * dy);
        int adjSquareR = CheckSquare(pieces, x + 1, y + dy);
        int jumpSquareR = CheckSquare(pieces, x + 2, y + 2 * dy);

        if (adjSquareL == 1 && jumpSquareL == 0)
        {
            Move mL = CreateMovePrefab("Move");
            
            // NetSynced Move / Client ?
            // PhotonView pView = mL.GetComponent<PhotonView>();
            // pView.RPC("Move", RpcTarget.All,  x - 2, y + 2 * dy);
            
            mL.Move(x - 2, y + 2 * dy);
            
            
            mL.SetPriority(1);
            mL.SetCapture(pieces[x - 1, y + dy]);
            selected.AddMove(mL);
            multiCapture = true;
        }

        if (adjSquareR == 1 && jumpSquareR == 0)
        {
            Move mR = CreateMovePrefab("Move");
            
            // NetSynced Move / Client ?
            // PhotonView pView = mR.GetComponent<PhotonView>();
            // pView.RPC("Move", RpcTarget.All,  x + 2, y + 2 * dy);
            
            mR.Move(x + 2, y + 2 * dy);
            
            mR.SetPriority(1);
            mR.SetCapture(pieces[x + 1, y + dy]);
            selected.AddMove(mR);
            multiCapture = true;
        }

        if (selected.GetKing())
        {
            int adjSquareB = CheckSquare(pieces, x + dx, y - dy);
            int jumpSquareB = CheckSquare(pieces, x + 2 * dx, y - 2 * dy);

            if (adjSquareB == 1 && jumpSquareB == 0)
            {
                Move mB = CreateMovePrefab("Move");
                
                // NetSynced Move / Client ?
                // PhotonView pView = mB.GetComponent<PhotonView>();
                // pView.RPC("Move", RpcTarget.All,  x + 2 * dx, y - 2 * dy);
                
                mB.Move(x + 2 * dx, y - 2 * dy);
                
                mB.SetPriority(1);
                mB.SetCapture(pieces[x + dx, y - dy]);
                selected.AddMove(mB);
                multiCapture = true;
            }
        }
    }

    //Display all possible moves of selected piece
    private void DisplayMoves()
    {
        List<Move> moves = selected.GetMoves();
        for (int i = 0; i < selected.GetMovesNum(); i++)
        {
            Move h = CreateMovePrefab("Highlight");
            int x = moves[i].GetX();
            int y = moves[i].GetY();
            Piece capture = moves[i].GetCapture();
            
            // NetSynced Move / Client ?
            // PhotonView pView = h.GetComponent<PhotonView>();
            // pView.RPC("Move", RpcTarget.All,  x, y);

            h.Move(x,y);
            
            h.SetCapture(capture);
            highlights.Add(h);
        }
    }

    //Clear highlighted squares
    private void ClearHighlights()
    {
        for (int i = 0; i < highlights.Count; i++)
        {
            // NetSynced Destroy
            PhotonView pView = highlights[i].GetComponent<PhotonView>();
            pView.RPC("DestroyPiece", RpcTarget.All);
            // Destroy(highlights[i].gameObject);
        }

        highlights.Clear();
    }

    //Find all possible moves for all pieces
    private void FindMoves()
    {
        int priority = 0;
        List<Piece> movablePieces = new List<Piece>();
        Piece[,] pieces = GetActivePieces();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece p = pieces[i, j];
                if (p == null)
                    continue;
                p.ClearMoves();

                int player = p.GetPlayer();
                if (player != turn)
                    continue;

                int up = 1;
                int dn = -1;
                if (player == 2)
                {
                    up = -1;
                    dn = 1;
                }

                //move forwards
                CheckDirection(p, pieces, i, j, dn, up);
                CheckDirection(p, pieces, i, j, up, up);

                if (p.GetKing()) //move backwards if the piece is a king
                {
                    CheckDirection(p, pieces, i, j, dn, dn);
                    CheckDirection(p, pieces, i, j, up, dn);
                }

                //If a capture move is available, keep only capture moves
                int prio = p.GetPriority();
                if (prio > priority)
                {
                    foreach (Piece piece in movablePieces)
                        piece.ClearMoves();

                    movablePieces.Clear();
                    priority = prio;
                }
                if (prio >= priority)
                    movablePieces.Add(p);
                else
                    p.ClearMoves();

            }

        }
    }

    private Piece CreateSquarePrefab(string c)
    {
        GameObject go = PhotonNetwork.Instantiate("Square", transform.position, Quaternion.identity);
        go.transform.parent = transform.Find("Moves").transform;
        return go.GetComponent<Piece>();
    }
    private Move CreateMovePrefab(string c)
    {
        GameObject go;
        switch (c)
        {
            case "Highlight":
                go = PhotonNetwork.Instantiate("Highlight", transform.position, Quaternion.identity);
                go.transform.parent = transform.Find("Moves").transform;
                return go.GetComponent<Move>();
            default:
                go = PhotonNetwork.Instantiate("Move", transform.position, Quaternion.identity);
                go.transform.parent = transform.Find("Moves").transform;
                return go.GetComponent<Move>();
        }
    }
    private Piece CreatePiecePrefab(string c)
    {
        GameObject go;
        switch (c)
        {
            case "White":
                go = PhotonNetwork.Instantiate("LightPiece", transform.position, Quaternion.identity);
                go.transform.parent = transform.Find("Pieces").transform;
                return go.GetComponent<Piece>();
            default:
                go = PhotonNetwork.Instantiate("DarkPiece", transform.position, Quaternion.identity);
                go.transform.parent = transform.Find("Pieces").transform;
                return go.GetComponent<Piece>();
        }
    }

    private void CheckDirection(Piece p, Piece[,] pieces, int x, int y, int dx, int dy)
    {
        Move m = CreateMovePrefab("Move");
        int adjSquare = CheckSquare(pieces, x + dx, y + dy);
        int jumpSquare = CheckSquare(pieces, x + 2 * dx, y + 2 * dy);


        if (adjSquare == 0) //Move
        {
            
            // NetSynced Move / Client ?
            // PhotonView pView = m.GetComponent<PhotonView>();
            // pView.RPC("Move", RpcTarget.All,  x + dx, y + dy);
            
            m.Move(x + dx, y + dy);
            
            
            m.SetPriority(0);
            p.AddMove(m);
        }
        else if (adjSquare == 1 && jumpSquare == 0) //Capture
        {
            
            // NetSynced Move / Client ?
            // PhotonView pView = m.GetComponent<PhotonView>();
            // pView.RPC("Move", RpcTarget.All,  x + 2 * dx, y + 2 * dy);
            
            m.Move(x + 2 * dx, y + 2 * dy);
            
            
            m.SetPriority(1);
            m.SetCapture(pieces[x + dx, y + dy]);
            p.AddMove(m);
        }
        else //No possible move
        {
            // NetSynced Destroy
            PhotonView pView = m.GetComponent<PhotonView>();
            pView.RPC("DestroyPiece", RpcTarget.All);
            // Destroy(m.gameObject);
        }
    }

    // TODO: Sync checkers array across clients
    [PunRPC]
    private void SetCheckersArray()
    {
        
    }

    //Check what is on square at (x,y)
    private int CheckSquare(Piece[,] pieces, int x, int y)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7) //out of board
            return -1;
        if (pieces[x, y] == null) //no piece
            return 0;
        if (pieces[x, y].GetPlayer() == turn) //player's piece
            return -1;
        return 1; //opponent's piece
    }

    //Display the current board layout in console
    private void DebugBoard()
    {
        string str = "";
        Piece[,] pieces = GetActivePieces();
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
    
    [PunRPC]
    private void MoveGameObject()
    {
        tPiece.transform.position = new Vector2(tPiece.GetX(), tPiece.GetY()) + boardOffset + pieceOffset;
    }
}
