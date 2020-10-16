using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private Token[,] tokens = new Token[8, 8];
    private Token selected = null;
    public GameObject highlightPrefab;
    public GameObject whiteTokenPrefab;
    public GameObject blackTokenPrefab;

    private List<Move> highlights = new List<Move>();

    private Vector2 boardOffset = new Vector2(-4.0f, -4.0f);
    private Vector2 tokenOffset = new Vector2(0.5f, 0.5f);

    private Vector2 mouseOver;

    // Start is called before the first frame update
    void Start()
    {
        createBoard();
    }

    // Update is called once per frame
    void Update()
    {
        updateMouseOver();

        int x = (int)mouseOver.x;
        int y = (int)mouseOver.y;
        
        if (Input.GetMouseButtonDown(0))
        {
            if (selected == null) {
                if (SelectToken(x, y))
                {
                    Debug.Log("Selected " + x.ToString() + " " + y.ToString());
                    if (selected.getMovesNum() > 0)
                    {
                        selected.select(true);
                        displayMoves();
                    }
                }
            }
            else
            {
                selected.select(false);
                if (SelectToken(x, y))
                {
                    Debug.Log("Selected " + x.ToString() + " " + y.ToString());
                    if (selected.getMovesNum() > 0)
                    {
                        selected.select(true);
                        displayMoves();
                    }
                }
                else
                { 
                Token t = tokens[selected.getX(), selected.getY()];
                
                
                if (checkValid(x, y))
                {
                    moveToken(t, x, y);
                    if (y == 7)
                    {
                        t.promote();
                    }
                    findMoves();
                    Debug.Log("Moved token");
                }



                selected = null;
                clearHighlights();
                }
            }
            debugBoard();
        }

    }

    //Check if the selected move is in the list of valid moves for the selected token
    private bool checkValid(int x, int y)
    {
        for (int i = 0; i< highlights.Count; i++)
        {
            if( highlights[i].getX() == x && highlights[i].getY() == y)
                return true;
        }
        return false;
    }

    //Get mouse location
    private void updateMouseOver()
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

    //Create all tokens
    private void createBoard()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 8; x += 2)
            {
                if (y % 2 == 1)
                    createToken(x + 1, y);
                else
                    createToken(x, y);
            }
        }
        findMoves();
    }

    //Create a token at x,y
    private void createToken(int x, int y)
    {
        GameObject go;
        go = Instantiate(whiteTokenPrefab) as GameObject;
        go.transform.SetParent(transform);
        Token t = go.GetComponent<Token>();
        t.setX(x);
        t.setY(y);
        t.setSide("player");
        tokens[x, y] = t;

        t.transform.position = (Vector2.right * x) + (Vector2.up * y) + boardOffset + tokenOffset;
    }

    //Select the token return true if a token is selected
    private bool SelectToken(int x, int y)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7)
            return false;
        Token t = tokens[x, y];
        if (t != null)
        {
            if (t.getSide() == "player")
            {
                clearHighlights();
                selected = t;
                return true;
            }
        }
        return false;
    }

    //Move the selected token to x,y
    private void moveToken(Token t, int x, int y)
    {
        t.transform.position = (Vector2.right * x) + (Vector2.up * y) + boardOffset + tokenOffset;

        tokens[selected.getX(), selected.getY()] = null; 

        selected.setX(x);
        selected.setY(y);
        tokens[x, y] = selected;
        selected = null;
    }


    //Display all possible moves of selected token
    private void displayMoves()
    {
        List<Move> moves = selected.getMoves();
        Debug.Log("Number of moves "+moves.Count);
        for (int i = 0; i< moves.Count; i++)
        {
            createHighlight(moves[i].getX(), moves[i].getY());
        }
    }

    //Create highlighted tiles at x,y
    private void createHighlight(int x, int y)
    {
        GameObject go;
        go = Instantiate(highlightPrefab) as GameObject;
        go.transform.SetParent(transform);
        Move h = go.GetComponent<Move>();
        h.setX(x);
        h.setY(y);
        
        highlights.Add(h);

        h.transform.position = (Vector2.right * x) + (Vector2.up * y) + boardOffset + tokenOffset;
    }

    //Clear highlighted tiles
    private void clearHighlights()
    {
        for (int i = 0; i<highlights.Count; i++)
        {
            Destroy(highlights[i].gameObject);
        }
        highlights.Clear();
    }


    //Find all possible moves for all token and store in the class
    private void findMoves()
    {
        for (int i = 0; i<8; i++)
        {
            for (int j = 0; j<8; j++)
            {
                Token t = tokens[i, j];
                if (t != null)
                {
                    t.clearMoves();
                    if (t.getKing())
                    {
                        //move backwards if the token is a king
                        if (checkMove(i - 1, j - 1) == 0)
                        {
                            t.addMove(new Move(i - 1, j - 1));
                        }
                        if (checkMove(i + 1, j - 1) == 0)
                        {
                            t.addMove(new Move(i + 1, j - 1));
                        }
                    }

                    //move forwards
                    if (checkMove(i - 1, j + 1) == 0)
                    {
                        t.addMove(new Move(i - 1, j + 1));
                    }
                    if (checkMove(i + 1, j + 1) == 0)
                    {
                        t.addMove(new Move(i + 1, j + 1));
                    }

                }
            }
        }
    }

    //Check if token is able to be moved to x,y
    //Improve this to account for capturing pieces and multiple captures
    private int checkMove(int x, int y)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7)
            return -1;
        else if (tokens[x,y] != null)
        {
            if (tokens[x, y].getSide() == "opponent")
            {
                return 1;
            }
            return -1;
        }
        return 0;
    }

    //Display the current board layout in console
    private void debugBoard()
    {
        string str = "";
        for (int j = 7; j >= 0; j--) 
        {
            for (int i = 0; i < 8; i++)
            {
                if (tokens[i, j] != null)
                    str += "T";
                else
                    str += "O";
            }
            str += "\n";
        }
        Debug.Log(str);
    }

}
