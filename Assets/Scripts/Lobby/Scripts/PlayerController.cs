using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
	PhotonView PV;
	private GameObject board;

	void Awake()
	{
		PV = GetComponent<PhotonView>();
	}

	void Start()
	{
		if (PV.IsMine)
		{
			board = GameObject.FindWithTag("Board");
			Debug.Log(board);

			Debug.Log("Me Authorized");
		}
	}

	void Update()
	{
		if(!PV.IsMine)
			return;

		
	}

	void FixedUpdate()
	{
		if(!PV.IsMine)
			return;

	}
	
	
	
	// COPIED FROM BOARD.CS
	
	
	
	
	
	
	
}
