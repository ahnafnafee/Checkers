using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject highlightPrefab;
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    public GameObject square;
    public GameObject move;

    private Piece[,] pieces = new Piece[8, 8]; //Grid
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
    // Start is called before the first frame update
    void Start()
    {
        CreateBoard();
        //Set player1 and player2 color
    }

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
                    MovePiece(selected, selectedMove);
            }
            else if (selected == null) //No pieces are selected
            {
                SelectPiece(x, y);
            }
            else //A piece is already selected
            {
                selected.select(false);
                if (!SelectPiece(x, y)) //If not selecting another piece
                {
                    Move selectedMove = CheckValid(x, y);
                    if (selectedMove != null)
                        MovePiece(selected, selectedMove);
                }
            }
            //DebugBoard();
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

    //Create a piece at x,y
    private void CreatePiece(int x, int y, int player)
    {
        Piece p;
        if (player == 1)
            p = CreatePiecePrefab(player1Color);
        else
            p = CreatePiecePrefab(player2Color);
        p.move(x,y);
        p.setPlayer(player);
        pieces[x, y] = p;
    }

    //Select the piece return true if a piece is selected
    private bool SelectPiece(int x, int y)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7)
            return false;
        Piece p = pieces[x, y];
        if (p == null) //if the selected square contains a piece
            return false;
        if (p.getPlayer() != turn) //if not the player's piece
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

    private void winner()
    {
        int p1Count = 0;
        int p1MovesCount = 0;
        int p2Count = 0;
        int p2MovesCount = 0;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece p = pieces[i, j];
                if (p != null)
                {
                    if (p.getPlayer() == 1)
                    {
                        p1Count += 1;
                        p1MovesCount += p.getMovesNum();
                    }
                    else
                    {
                        p2Count += 1;
                        p2MovesCount += p.getMovesNum();
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

    //Move the selected piece to x,y 
    private void MovePiece(Piece p, Move move)
    {
        int x = move.getX();
        int y = move.getY();
        Debug.Log("Moved piece " + p.getX() + " " + p.getY() + " to " + x + " " + y);
        pieces[p.getX(), p.getY()] = null;
        p.move(x, y);
        pieces[x, y] = p;

        ClearHighlights();

        //Delete captured piece
        Piece capture = move.getCapture();
        if (capture != null) { 
            int cX = capture.getX();
            int cY = capture.getY();
            Destroy(capture.gameObject);
            pieces[cX, cY] = null;

            //clear all moves
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Piece tmpP = pieces[i, j];
                    if (tmpP != null)
                        tmpP.clearMoves();
                }
            }
            //find additional capture
            findMultiCapture(x, y, x - cX, y - cY);
        }

        if (multiCapture)
        {
            selected.select(true);
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
                        tmpP.clearMoves();
                }
            }
            selected.select(false);
            selected = null;
            turn = (turn == 1) ? 2 : 1;
            //Display turn change
            Debug.Log("Turn " + turn);

            FindMoves();

            //Check winner
            winner();
        }

        //Promote the piece
        if ((p.getPlayer() == 1 && y == 7) ||
            (p.getPlayer() == 2 && y == 0))
            p.promote();
    }

    private void findMultiCapture(int x, int y, int dx, int dy)
    {
        multiCapture = false;

        int adjSquareL = CheckSquare(x - 1, y + dy);
        int jumpSquareL = CheckSquare(x - 2, y + 2 * dy);
        int adjSquareR = CheckSquare(x + 1, y + dy);
        int jumpSquareR = CheckSquare(x + 2, y + 2 * dy);

        if (adjSquareL == 1 && jumpSquareL == 0)
        {
            Move mL = CreateMovePrefab("Move");
            mL.move(x - 2, y + 2 * dy);
            mL.setPriority(1);
            mL.setCapture(pieces[x - 1, y + dy]);
            selected.addMove(mL);
            multiCapture = true;
        }

        if (adjSquareR == 1 && jumpSquareR == 0)
        {
            Move mR = CreateMovePrefab("Move");
            mR.move(x + 2, y + 2 * dy);
            mR.setPriority(1);
            mR.setCapture(pieces[x + 1, y + dy]);
            selected.addMove(mR);
            multiCapture = true;
        }

        if (selected.getKing())
        {
            int adjSquareB = CheckSquare(x + dx, y - dy);
            int jumpSquareB = CheckSquare(x + 2 * dx, y - 2 * dy);

            if (adjSquareB == 1 && jumpSquareB == 0)
            {
                Move mB = CreateMovePrefab("Move");
                mB.move(x + 2 * dx, y - 2 * dy);
                mB.setPriority(1);
                mB.setCapture(pieces[x + dx, y - dy]);
                selected.addMove(mB);
                multiCapture = true;
            }
        }
    }

    //Display all possible moves of selected piece
    private void DisplayMoves()
    {
        List<Move> moves = selected.getMoves();
        for (int i = 0; i < selected.getMovesNum(); i++)
        {
            Move h = CreateMovePrefab("Highlight");
            int x = moves[i].getX();
            int y = moves[i].getY();
            Piece capture = moves[i].getCapture();
            h.move(x,y);
            h.setCapture(capture);
            highlights.Add(h);
        }
    }

    //Clear highlighted squares
    private void ClearHighlights()
    {
        for (int i = 0; i < highlights.Count; i++)
            Destroy(highlights[i].gameObject);
        highlights.Clear();
    }

    //Find all possible moves for all pieces
    private void FindMoves()
    {
        int priority = 0;
        List<Piece> movablePieces = new List<Piece>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece p = pieces[i, j];
                if (p == null)
                    continue;
                p.clearMoves();

                int player = p.getPlayer();
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
                CheckDirection(p, i, j, dn, up);
                CheckDirection(p, i, j, up, up);

                if (p.getKing()) //move backwards if the piece is a king
                {
                    CheckDirection(p, i, j, dn, dn);
                    CheckDirection(p, i, j, up, dn);
                }

                //If a capture move is available, keep only capture moves
                int prio = p.getPriority();
                if (prio > priority)
                {
                    foreach (Piece piece in movablePieces)
                        piece.clearMoves();

                    movablePieces.Clear();
                    priority = prio;
                }
                if (prio >= priority)
                    movablePieces.Add(p);
                else
                    p.clearMoves();

            }

        }
    }

    private Square CreateSquarePrefab(string c)
    {
        GameObject go = Instantiate(square, transform, true);
        go.transform.parent = transform.Find("TempObjects").transform;
        return go.GetComponent<Square>();
    }
    private Move CreateMovePrefab(string c)
    {
        GameObject go;
        switch (c)
        {
            case "Highlight":
                go = Instantiate(highlightPrefab, transform, true);
                go.transform.parent = transform.Find("Moves").transform;
                return go.GetComponent<Move>();
            default:
                go = Instantiate(move, transform, true);
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
                go = Instantiate(whitePiecePrefab, transform, true);
                return go.GetComponent<Piece>();
            default:
                go = Instantiate(blackPiecePrefab, transform, true);
                return go.GetComponent<Piece>();
        }
    }

    private void CheckDirection(Piece p, int x, int y, int dx, int dy)
    {
        Move m = CreateMovePrefab("Move");
        int adjSquare = CheckSquare(x + dx, y + dy);
        int jumpSquare = CheckSquare(x + 2 * dx, y + 2 * dy);


        if (adjSquare == 0) //Move
        {
            m.move(x + dx, y + dy);
            m.setPriority(0);
            p.addMove(m);
        }
        else if (adjSquare == 1 && jumpSquare == 0) //Capture
        {
            m.move(x + 2 * dx, y + 2 * dy);
            m.setPriority(1);
            m.setCapture(pieces[x + dx, y + dy]);
            p.addMove(m);
        }
        else //No possible move
        {
            Destroy(m.gameObject);
        }
    }

    //Check what is on square at (x,y)
    private int CheckSquare(int x, int y)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7) //out of board
            return -1;
        if (pieces[x, y] == null) //no piece
            return 0;
        if (pieces[x, y].getPlayer() == turn) //player's piece
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
