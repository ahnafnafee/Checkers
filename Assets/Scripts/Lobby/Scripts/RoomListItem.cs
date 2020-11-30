using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Lobby.Scripts
{
    public class RoomListItem : MonoBehaviour
    {
        [SerializeField] TMP_Text text;

        private RoomInfo info;

        public void SetUp(RoomInfo rInfo)
        {
            info = rInfo;
            text.text = rInfo.Name;
        }

        public void OnClick()
        {
            Launcher.Instance.JoinRoom(info);
        }
    }
}
