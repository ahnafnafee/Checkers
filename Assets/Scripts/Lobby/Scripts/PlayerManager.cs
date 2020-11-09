using System.IO;
using Photon.Pun;
using UnityEngine;

namespace Lobby.Scripts
{
    public class PlayerManager : MonoBehaviour
    {
        PhotonView PV;

        void Awake()
        {
            PV = GetComponent<PhotonView>();
        }

        void Start()
        {
            if(PV.IsMine)
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
