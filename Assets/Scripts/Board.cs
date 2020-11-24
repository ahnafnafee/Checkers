using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviourPunCallbacks
{
    public static Board Instance;

    [Header("Instantiable Prefabs")] 
    public GameObject blackPiecePrefab = default;
    public GameObject whitePiecePrefab = default;
    public GameObject highlightPrefab = default;
    public GameObject movePrefab = default;

    [Header("Game Manager")] 
    private GameManager gm;
    private Vector2 mouseOver = default;
    private List<Move> highlights = new List<Move>(); //List of all possible moves

    [Header("Piece Attributes")]
    private bool isClickable = true;
    public GameObject square = default;
    private Piece tPiece = default;
    private Piece selected = default; //Selected piece (null if none selected)
    private Move sMove = default;
    bool multiCapture = false;
    private Vector2 pieceOffset = new Vector2(0.5f, 0.5f);

    [Header("Player Attributes")]
    //Change player color
    private string player1Color = "Dark";
    private string player2Color = "Light";
    private int priority = default;
    private string p1ID = default;
    private string p2ID = default;


    [Header("Board Attributes")] 
    private PhotonView pv = default;
    private int turn = 1; //1 = player 1; 2 = player 2
    private Vector2 boardOffset = new Vector2(-4.0f, -4.0f);
    private bool gameCompleted = default;

    [Header("GUI")] 
    public GameObject winGUI = default;
    public GameObject restartBtn = default;
    public GameObject playerSelected1 = default;
    public GameObject playerSelected2 = default;
    public TextMeshProUGUI winText = default;


    void Awake()
    {
        Instance = this;
        pv = GetComponent<PhotonView>();
    }

    void Start()
    {
        // Default conditions
        isClickable = true;
        gameCompleted = false;
        playerSelected1.SetActive(true);
        playerSelected2.SetActive(false);

        CreateBoard();
        if (!IsPlayer1())
            transform.localRotation = Quaternion.Euler(0, 0, 180);
        //Set player1 and player2 color
        winGUI.SetActive(false);
        
        Debug.Log(PhotonNetwork.LocalPlayer);
    }

    void Update()
    {
        // Enforces player to go back to lobby
        // if player count is less or equal to 11
        CheckPlayerNum();

        UpdateMouseOver();

        // For debugging wins
        DebugWin();
        // For clicking pieces
        ClickPiece();
    }

    //Returns true if player is the first player in the playerlist
    private bool IsPlayer1()
    {
        //Debug.Log(PhotonNetwork.LocalPlayer);
        if (PhotonNetwork.PlayerList.GetValue(0).Equals(PhotonNetwork.LocalPlayer))
            return true;
        return false;
    }

    private void CheckPlayerNum()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount <= 1 && !gameCompleted)
            {
                pv.RPC("EndGame", RpcTarget.All, (IsPlayer1() ? 1 : 2));
                Debug.Log(PhotonNetwork.LocalPlayer + " Player:" + (IsPlayer1() ? 1 : 2));
                PhotonNetwork.CurrentRoom.IsVisible = false;
                restartBtn.SetActive(false);
            } 
            else if (PhotonNetwork.CurrentRoom.PlayerCount <= 1 && gameCompleted)
            {
                PhotonNetwork.CurrentRoom.IsVisible = false;
                restartBtn.SetActive(false);
            }

            if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
            {
                restartBtn.SetActive(true);
            }
        }
        
    }


    public void RestartGame()
    {
        PhotonNetwork.DestroyAll();
        pv.RPC("RpcRestart", RpcTarget.All);
    }

    [PunRPC]
    private void RpcRestart()
    {
        winGUI.SetActive(false);
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex);
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
            if ((IsPlayer1() ? 1 : 2) != turn)
                return;
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
            else if (selected == null) //No pieces are selected
            {
                SelectPiece(x, y);
            }
            else //A piece is already selected
            {
                selected.Select(false);
                if (SelectPiece(x, y))//If not selecting another piece
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
        Piece[,] pieces = new Piece[8, 8];
        Piece p = null;
        foreach (Transform child in transform.Find("Pieces"))
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

        int x = move.GetX();
        int y = move.GetY();
        Debug.Log("Moved piece " + p.GetX() + " " + p.GetY() + " to " + x + " " + y);

        // NetSynced Move
        PhotonView pView = p.GetComponent<PhotonView>();
        pView.RPC("Move", RpcTarget.All, x, y);

        ClearHighlights();

        //Delete captured piece
        Piece capture = move.GetCapture();
        if (capture != null)
        {
            int cX = capture.GetX();
            int cY = capture.GetY();

            pView = capture.GetComponent<PhotonView>();
            pView.RPC("DestroyPiece", RpcTarget.All);

            //clear all moves
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
            pv.RPC("ChangeTurn", RpcTarget.All);
        }

        //Promote the piece
        if ((p.GetPlayer() == 1 && y == 7) ||
            (p.GetPlayer() == 2 && y == 0))
            p.GetComponent<PhotonView>().RPC("Promote", RpcTarget.All);

    }

    public void ClearMoves()
    {
        Piece[,] pieces = GetActivePieces();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece tmpP = pieces[i, j];
                if (tmpP != null)
                    tmpP.ClearMoves();
            }
        }
    }

    [PunRPC]
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
        Invoke(nameof(FindMoves), 0.1f);
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
            //Multi capture front
            /*CreatePiece(1, 1, 1);
            CreatePiece(3, 1, 1);
            CreatePiece(2, 2, 2);
            CreatePiece(4, 4, 2);
            CreatePiece(6, 6, 2);
            CreatePiece(5, 7, 2);*/

            //multi capture king
            /*CreatePiece(6, 6, 1);
            CreatePiece(1, 1, 2);
            CreatePiece(3, 3, 2);
            CreatePiece(5, 5, 2);
            CreatePiece(1, 3, 2);
            CreatePiece(1, 5, 2);
            CreatePiece(3, 5, 2);
            CreatePiece(7, 5, 2);
            Piece[,] pieces = GetActivePieces();
            pieces[6, 6].GetComponent<PhotonView>().RPC("Promote", RpcTarget.All);*/

            //promote mid multi capture
            /*CreatePiece(1, 1, 1);
            CreatePiece(3, 1, 1);
            CreatePiece(2, 2, 2);
            CreatePiece(4, 4, 2);
            CreatePiece(6, 6, 2);
            CreatePiece(2, 4, 2);
            CreatePiece(4, 6, 2);*/
        }
        else
        {
            restartBtn.SetActive(false);
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

        // NetSynced Move
        PhotonView pView = p.GetComponent<PhotonView>();
        pView.RPC("Move", RpcTarget.All, x, y);

        // p.Move(x,y);

        pView.RPC("SetPlayer", RpcTarget.All, player);
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
        Debug.Log(selected.GetMovesNum());
        if (selected.GetMovesNum() > 0) //highlight piece if move is possible
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
                    else if (p.GetPlayer() == 2)
                    {
                        p2Count += 1;
                        p2MovesCount += p.GetMovesNum();
                    }
                }
            }
        }

        //if no pieces left or no available moves
        if (p2Count == 0 || (p2MovesCount == 0 && turn == 2)) //p1 win
            pv.RPC("EndGame", RpcTarget.All, 1);
        else if (p1Count == 0 || (p1MovesCount == 0 && turn == 1)) //p2 win
            pv.RPC("EndGame", RpcTarget.All, 2);

        ClearMoves();
    }

    void DebugWin()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            pv.RPC("EndGame", RpcTarget.All, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            pv.RPC("EndGame", RpcTarget.All, 2);
        }
    }

    //After a person wins announce to both players 
    [PunRPC]
    private void EndGame(int player)
    {
        winGUI.SetActive(true);
        isClickable = false;
        gameCompleted = true;
        
        winText.text = "Player " + player + " (" + ((player == 1) ? player1Color : player2Color) + ") wins";
        Debug.Log("P" + player + " won / " + ((player == 1) ? player1Color : player2Color));
    }

    private void FindMultiCapture(int x, int y, int dx, int dy)
    {
        multiCapture = false;
        Piece[,] pieces = GetActivePieces();

        int adjSquareL = CheckSquare(x - 1, y + dy);
        int jumpSquareL = CheckSquare(x - 2, y + (2 * dy));
        int adjSquareR = CheckSquare(x + 1, y + dy);
        int jumpSquareR = CheckSquare(x + 2, y + (2 * dy));

        if (adjSquareL == 1 && jumpSquareL == 0)
        {
            Move mL = CreateMovePrefab("Move");
            mL.MoveObj(x - 2, y + (2 * dy));

            mL.SetPriority(1);
            mL.SetCapture(pieces[x - 1, y + dy]);

            selected.AddMove(mL);
            multiCapture = true;
        }

        if (adjSquareR == 1 && jumpSquareR == 0)
        {
            Move mR = CreateMovePrefab("Move");
            mR.MoveObj(x + 2, y + (2 * dy));

            mR.SetPriority(1);
            mR.SetCapture(pieces[x + 1, y + dy]);

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
                mB.SetCapture(pieces[x + dx, y - dy]);
                selected.AddMove(mB);
                multiCapture = true;
            }
        }
        HighlightMovable();
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

    //Find all possible moves for all pieces
    private void FindMoves()
    {
        priority = 0;
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
                CheckDirection(p, i, j, dn, up);
                CheckDirection(p, i, j, up, up);

                if (p.GetKing()) //move backwards if the piece is a king
                {
                    CheckDirection(p, i, j, dn, dn);
                    CheckDirection(p, i, j, up, dn);
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
        if ((IsPlayer1() ? 1 : 2) == turn)
            HighlightMovable(); 
    }

    private void HighlightMovable()
    {
        Piece[,] pieces = GetActivePieces();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece p = pieces[i, j];
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
                go = Instantiate(movePrefab, transform.position, Quaternion.identity);
                go.transform.parent = transform.Find("Moves").transform;
                return go.GetComponent<Move>();
        }
    }

    private Piece CreatePiecePrefab(string c)
    {
        GameObject go;
        PhotonView pView;
        switch (c)
        {
            //SHOULD BE GLOBAL
            case "Light":
                go = PhotonNetwork.Instantiate("LightPiece", transform.position, Quaternion.identity);
                pView = go.GetComponent<PhotonView>();
                pView.RPC("SetParent", RpcTarget.All, pv.ViewID);
                return go.GetComponent<Piece>();
            default:
                go = PhotonNetwork.Instantiate("DarkPiece", transform.position, Quaternion.identity);
                pView = go.GetComponent<PhotonView>();
                pView.RPC("SetParent", RpcTarget.All, pv.ViewID);
                return go.GetComponent<Piece>();
        }
    }

    private void CheckDirection(Piece p, int x, int y, int dx, int dy)
    {
        Piece[,] pieces = GetActivePieces();
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
            m.SetCapture(pieces[x + dx, y + dy]);
            p.AddMove(m);
        }
        else //No possible move
        {
            Destroy(m.gameObject);
        }
    }

    //Check what is on square at (x,y)
    private int CheckSquare(int x, int y)
    {
        Piece[,] pieces = GetActivePieces();
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