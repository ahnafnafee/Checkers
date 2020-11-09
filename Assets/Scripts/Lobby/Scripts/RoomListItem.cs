using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Lobby.Scripts
{
    public class RoomListItem : MonoBehaviour
    {
        [SerializeField] TMP_Text text;

        public RoomInfo info;

        public void SetUp(RoomInfo _info)
        {
            this.info = _info;
            text.text = _info.Name;
        }

        public void OnClick()
        {
            Launcher.Instance.JoinRoom(info);
        }
    }
}
