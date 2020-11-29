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
            Assert.IsTrue(b.SelectPiece(2, 2));
        }
        
                
        [Test]
        public void BoardSelectInvalidNoMoves()
        {
            b = boardObject.AddComponent<Board>();
            b.Start();
            Assert.IsFalse(b.SelectPiece(0, 0));
        }
        
        [Test]
        public void BoardSelectInvalidEmpty()
        {
            b = boardObject.AddComponent<Board>();
            b.Start();
            Assert.IsFalse(b.SelectPiece(0, 1));
        }
        
        [Test]
        public void BoardSelectInvalidEnemy()
        {
            b = boardObject.AddComponent<Board>();
            b.Start();
            Assert.IsFalse(b.SelectPiece(7, 5));
        }
        
        [Test]
        public void BoardSelectInvalidOutOfBounds()
        {
            b = boardObject.AddComponent<Board>();
            b.Start();
            Assert.IsFalse(b.SelectPiece(10, 10));
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
        public void BoardMoveSingleCapture()
        {
            b = boardObject.AddComponent<Board>();
            b.Start();

            b.SetBoard("001");
            
            b.UpdateMouseOver(0, 0);
            b.Click();
            b.Update();
            

        }
    }
}
