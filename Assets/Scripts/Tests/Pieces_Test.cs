using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class Pieces_Test
    {
        private GameObject pieceObject = new GameObject();
        private Piece p;
        
        [SetUp]
        public void SetUpTest()
        {
            pieceObject.name = "piece";
            var select = new GameObject {name = "selected"};
            select.SetActive(false);
            select.transform.parent = pieceObject.transform;
            var crown = new GameObject {name = "crown"};
            crown.SetActive(false);
            crown.transform.parent = pieceObject.transform;
            var movable = new GameObject {name = "movable"};
            movable.SetActive(false);
            movable.transform.parent = pieceObject.transform;
        }

        [Test]
        public void PiecePosition()
        {
            p = pieceObject.AddComponent<Piece>();
            p.SetVal(1,2);
            Assert.AreEqual(1, p.GetX());
            Assert.AreEqual(2, p.GetY());
        }
        
        [Test]
        public void PiecePositionMove()
        {
            p = pieceObject.AddComponent<Piece>();
            p.SetVal(1,2);
            p.Move(3,4);
            Assert.AreEqual(3, p.GetX());
            Assert.AreEqual(4, p.GetY());
        }
        
        [Test]
        public void PiecePromote()
        {
            p = pieceObject.AddComponent<Piece>();
            Assert.IsFalse(p.GetKing());
            p.Promote();
            Assert.IsTrue(p.transform.Find("crown").gameObject.activeInHierarchy);
            Assert.IsTrue(p.GetKing());
        }
        
        [Test]
        public void PieceMovableTrue()
        {
            p = pieceObject.AddComponent<Piece>();
            p.HighlightPiece(true);
            Assert.IsTrue(p.transform.Find("movable").gameObject.activeInHierarchy);
        }
        
        [Test]
        public void PieceMovableFalse()
        {
            p = pieceObject.AddComponent<Piece>();
            p.HighlightPiece(false);
            Assert.IsFalse(p.transform.Find("movable").gameObject.activeInHierarchy);
        }
        
        [Test]
        public void PieceSelectTrue()
        {
            p = pieceObject.AddComponent<Piece>();
            p.Select(true);
            Assert.IsTrue(p.transform.Find("selected").gameObject.activeInHierarchy);
        }
        
        [Test]
        public void PieceSelectFalse()
        {
            p = pieceObject.AddComponent<Piece>();
            p.Select(false);
            Assert.IsFalse(p.transform.Find("selected").gameObject.activeInHierarchy);
        }
        
        [Test]
        public void PieceDestroy()
        {
            var tempObject = new GameObject();
            var tempPiece = tempObject.AddComponent<Piece>();
            tempObject.name = "tempPiece";
            Assert.IsNotNull(tempObject);
            tempPiece.DestroyPiece();
            Assert.IsTrue(tempObject == null);
        }
        
        [Test]
        public void PieceMoveBasicRight()
        {
            p = pieceObject.AddComponent<Piece>();
            p.SetVal(2,2);
            
            var moveObject = new GameObject();
            var move = moveObject.AddComponent<Move>();
            move.SetPriority(0);
            move.MoveObj(3,3);
            
            p.AddMove(move);
            Assert.AreEqual(0, p.GetPriority());
            Assert.AreEqual(1, p.GetMovesNum());
            Assert.AreEqual(new List<Move>{move}, p.GetMoves());
        }
        
        [Test]
        public void PieceMoveBasicLeft()
        {
            p = pieceObject.AddComponent<Piece>();
            p.SetVal(2,2);
            
            var moveObject = new GameObject();
            var move = moveObject.AddComponent<Move>();
            move.SetPriority(0);
            move.MoveObj(1,3);
            
            p.AddMove(move);
            Assert.AreEqual(0, p.GetPriority());
            Assert.AreEqual(1, p.GetMovesNum());
            Assert.AreEqual(new List<Move>{move}, p.GetMoves());
        }
        
        [Test]
        public void PieceMoveClear()
        {
            p = pieceObject.AddComponent<Piece>();
            p.SetVal(2,2);
            
            var moveObject = new GameObject();
            var move = moveObject.AddComponent<Move>();
            move.SetPriority(0);
            move.MoveObj(1,3);
            
            p.AddMove(move);
            
            p.ClearMoves();
            Assert.AreEqual(0, p.GetMovesNum());
            Assert.AreEqual(new List<Move>{}, p.GetMoves());
        }
        
        [Test]
        public void PieceMoveCaptureRight()
        {
            p = pieceObject.AddComponent<Piece>();
            p.SetVal(2,2);
            
            var pieceObjectOpponent = new GameObject();
            var pOpponent = pieceObjectOpponent.AddComponent<Piece>();
            pOpponent.SetVal(3,3);
            
            var moveObject = new GameObject();
            var move = moveObject.AddComponent<Move>();
            move.SetPriority(1);
            move.MoveObj(4,4);
            move.SetCapture(pOpponent);
            
            p.AddMove(move);
            Assert.AreEqual(1, p.GetPriority());
            Assert.AreEqual(1, p.GetMovesNum());
            Assert.AreEqual(new List<Move>{move}, p.GetMoves());
        }
        
        [Test]
        public void PieceMoveCaptureLeft()
        {
            p = pieceObject.AddComponent<Piece>();
            p.SetVal(2,2);
            
            var pieceObjectOpponent = new GameObject();
            var pOpponent = pieceObjectOpponent.AddComponent<Piece>();
            pOpponent.SetVal(1,3);
            
            var moveObject = new GameObject();
            var move = moveObject.AddComponent<Move>();
            move.SetPriority(1);
            move.MoveObj(0,4);
            move.SetCapture(pOpponent);
            
            p.AddMove(move);
            Assert.AreEqual(1, p.GetPriority());
            Assert.AreEqual(1, p.GetMovesNum());
            Assert.AreEqual(new List<Move>{move}, p.GetMoves());
        }
        
        [Test]
        public void PieceMoveMixedCaptureOverwriteRight()
        {
            p = pieceObject.AddComponent<Piece>();
            p.SetVal(2,2);
            
            var moveObject1 = new GameObject();
            var move1 = moveObject1.AddComponent<Move>();
            move1.SetPriority(0);
            move1.MoveObj(1,3);
            
            var pieceObjectOpponent = new GameObject();
            var pOpponent = pieceObjectOpponent.AddComponent<Piece>();
            pOpponent.SetVal(3,3);
            
            var moveObject2 = new GameObject();
            var move2 = moveObject2.AddComponent<Move>();
            move2.SetPriority(1);
            move2.MoveObj(4,4);
            move2.SetCapture(pOpponent);
            
            p.AddMove(move1);
            Assert.AreEqual(0, p.GetPriority());
            Assert.AreEqual(1, p.GetMovesNum());
            
            p.AddMove(move2);
            Assert.AreEqual(1, p.GetPriority());
            Assert.AreEqual(1, p.GetMovesNum());
            Assert.AreEqual(new List<Move>{move2}, p.GetMoves());
        }
        
        [Test]
        public void PieceMoveMixedCaptureOverwriteLeft()
        {
            p = pieceObject.AddComponent<Piece>();
            p.SetVal(2,2);
            
            var pieceObjectOpponent = new GameObject();
            var pOpponent = pieceObjectOpponent.AddComponent<Piece>();
            pOpponent.SetVal(1,3);
            
            var moveObject1 = new GameObject();
            var move1 = moveObject1.AddComponent<Move>();
            move1.SetPriority(1);
            move1.MoveObj(0,4);
            move1.SetCapture(pOpponent);
            
            var moveObject2 = new GameObject();
            var move2 = moveObject2.AddComponent<Move>();
            move2.SetPriority(0);
            move2.MoveObj(3, 3);

            p.AddMove(move1);
            Assert.AreEqual(1, p.GetPriority());
            Assert.AreEqual(1, p.GetMovesNum());
            
            p.AddMove(move2);
            Assert.AreEqual(1, p.GetPriority());
            Assert.AreEqual(1, p.GetMovesNum());
            Assert.AreEqual(new List<Move>{move1}, p.GetMoves());
        }
        
        [Test]
        public void PieceMoveTwoBasic()
        {
            p = pieceObject.AddComponent<Piece>();
            p.SetVal(2,2);
            
            var moveObject1 = new GameObject();
            var move1 = moveObject1.AddComponent<Move>();
            move1.SetPriority(0);
            move1.MoveObj(3,3);

            var moveObject2 = new GameObject();
            var move2 = moveObject2.AddComponent<Move>();
            move2.SetPriority(0);
            move2.MoveObj(1,3);

            p.AddMove(move1);
            Assert.AreEqual(0, p.GetPriority());
            Assert.AreEqual(1, p.GetMovesNum());
            
            p.AddMove(move2);
            Assert.AreEqual(0, p.GetPriority());
            Assert.AreEqual(2, p.GetMovesNum());
            Assert.AreEqual(new List<Move>{move1, move2}, p.GetMoves());
        }
        
        [Test]
        public void PieceMoveTwoCapture()
        {
            p = pieceObject.AddComponent<Piece>();
            p.SetVal(2,2);
            
            var pieceObjectOpponent1 = new GameObject();
            var pOpponent1 = pieceObjectOpponent1.AddComponent<Piece>();
            pOpponent1.SetVal(1,3);
            
            var moveObject1 = new GameObject();
            var move1 = moveObject1.AddComponent<Move>();
            move1.SetPriority(1);
            move1.MoveObj(0,4);
            move1.SetCapture(pOpponent1);
            
            var pieceObjectOpponent2 = new GameObject();
            var pOpponent2 = pieceObjectOpponent2.AddComponent<Piece>();
            pOpponent2.SetVal(3,3);
            
            var moveObject2 = new GameObject();
            var move2 = moveObject2.AddComponent<Move>();
            move2.SetPriority(1);
            move2.MoveObj(4,4);
            move2.SetCapture(pOpponent2);
            
            p.AddMove(move1);
            Assert.AreEqual(1, p.GetPriority());
            Assert.AreEqual(1, p.GetMovesNum());
            
            p.AddMove(move2);
            Assert.AreEqual(1, p.GetPriority());
            Assert.AreEqual(2, p.GetMovesNum());
            Assert.AreEqual(new List<Move>{move1, move2}, p.GetMoves());
        }
    }
}
