using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
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
    }

    // Update is called once per frame
    void Update()
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
        if (p != null) //if the selected square contains a piece
        {
            if (p.getSide() == "player") //if the player owns the piece
            {
                ClearHighlights();
                selected = p;

                Debug.Log("Selected " + x + " " + y);

                FindMoves();

                if (highlights.Count > 0) //highlight piece if move is possible
                {
                    selected.select(true);
                    return true;
                }
                else //deselect piece if piece has no possible moves
                {
                    selected.select(false);
                    selected = null;
                    return false;
                }
            }
        }
        return false;
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

        pieces[p.getX(), p.getY()] = null;
        p.setX(x);
        p.setY(y);
        pieces[x, y] = p;
        selected = null;

        bool multiCapture = false;
        List<Square> captures = move.getCaptures();
        for (int i = 0; i < captures.Count; i++)
        {
            int cX = captures[i].getX();
            int cY = captures[i].getY();
            Destroy(pieces[cX, cY].gameObject);
            multiCapture = true;
        }
        /*if (multiCapture)
        {
            SelectPiece(x, y);
        }*/


        MoveGameObject(p, x, y);
    }

    private void MoveGameObject(Square go, int x, int y)
    {
        go.transform.position = (Vector2.right * x) + (Vector2.up * y) + boardOffset + pieceOffset;
    }


    //Display all possible moves of selected piece
    private void DisplayMoves()
    {
        Debug.Log("Number of moves " + highlights.Count);
        for (int i = 0; i < highlights.Count; i++)
        {
            CreateHighlight(highlights[i].getX(), highlights[i].getY());
        }
    }

    //Create highlighted squares at x,y
    private void CreateHighlight(int x, int y)
    {
        GameObject go;
        go = Instantiate(highlightPrefab, transform, true);
        Move h = go.GetComponent<Move>();
        h.setX(x);
        h.setY(y);

        highlights.Add(h);

        MoveGameObject(h, x, y);
    }

    //Create highlighted squares at x,y
    private void CreateHighlight2(int x, int y, int captureX, int captureY)
    {
        GameObject go;
        go = Instantiate(highlightPrefab, transform, true);
        Move h = go.GetComponent<Move>();
        h.setX(x);
        h.setY(y);
        h.addCapture(pieces[captureX, captureY]);
        highlights.Add(h);

        MoveGameObject(h, x, y);
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


    //Find all possible moves for the selected piece
    private void FindMoves() //Change to accept one piece
    {
        if (selected != null)
        {
            int x = selected.getX();
            int y = selected.getY();

            int UR = 1;
            int DL = -1;


            //////////////////////////
            //REWRITE THIS PART
            ///////////////////////
            //move forwards
            //Up Left
            if (CheckSquare(x - 1, y + UR) == 0)
                CreateHighlight(x - 1, y + 1);
            else if (CheckSquare(x - 1, y + 1) == 1 && CheckSquare(x - 2, y + 2) == 0)
                CreateHighlight2(x - 2, y + 2, x - 1, y + 1);

            //Up Right
            if (CheckSquare(x + 1, y + 1) == 0)
                CreateHighlight(x + 1, y + 1);
            else if (CheckSquare(x + 1, y + 1) == 1 && CheckSquare(x + 2, y + 2) == 0)
                CreateHighlight2(x + 2, y + 2, x + 1, y + 1);


            if (selected.getKing())
            {
                //move backwards if the piece is a king
                if (CheckSquare(x - 1, y - 1) == 0)
                    CreateHighlight(x - 1, y - 1);
                else if (CheckSquare(x - 1, y - 1) == 1 && CheckSquare(x - 2, y - 2) == 0)
                    CreateHighlight2(x - 2, y - 2, x - 1, y - 1);

                if (CheckSquare(x + 1, y - 1) == 0)
                    CreateHighlight(x + 1, y - 1);
                else if (CheckSquare(x + 1, y - 1) == 1 && CheckSquare(x + 2, y - 2) == 0)
                    CreateHighlight2(x + 2, y - 2, x + 1, y - 1);
            }


            //Remove moves with lower prioriy
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
