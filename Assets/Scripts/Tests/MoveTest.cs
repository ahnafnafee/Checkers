using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Tests.TestingScripts;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MoveTest
    {
        private GameObject moveObject = new GameObject();
        private Move m;
        
        [SetUp]
        public void SetUpTest()
        {
            moveObject.name = "move";
        }

        [Test]
        public void MovePosition()
        {
            m = moveObject.AddComponent<Move>();
            m.SetVal(1,2);
            Assert.AreEqual(1, m.GetX());
            Assert.AreEqual(2, m.GetY());
        }
        
        [Test]
        public void MovePositionMove1()
        {
            m = moveObject.AddComponent<Move>();
            m.SetVal(1,2);
            m.MoveObj(3,4);
            Assert.AreEqual(3, m.GetX());
            Assert.AreEqual(4, m.GetY());
        }
        
        [Test]
        public void MovePositionMove2()
        {
            m = moveObject.AddComponent<Move>();
            m.SetVal(1,2);
            m.MoveObj(6,7);
            Assert.AreEqual(6, m.GetX());
            Assert.AreEqual(7, m.GetY());
        }

        [Test]
        public void MovePriorityZero()
        {
            m = moveObject.AddComponent<Move>();
            m.SetPriority(0);
            Assert.AreEqual(0, m.GetPriority());
        }
        
        [Test]
        public void MovePriorityOne()
        {
            m = moveObject.AddComponent<Move>();
            m.SetPriority(1);
            Assert.AreEqual(1, m.GetPriority());
        }
        
        [Test]
        public void PieceCapture()
        {
            m = moveObject.AddComponent<Move>();
            m.SetVal(2,2);
            var pieceObjectOpponent = new GameObject();
            var pOpponent = pieceObjectOpponent.AddComponent<Piece>();
            pOpponent.SetVal(3,3);
            m.SetCapture(pOpponent);
            
            
            Assert.AreEqual(pOpponent, m.GetCapture());
        }
        
     
    }
}
