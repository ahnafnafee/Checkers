using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lobby.Scripts;
using NUnit.Framework;
using Photon.Pun;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class LobbyTest
    {
        private GameObject canvasObject;
        private Launcher l;


        [SetUp]
        public void SetUpTest()
        {
            canvasObject = new GameObject();
            canvasObject.name = "Canvas";
        }

        [Test]
        public void ServerConnect()
        {
            l = canvasObject.AddComponent<Launcher>();
            Debug.Log(l);
            Debug.Log("Hmm");

            l.Start();

            Assert.AreEqual(1, 1);
        }
    }
}
