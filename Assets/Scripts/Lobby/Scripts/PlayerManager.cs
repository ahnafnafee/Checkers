using System.IO;
using Photon.Pun;
using UnityEngine;

namespace Lobby.Scripts
{
    public class PlayerManager : MonoBehaviour
    {
        PhotonView pv;

        void Awake()
        {
            pv = GetComponent<PhotonView>();
        }

        void Start()
        {
            if(pv.IsMine)
            {
                CreateController();
            }
        }

        void CreateController()
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), Vector3.zero, Quaternion.identity);
        }
    }
}
