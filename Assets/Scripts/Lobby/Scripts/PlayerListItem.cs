using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Lobby.Scripts
{
    public class PlayerListItem : MonoBehaviourPunCallbacks
    {
        [SerializeField] TMP_Text text = default;
        Player player;

        public void SetUp(Player p, int playerNum)
        {
            player = p;
            text.text = "Player " + playerNum;
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
