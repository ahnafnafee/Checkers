using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using System.Linq;

namespace Lobby.Scripts
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        public static Launcher Instance;
        
        [SerializeField] TMP_InputField roomNameInputField;
        [SerializeField] TMP_Text errorText;
        [SerializeField] TMP_Text roomNameText;
        [SerializeField] Transform roomListContent;
        [SerializeField] GameObject roomListItemPrefab;
        [SerializeField] Transform playerListContent;
        [SerializeField] GameObject playerListItemPrefab;
        [SerializeField] GameObject startGameButton;
        
        void Awake()
        {
            Instance = this;
        }
        
        void Start()
        {
            Debug.Log("Connected to Server");
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Master");
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override void OnJoinedLobby()
        {
            MenuManager.Instance.OpenMenu("title");
            Debug.Log("Joined Lobby");
            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
        }

        public void CreateRoom()
        {
            if(string.IsNullOrEmpty(roomNameInputField.text))
            {
                return;
            }
            PhotonNetwork.CreateRoom(roomNameInputField.text);
            MenuManager.Instance.OpenMenu("loading");
        }
        
        public override void OnJoinedRoom()
        {
            MenuManager.Instance.OpenMenu("room");
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;
            PhotonNetwork.CurrentRoom.MaxPlayers = 2;
            
            Player[] players = PhotonNetwork.PlayerList;

            foreach(Transform child in playerListContent)
            {
                Destroy(child.gameObject);
            }

            for(int i = 0; i < players.Count(); i++)
            {
                Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
            }

            startGameButton.SetActive(PhotonNetwork.IsMasterClient);
            
        }
        
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        }
        
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            errorText.text = "Room Creation Failed: " + message;
            Debug.LogError("Room Creation Failed: " + message);
            MenuManager.Instance.OpenMenu("error");
        }
        
        public void StartGame()
        {
            // if (PhotonNetwork.IsMasterClient)
            // {
            //     PhotonNetwork.LoadLevel(1);
            // }
            
            PhotonNetwork.LoadLevel(1);
        }
        
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            MenuManager.Instance.OpenMenu("loading");
        }
        
        public override void OnLeftRoom()
        {
            MenuManager.Instance.OpenMenu("title");
        }
        
        public void JoinRoom(RoomInfo info)
        {
            PhotonNetwork.JoinRoom(info.Name);
            MenuManager.Instance.OpenMenu("loading");
        }

        
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach(Transform trans in roomListContent)
            {
                Destroy(trans.gameObject);
            }
            
            for(int i = 0; i < roomList.Count; i++)
            {
                if(roomList[i].RemovedFromList)
                    continue;
                Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
            }
        }
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
        }


    }
}
