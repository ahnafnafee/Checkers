using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour
{
    [Header("Instantiable Prefabs")] 
    public GameObject blackPiecePrefab = default;
    public GameObject whitePiecePrefab = default;
    public GameObject highlightPrefab = default;
    public GameObject MovePrefab = default;

    [Header("Game Manager")] 

    private Vector2 mouseOver = default;
    private List<Move> highlights = new List<Move>(); //List of all possible Moves

    [Header("Piece Attributes")]
    private bool isClickable = true;
    public GameObject square = default;
    private Piece tPiece = default;
    private Piece selected = default; //Selected Piece (null if none selected)
    private Move sMove = default;
    bool multiCapture = false;
    private Vector2 PieceOffset = new Vector2(0.5f, 0.5f);

    [Header("Player Attributes")]
    //Change player color
    private string player1Color = "Dark";
    private string player2Color = "Light";
    private int priority = default;
    private string p1ID = default;
    private string p2ID = default;


    [Header("Board Attributes")] 

    private int turn = 1; //1 = player 1; 2 = player 2
    private Vector2 boardOffset = new Vector2(-4.0f, -4.0f);
    private bool gameCompleted = default;

    [Header("GUI")] 
    public GameObject winGUI = default;
    public GameObject restartBtn = default;
    public GameObject playerSelected1 = default;
    public GameObject playerSelected2 = default;

    void Start()
    {
        // Default conditions
        isClickable = true;
        gameCompleted = false;
        playerSelected1.SetActive(true);
        playerSelected2.SetActive(false);

        CreateBoard();
        //Set player1 and player2 color
        winGUI.SetActive(false);
    }

    void Update()
    {
        UpdateMouseOver();

        // For clicking Pieces
        ClickPiece();
    }



    private void ClickPiece()
    {
        if (!isClickable)
            return;
        
        int x = (int) mouseOver.x;
        int y = (int) mouseOver.y;

        if (Input.GetMouseButtonDown(0))
        {
            DebugBoard();

            if (turn == 2)
            {
                x = 7 - x;
                y = 7 - y;
            }
            
            if (multiCapture)
            {
                Move selectedMove = CheckValid(x, y);
                if (selectedMove != null)
                {
                    sMove = selectedMove;
                    MovePiece();

                    if (!multiCapture)
                        Invoke(nameof(CheckWin), 0.1f);
                }
                if (!multiCapture)
                {
                    ClearHighlights();
                    DebugBoard();
                }
            }
            else if (selected == null) //No Pieces are selected
            {
                SelectPiece(x, y);
            }
            else //A Piece is already selected
            {
                selected.Select(false);
                if (SelectPiece(x, y))//If not selecting another Piece
                    return;
                
                Move selectedMove = CheckValid(x, y);
                if (selectedMove != null)
                {
                    sMove = selectedMove;
                    MovePiece();

                    //Wait 0.1 sec second and check if valid 
                    //might need more testing depending on ping
                    if (!multiCapture)
                        Invoke(nameof(CheckWin), 0.1f);
                }
                if (!multiCapture)
                {
                    ClearHighlights();
                    DebugBoard();
                }
            }
        }
    }

    private Piece[,] GetActivePieces()
    {
        Piece[,] Pieces = new Piece[8, 8];
        Piece p = null;
        foreach (Transform child in transform.Find("Pieces"))
        {
            p = child.GetComponent<Piece>();
            Pieces[p.GetX(), p.GetY()] = p;
        }

        return Pieces;
    }

    private void MovePiece()
    {
        Piece p = selected;
        Move Move = sMove;

        int x = Move.GetX();
        int y = Move.GetY();
        Debug.Log("Moved Piece " + p.GetX() + " " + p.GetY() + " to " + x + " " + y);
        

        ClearHighlights();

        //Delete captured Piece
        Piece capture = Move.GetCapture();
        if (capture != null)
        {
            int cX = capture.GetX();
            int cY = capture.GetY();

            //clear all Moves
            ClearMoves();

            //find additional capture
            FindMultiCapture(x, y, x - cX, y - cY);
        }

        if (multiCapture)
        {
            selected.Select(true);
            DisplayMoves();
        }
        else
        {
            ClearMoves();

            selected.Select(false);
            selected = null;

            HighlightMovable();
        }

    }

    public void ClearMoves()
    {
        Piece[,] Pieces = GetActivePieces();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece tmpP = Pieces[i, j];
                if (tmpP != null)
                    tmpP.ClearMoves();
            }
        }
    }

    
    public void ChangeTurn()
    {
        if (turn == 1)
        {
            turn = 2;
            playerSelected1.SetActive(false);
            playerSelected2.SetActive(true);
        }
        else
        {
            turn = 1;
            playerSelected1.SetActive(true);
            playerSelected2.SetActive(false);
        }
        
        Debug.Log("Player " + turn + "'s turn / " + ((turn == 1) ? player1Color : player2Color));
        Invoke(nameof(FineMoves), 0.1f);
    }

    //Check if the selected Move is in the list of valid Moves for the selected Piece
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
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f,
            LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int) (hit.point.x - boardOffset.x);
            mouseOver.y = (int) (hit.point.y - boardOffset.y);
        }
        else
        {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }

    //Create all Pieces
    // 
    public void CreateBoard()
    {

        
        FineMoves();
    }

    //Select the Piece return true if a Piece is selected
    private bool SelectPiece(int x, int y)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7)
            return false;
        Piece p = GetActivePieces()[x, y];
        if (p == null) //if the selected square contains a Piece
            return false;
        if (p.GetPlayer() != turn) //if not the player's Piece
            return false;

        ClearHighlights();

        Debug.Log("Selected " + x + " " + y);

        selected = p;
        Debug.Log(selected.GetMovesNum());
        if (selected.GetMovesNum() > 0) //highlight Piece if Move is possible
        {
            selected.Select(true);
            DisplayMoves();
            return true;
        }
        
        selected.Select(false);
        selected = null;
        return false;
    }

    private void CheckWin()
    {
        int p1Count = 0;
        int p1MovesCount = 0;
        int p2Count = 0;
        int p2MovesCount = 0;
        Piece[,] Pieces = GetActivePieces();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece p = Pieces[i, j];
                if (p != null)
                {
                    if (p.GetPlayer() == 1)
                    {
                        p1Count += 1;
                        p1MovesCount += p.GetMovesNum();
                    }
                    else if (p.GetPlayer() == 2)
                    {
                        p2Count += 1;
                        p2MovesCount += p.GetMovesNum();
                    }
                }
            }
        }
        ClearMoves();
    }

    //TODO: Win UI not loading properly if someone force closes a game
    //After a person wins announce to both players 
    
    private void EndGame(int player)
    {
        winGUI.SetActive(true);
        isClickable = false;
        gameCompleted = true;
    }

    private void FindMultiCapture(int x, int y, int dx, int dy)
    {
        multiCapture = false;
        Piece[,] Pieces = GetActivePieces();

        int adjSquareL = CheckSquare(x - 1, y + dy);
        int jumpSquareL = CheckSquare(x - 2, y + (2 * dy));
        int adjSquareR = CheckSquare(x + 1, y + dy);
        int jumpSquareR = CheckSquare(x + 2, y + (2 * dy));

        if (adjSquareL == 1 && jumpSquareL == 0)
        {
            Move mL = CreateMovePrefab("Move");
            mL.MoveObj(x - 2, y + (2 * dy));

            mL.SetPriority(1);
            mL.SetCapture(Pieces[x - 1, y + dy]);

            selected.AddMove(mL);
            multiCapture = true;
        }

        if (adjSquareR == 1 && jumpSquareR == 0)
        {
            Move mR = CreateMovePrefab("Move");
            mR.MoveObj(x + 2, y + (2 * dy));

            mR.SetPriority(1);
            mR.SetCapture(Pieces[x + 1, y + dy]);

            selected.AddMove(mR);
            multiCapture = true;
        }

        if (selected.GetKing())
        {
            int adjSquareB = CheckSquare(x + dx, y - dy);
            int jumpSquareB = CheckSquare(x + (2 * dx), y - (2 * dy));

            if (adjSquareB == 1 && jumpSquareB == 0)
            {
                Move mB = CreateMovePrefab("Move");

                mB.MoveObj(x + 2 * dx, y - (2 * dy));

                mB.SetPriority(1);
                mB.SetCapture(Pieces[x + dx, y - dy]);
                selected.AddMove(mB);
                multiCapture = true;
            }
        }
        HighlightMovable();
    }

    //Display all possible Moves of selected Piece
    private void DisplayMoves()
    {
        List<Move> Moves = selected.GetMoves();
        for (int i = 0; i < selected.GetMovesNum(); i++)
        {
            Move h = CreateMovePrefab("Highlight");
            int x = Moves[i].GetX();
            int y = Moves[i].GetY();
            Piece capture = Moves[i].GetCapture();

            h.MoveObj(x, y);

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

            Destroy(highlights[i].gameObject);
        }

        highlights.Clear();
    }

    //Find all possible Moves for all Pieces
    private void FineMoves()
    {
        priority = 0;
        List<Piece> movablePieces = new List<Piece>();
        Piece[,] Pieces = GetActivePieces();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece p = Pieces[i, j];
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

                //Move forwards
                CheckDirection(p, i, j, dn, up);
                CheckDirection(p, i, j, up, up);

                if (p.GetKing()) //Move backwards if the Piece is a king
                {
                    CheckDirection(p, i, j, dn, dn);
                    CheckDirection(p, i, j, up, dn);
                }

                //If a capture Move is available, keep only capture Moves
                int prio = p.GetPriority();
                if (prio > priority)
                {
                    foreach (Piece Piece in movablePieces)
                        Piece.ClearMoves();

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

    private void HighlightMovable()
    {
        Piece[,] Pieces = GetActivePieces();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece p = Pieces[i, j];
                if (p == null)
                    continue;
                if (p.GetMovesNum() == 0)
                    p.HighlightPiece(false);
                else
                    p.HighlightPiece(true);
            }
        }
    }

    private Move CreateMovePrefab(string c)
    {
        GameObject go;
        switch (c)
        {
            //SHOULD BE LOCAL
            case "Highlight":
                go = Instantiate(highlightPrefab, transform.position, Quaternion.identity);
                go.transform.parent = transform.Find("Moves").transform;
                return go.GetComponent<Move>();
            default:
                go = Instantiate(MovePrefab, transform.position, Quaternion.identity);
                go.transform.parent = transform.Find("Moves").transform;
                return go.GetComponent<Move>();
        }
    }



    private void CheckDirection(Piece p, int x, int y, int dx, int dy)
    {
        Piece[,] Pieces = GetActivePieces();
        Move m = CreateMovePrefab("Move");

        int adjSquare = CheckSquare(x + dx, y + dy);
        int jumpSquare = CheckSquare(x + (2 * dx), y + (2 * dy));


        if (adjSquare == 0) //Move
        {
            // NetSynced Move / Client ?
            // PhotonView pView = m.GetComponent<PhotonView>();
            // pView.RPC("Move", RpcTarget.All,  x + dx, y + dy);

            m.MoveObj(x + dx, y + dy);


            m.SetPriority(0);
            p.AddMove(m);
        }
        else if (adjSquare == 1 && jumpSquare == 0) //Capture
        {
            // NetSynced Move / Client ?
            // PhotonView pView = m.GetComponent<PhotonView>();
            // pView.RPC("Move", RpcTarget.All,  x + 2 * dx, y + 2 * dy);

            m.MoveObj(x + (2 * dx), y + (2 * dy));


            m.SetPriority(1);
            m.SetCapture(Pieces[x + dx, y + dy]);
            p.AddMove(m);
        }
        else //No possible Move
        {
            Destroy(m.gameObject);
        }
    }

    //Check what is on square at (x,y)
    private int CheckSquare(int x, int y)
    {
        Piece[,] Pieces = GetActivePieces();
        if (x < 0 || x > 7 || y < 0 || y > 7) //out of board
            return -1;
        if (Pieces[x, y] == null) //no Piece
            return 0;
        if (Pieces[x, y].GetPlayer() == turn) //player's Piece
            return -1;
        return 1; //opponent's Piece
    }

    //Display the current board layout in console
    private void DebugBoard()
    {
        string str = "";
        Piece[,] Pieces = GetActivePieces();
        for (int j = 7; j >= 0; j--)
        {
            for (int i = 0; i < 8; i++)
            {
                if (Pieces[i, j] != null)
                    str += "P";
                else
                    str += "O";
            }

            str += "\n";
        }

        Debug.Log(str);
    }

}