using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Lobby.Scripts
{
    public class PlayerListItem : MonoBehaviourPunCallbacks
    {
        [SerializeField] TMP_Text text;
        [SerializeField] GameObject partyLeader;
        [SerializeField] GameObject youText;
        Player player;
        private int pNum;
        

        private void Awake()
        {
            partyLeader.SetActive(false);
            youText.SetActive(false);
        }

        public void SetUp(Player p, int playerNum)
        {
            player = p;
            text.text = "Player " + playerNum;
            pNum = playerNum;
        }

        public void MakeLeader()
        {
            partyLeader.SetActive(true);
        }

        public void IsMe()
        {
            youText.SetActive(true);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if(Equals(player, otherPlayer))
            {
                Destroy(gameObject);
            }
        }

        public override void OnLeftRoom()
        {
            Destroy(gameObject);
        }
    }
}
