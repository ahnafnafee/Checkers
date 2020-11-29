using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using NUnit.Framework;
using Tests.TestingScripts;
using UnityEngine;
using UnityEngine.TestTools;


namespace Tests
{
    public class BoardTest
    {
        private GameObject boardObject;
        private Board b;
        
        //Setup empty board
        [SetUp]
        public void SetUpTest()
        {
            boardObject = new GameObject();
            boardObject.name = "Board";
            GameObject moves = new GameObject();
            moves.name = "Moves";
            moves.transform.SetParent(boardObject.transform);
        }
        
        //Remove everything from board after each test
        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(boardObject);
        }

        [Test]
        public void BoardGenerate()
        {
            b = boardObject.AddComponent<Board>();
            b.Start();
            foreach (Transform child in boardObject.transform)
            {
                if (child.name.Equals("Piece"))
                {
                    Piece p = child.GetComponent<Piece>();
                    var x = p.GetX();
                    var y = p.GetY();
                    //Assert x and y are on the board
                    Assert.True(x >= 0 && x <= 7);
                    Assert.True(y >= 0 && y <= 7);
                }
            }
        }
        
        [Test]
        public void BoardSelectValid()
        {
            b = boardObject.AddComponent<Board>();
            b.Start();
                        
            b.UpdateMouseOver(2, 2);
            b.Click();
            b.Update();

            var pieces = b.GetActivePieces();
            Assert.IsTrue(pieces[2,2].transform.Find("selected").gameObject.activeInHierarchy);
        }
        
                
        [Test]
        public void BoardSelectInvalidNoMoves()
        {
            b = boardObject.AddComponent<Board>();
            b.Start();
            
            b.UpdateMouseOver(0, 0);
            b.Click();
            b.Update();

            var pieces = b.GetActivePieces();
            foreach (var p in pieces)
            {
                if (p != null)
                    Assert.IsFalse(p.transform.Find("selected").gameObject.activeInHierarchy);
            }
        }
        
        [Test]
        public void BoardSelectInvalidEmpty()
        {
            b = boardObject.AddComponent<Board>();
            b.Start();
            
            b.UpdateMouseOver(0, 1);
            b.Click();
            b.Update();

            var pieces = b.GetActivePieces();
            foreach (var p in pieces)
            {
                if (p != null)
                    Assert.IsFalse(p.transform.Find("selected").gameObject.activeInHierarchy);
            }
        }
        
        [Test]
        public void BoardSelectInvalidEnemy()
        {
            b = boardObject.AddComponent<Board>();
            b.Start();
            
            b.UpdateMouseOver(7, 5);
            b.Click();
            b.Update();

            var pieces = b.GetActivePieces();
            foreach (var p in pieces)
            {
                if (p != null)
                    Assert.IsFalse(p.transform.Find("selected").gameObject.activeInHierarchy);
            }
        }
        
        [Test]
        public void BoardSelectInvalidOutOfBounds()
        {
            b = boardObject.AddComponent<Board>();
            b.Start();
            
            b.UpdateMouseOver(10, 10);
            b.Click();
            b.Update();

            var pieces = b.GetActivePieces();
            foreach (var p in pieces)
            {
                if (p != null)
                    Assert.IsFalse(p.transform.Find("selected").gameObject.activeInHierarchy);
            }
        }
        
        [Test]
        public void BoardSelectAnotherValid()
        {
            b = boardObject.AddComponent<Board>();
            b.Start();
                        
            b.UpdateMouseOver(2, 2);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(4, 2);
            b.Click();
            b.Update();

            var pieces = b.GetActivePieces();
            Assert.IsFalse(pieces[2,2].transform.Find("selected").gameObject.activeInHierarchy);
            Assert.IsTrue(pieces[4,2].transform.Find("selected").gameObject.activeInHierarchy);
        }
        
