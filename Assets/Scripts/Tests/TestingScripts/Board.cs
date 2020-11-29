using System.Collections.Generic;
using UnityEngine;

namespace Tests.TestingScripts
{
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
        private bool click = false;
        


        [Header("Board Attributes")] 

        private int turn = 1; //1 = player 1; 2 = player 2
        private Vector2 boardOffset = new Vector2(-4.0f, -4.0f);
        private bool gameCompleted = default;
        private string winner = "";
    

        public void Start()
        {
            // Default conditions
            isClickable = true;
            gameCompleted = false;
        
            CreateBoard();
        }

        public void Update()
        {
            //UpdateMouseOver();

            // For clicking Pieces
            ClickPiece();
        }



        private void ClickPiece()
        {
            if (!isClickable)
                return;
        
            int x = (int) mouseOver.x;
            int y = (int) mouseOver.y;

            if (click)
            {
                click = false;
                //DebugBoard();
                
            
                if (multiCapture)
                {
                    Move selectedMove = CheckValid(x, y);
                    if (selectedMove != null)
                    {
                        sMove = selectedMove;
                        MovePiece();

                    }
                    if (!multiCapture)
                    {
                        ClearHighlights();
                        //DebugBoard();
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


                    }
                    if (!multiCapture)
                    {
                        ClearHighlights();
                        //DebugBoard();
                    }
                }
            }
        }

        public void UpdateMouseOver(int x, int y)
        {
            mouseOver.x = x;
            mouseOver.y = y;
        }
        
        public void Click()
        {
            click = true;
        }
        
        public Piece[,] GetActivePieces()
        {
            Piece[,] Pieces = new Piece[8, 8];
            Piece p = null;
            foreach (Transform child in transform)
            {
                if (child.name.Equals("Piece"))
                {
                    p = child.GetComponent<Piece>();
                    Pieces[p.GetX(), p.GetY()] = p;
                }
            }

            return Pieces;
        }

        private void MovePiece()
        {
            Piece p = selected;
            Move Move = sMove;

            int x = Move.GetX();
            int y = Move.GetY();
            Debug.Log("Moved piece " + p.GetX() + " " + p.GetY() + " to " + x + " " + y);
            
            p.Move(x,y);

            ClearHighlights();

            //Delete captured Piece
            Piece capture = Move.GetCapture();
            if (capture != null)
            {
                int cX = capture.GetX();
                int cY = capture.GetY();

                capture.DestroyPiece();
                
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
                
                ChangeTurn();
            }

            //Promote the piece
            if ((p.GetPlayer() == 1 && y == 7) ||
                (p.GetPlayer() == 2 && y == 0))
                p.Promote();
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

    
        private void ChangeTurn()
        {
            if (turn == 1)
                turn = 2;
            else
                turn = 1;
            
            Debug.Log("Player " + turn + "'s turn / " + ((turn == 1) ? player1Color : player2Color));
            FindMoves();
            CheckWin();

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

        //Create all Pieces
        private void CreateBoard()
        {
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
        
        public void SetBoard(string posStr)
        {
            isClickable = true;
            gameCompleted = false;
            
            var posArr = posStr.Split('-');

            foreach (var pos in posArr)
            {
                var c = pos.ToCharArray();
                CreatePiece(int.Parse(c[0].ToString()), int.Parse(c[1].ToString()), int.Parse(c[2].ToString()));
            }
            
            FindMoves();
        }

        private void CreatePiece(int x, int y, int player)
        {
            Piece p;
            if (player == 1)
                p = CreatePiecePrefab(player1Color);
            else
                p = CreatePiecePrefab(player2Color);
            p.SetPlayer(player);
            p.Move(x,y);
        }
    
        //Select the Piece return true if a Piece is selected
        public bool SelectPiece(int x, int y)
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
            Debug.Log("Number of moves: " + selected.GetMovesNum());
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
            //if no pieces left or no available moves
            if (p2Count == 0 || (p2MovesCount == 0 && turn == 2)) //p1 win
                EndGame(1);
            else if (p1Count == 0 || (p1MovesCount == 0 && turn == 1)) //p2 win
                EndGame(2);
        }

        //TODO: Win UI not loading properly if someone force closes a game
        //After a person wins announce to both players 
    
        private void EndGame(int player)
        {
            isClickable = false;
            gameCompleted = true;

            if (player == 1)
                winner = "Player 1";
            else
                winner = "Player 2";
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

                DestroyImmediate(highlights[i].gameObject);
            }

            highlights.Clear();
        }

        //Find all possible Moves for all Pieces
        private void FindMoves()
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
            var move = new GameObject();
            move.transform.parent = transform.Find("Moves").transform;
            switch (c)
            {
                //SHOULD BE LOCAL
                case "Highlight":
                    move.name = "Highlight";
                    break;
                default:
                    move.name = "Move";
                    break;
            }
            var m = move.AddComponent<Move>();

            return m;
        }

        private Piece CreatePiecePrefab(string c)
        {
            var piece = new GameObject();
            piece.transform.parent = transform;

            piece.name = "Piece";
            var p = piece.AddComponent<Piece>();
        
            var select = new GameObject {name = "selected"};
            select.SetActive(false);
            select.transform.SetParent(piece.transform);
            var crown = new GameObject {name = "crown"};
            crown.SetActive(false);
            crown.transform.SetParent(piece.transform);
            var movable = new GameObject {name = "movable"};
            movable.SetActive(false);
            movable.transform.SetParent(piece.transform);
        
            return p;
        }

        private void CheckDirection(Piece p, int x, int y, int dx, int dy)
        {
            var Pieces = GetActivePieces();
            var m = CreateMovePrefab("Move");

            var adjSquare = CheckSquare(x + dx, y + dy);
            var jumpSquare = CheckSquare(x + (2 * dx), y + (2 * dy));


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
                DestroyImmediate(m.gameObject);
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
        /*private void DebugBoard()
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
        }*/


        public string GetWinner()
        {
            return winner;
        }
    }
}