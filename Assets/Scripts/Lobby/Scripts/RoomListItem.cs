using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Lobby.Scripts
{
    public class RoomListItem : MonoBehaviour
    {
        [SerializeField] TMP_Text text;

        public RoomInfo info;

        public void SetUp(RoomInfo Info)
        {
            this.info = Info;
            text.text = Info.Name;
        }

        public void OnClick()
        {
            Launcher.Instance.JoinRoom(info);
        }
    }
}