        [Test]
        public void BoardMoveBasic()
        {
            b = boardObject.AddComponent<Board>();
            b.Start();
            //Select piece at 2,2
            b.UpdateMouseOver(2, 2);
            b.Click();
            b.Update();
            
            //Select highlighted move at 3,3
            b.UpdateMouseOver(3, 3);
            b.Click();
            b.Update();
            
            //Assert if piece moved
            var pieces = b.GetActivePieces();
           
            Assert.IsNull(pieces[2,2]);
            Assert.IsNotNull(pieces[3,3]);
        }
        
        [Test]
        public void BoardMoveInvalid()
        {
            b = boardObject.AddComponent<Board>();
            b.Start();
            //Select piece at 2,2
            b.UpdateMouseOver(2, 2);
            b.Click();
            b.Update();
            
            //Select highlighted move at 3,3
            b.UpdateMouseOver(9, 9);
            b.Click();
            b.Update();
            
            //Assert if piece moved
            var pieces = b.GetActivePieces();
           
            Assert.IsNotNull(pieces[2,2]);
        }
        
        [Test]
        public void BoardMoveBasicOpponent()
        {
            b = boardObject.AddComponent<Board>();
            b.Start();
            //Select piece at 2,2
            b.UpdateMouseOver(2, 2);
            b.Click();
            b.Update();
            
            //Select highlighted move at 3,3
            b.UpdateMouseOver(3, 3);
            b.Click();
            b.Update();
            
            //Select piece at 1,5
            b.UpdateMouseOver(1, 5);
            b.Click();
            b.Update();
            
            //Select highlighted move at 2,4
            b.UpdateMouseOver(2, 4);
            b.Click();
            b.Update();
            
            //Assert if piece moved
            var pieces = b.GetActivePieces();
            
            Assert.IsNull(pieces[1,5]);
            Assert.IsNotNull(pieces[2,4]);
        }


        [Test]
        public void BoardMoveSingleCaptureRight()
        {
            b = boardObject.AddComponent<Board>();

            b.SetBoard("001-112");
            
            b.UpdateMouseOver(0, 0);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(2, 2);
            b.Click();
            b.Update();

            var pieces = b.GetActivePieces();
            
            Assert.IsNull(pieces[0,0]);
            Assert.IsNull(pieces[1,1]);
            Assert.IsNotNull(pieces[2,2]);
        }
        
        [Test]
        public void BoardMoveSingleCaptureLeft()
        {
            b = boardObject.AddComponent<Board>();

            b.SetBoard("201-112");
            
            b.UpdateMouseOver(2, 0);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(0, 2);
            b.Click();
            b.Update();

            var pieces = b.GetActivePieces();
            
            Assert.IsNull(pieces[2,0]);
            Assert.IsNull(pieces[1,1]);
            Assert.IsNotNull(pieces[0,2]);
        }
        
        [Test]
        public void BoardMovePromote()
        {
            b = boardObject.AddComponent<Board>();

            b.SetBoard("661");
            
            b.UpdateMouseOver(6, 6);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(7, 7);
            b.Click();
            b.Update();

            var pieces = b.GetActivePieces();
            
            Assert.IsNull(pieces[6,6]);
            Assert.IsNotNull(pieces[7,7]);
            Assert.IsTrue(pieces[7,7].GetKing());
        }
        
        [Test]
        public void BoardMovePromoteOpponent()
        {
            b = boardObject.AddComponent<Board>();

            b.SetBoard("661-112");
            
            b.UpdateMouseOver(6, 6);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(7, 7);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(1, 1);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(0, 0);
            b.Click();
            b.Update();
            
            var pieces = b.GetActivePieces();
            
            Assert.IsNull(pieces[1,1]);
            Assert.IsNotNull(pieces[0,0]);
            Assert.IsTrue(pieces[0,0].GetKing());
        }
        
        [Test]
        public void BoardPlayerOneWin()
        {
            b = boardObject.AddComponent<Board>();

            b.SetBoard("001-112");
            
            b.UpdateMouseOver(0, 0);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(2, 2);
            b.Click();
            b.Update();

            var pieces = b.GetActivePieces();
            
            Assert.IsNull(pieces[0,0]);
            Assert.IsNull(pieces[1,1]);
            Assert.IsNotNull(pieces[2,2]);
            Assert.AreEqual("Player 1",b.GetWinner());
        }
        
