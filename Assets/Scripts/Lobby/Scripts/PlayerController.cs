using Photon.Pun;
using UnityEngine;

namespace Lobby.Scripts
{
	public class PlayerController : MonoBehaviourPunCallbacks
	{
		PhotonView pv;
		private PhotonView pvBoard;

		void Awake()
		{
			pv = GetComponent<PhotonView>();
		}
	}
}
