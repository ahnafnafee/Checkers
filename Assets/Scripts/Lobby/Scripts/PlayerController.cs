using Photon.Pun;
using UnityEngine;

namespace Lobby.Scripts
{
	public class PlayerController : MonoBehaviourPunCallbacks
	{
		PhotonView PV;
		private Board board;
		private PhotonView pvBoard;

		void Awake()
		{
			PV = GetComponent<PhotonView>();
		}
	}
}