        [Test]
        public void BoardPlayerTwoWin()
        {
            b = boardObject.AddComponent<Board>();

            b.SetBoard("001-222");
            
            b.UpdateMouseOver(0, 0);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(1, 1);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(2, 2);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(0, 0);
            b.Click();
            b.Update();
            
            var pieces = b.GetActivePieces();
            
            Assert.IsNull(pieces[2,2]);
            Assert.IsNull(pieces[1,1]);
            Assert.IsNotNull(pieces[0,0]);
            Assert.AreEqual("Player 2",b.GetWinner());
        }
        
        [Test]
        public void BoardClickAfterWin()
        {
            b = boardObject.AddComponent<Board>();

            b.SetBoard("001-112");
            
            b.UpdateMouseOver(0, 0);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(2, 2);
            b.Click();
            b.Update();

            var pieces = b.GetActivePieces();
            
            b.UpdateMouseOver(2, 2);
            b.Click();
            b.Update();
            
            Assert.IsNull(pieces[0,0]);
            Assert.IsNull(pieces[1,1]);
            Assert.IsNotNull(pieces[2,2]);
            Assert.AreEqual("Player 1",b.GetWinner());
        }
        
        [Test]
        public void BoardMoveKingCapture()
        {
            b = boardObject.AddComponent<Board>();

            b.SetBoard("661-372");
            
            b.UpdateMouseOver(6, 6);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(5, 7);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(3, 7);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(4, 6);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(5, 7);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(3, 5);
            b.Click();
            b.Update();
            
            var pieces = b.GetActivePieces();
            
            Assert.IsNull(pieces[4,6]);
            Assert.IsNull(pieces[5,7]);
            Assert.IsNotNull(pieces[3,5]);
        }
        
        
        [Test]
        public void BoardMoveMultiCaptureRight()
        {
            b = boardObject.AddComponent<Board>();

            b.SetBoard("001-112-332");
            
            b.UpdateMouseOver(0, 0);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(2, 2);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(4, 4);
            b.Click();
            b.Update();

            var pieces = b.GetActivePieces();
            
            Assert.IsNull(pieces[1,1]);
            Assert.IsNull(pieces[3,3]);
            Assert.IsNull(pieces[0,0]);
            Assert.IsNotNull(pieces[4,4]);
        }
        
        [Test]
        public void BoardMoveMultiCaptureLeft()
        {
            b = boardObject.AddComponent<Board>();

            b.SetBoard("601-512-332");
            
            b.UpdateMouseOver(6, 0);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(4, 2);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(2, 4);
            b.Click();
            b.Update();

            var pieces = b.GetActivePieces();
            
            Assert.IsNull(pieces[5,1]);
            Assert.IsNull(pieces[3,3]);
            Assert.IsNull(pieces[6,0]);
            Assert.IsNotNull(pieces[2,4]);
        }
        
        [Test]
        public void BoardMoveKingMultiCaptureDiamond()
        {
            b = boardObject.AddComponent<Board>();

            b.SetBoard("661-772-462-442-642");
            
            b.UpdateMouseOver(6, 6);
            b.Click();
            b.Update();
            b.UpdateMouseOver(5, 7);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(7, 7);
            b.Click();
            b.Update();
            b.UpdateMouseOver(6, 6);
            b.Click();
            b.Update();
            
            b.UpdateMouseOver(5, 7);
            b.Click();
            b.Update();
            b.UpdateMouseOver(3, 5);
            b.Click();
            b.Update();
            b.UpdateMouseOver(5, 3);
            b.Click();
            b.Update();
            b.UpdateMouseOver(7, 5);
            b.Click();
            b.Update();
            b.UpdateMouseOver(5, 7);
            b.Click();
            b.Update();

            var pieces = b.GetActivePieces();
            
            Assert.IsNull(pieces[4,6]);
            Assert.IsNull(pieces[4,4]);
            Assert.IsNull(pieces[6,4]);
            Assert.IsNull(pieces[6,6]);
            Assert.IsNotNull(pieces[5,7]);
        }
    }
}
