using Photon.Pun;
using UnityEngine;

namespace Lobby.Scripts
{
	public class PlayerController : MonoBehaviourPunCallbacks
	{
		PhotonView PV;
		private Board board;
		private PhotonView pvBoard;
		private int count = 0;

		void Awake()
		{
			PV = GetComponent<PhotonView>();
		}

		void Start()
		{
			if (PV.IsMine && PhotonNetwork.IsMasterClient)
			{
				Debug.Log("Server Master");
				
				GameObject go = GameObject.FindWithTag("Board");
				board = go.GetComponent<Board>();
				pvBoard = go.GetComponent<PhotonView>();
				
				pvBoard.RPC("CreateBoard", RpcTarget.All);
			}
			
			// GameObject go2 = GameObject.FindWithTag("Board");
			// pvBoard = go2.GetComponent<PhotonView>();
		}

		void Update()
		{
			if(!PV.IsMine)
				return;

			// pvBoard.RPC("RpcUpdateBoard", RpcTarget.All);
		
		}

		void FixedUpdate()
		{
			if(!PV.IsMine)
				return;

		}
	
	
	
	
	
	
	
	}
}
