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

        public void SetUp(Player _player)
        {
            player = _player;
            text.text = _player.NickName;
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if(player == otherPlayer)
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
