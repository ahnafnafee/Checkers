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
        [SerializeField] TMP_Text errorText;
        [SerializeField] Transform playerListContent;
        [SerializeField] GameObject playerListItemPrefab;
        [SerializeField] Transform roomListContent;
        [SerializeField] GameObject roomListItemPrefab;

        [SerializeField] TMP_InputField roomNameInputField;
        [SerializeField] TMP_Text roomNameText;
        [SerializeField] GameObject startGameButton;

        void Awake()
        {
            // Ensures there is only 1 Launcher
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        public void Start()
        {
            // Debug.Log($"Network: {PhotonNetwork.IsConnected}");
            // Debug.Log($"Room: {PhotonNetwork.InRoom}");
            if (PhotonNetwork.IsConnected == false)
            {
                // Debug.Log("Connected to Server");
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnConnectedToMaster()
        {
            // Debug.Log("Connected to Master");

            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override void OnJoinedLobby()
        {
            MenuManager.Instance.OpenMenu("title");
            // Debug.Log("Joined Lobby");
            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
        }

        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(roomNameInputField.text))
            {
                return;
            }

            // Room attributes
            RoomOptions roomOps = new RoomOptions {IsVisible = true, IsOpen = true, MaxPlayers = 2};

            PhotonNetwork.CreateRoom(roomNameInputField.text, roomOps);
            MenuManager.Instance.OpenMenu("loading");
        }

        public override void OnJoinedRoom()
        {
            MenuManager.Instance.OpenMenu("room");
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;

            RefreshPlayerList();

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
            MenuManager.Instance.OpenMenu("loading");
            PhotonNetwork.LoadLevel(2);
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
            foreach (Transform trans in roomListContent)
            {
                Destroy(trans.gameObject);
            }
            

            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].RemovedFromList)
                    continue;

                if (roomList[i].PlayerCount == 2)
                    continue;

                Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            Debug.Log(otherPlayer.NickName + " has left the game");

            RefreshPlayerList();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer, 1);

            RefreshPlayerList();
        }
        
        private bool IsPlayer1()
        {
            //Debug.Log(PhotonNetwork.LocalPlayer);
            if (PhotonNetwork.PlayerList.GetValue(0).Equals(PhotonNetwork.LocalPlayer))
                return true;
            return false;
        }

        private void RefreshPlayerList()
        {
            Player[] players = PhotonNetwork.PlayerList;
            
            foreach (Transform child in playerListContent)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < players.Count(); i++)
            {
                
                PlayerListItem pItem = Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>();
                pItem.SetUp(players[i], i + 1);
                
                if (i == 0)
                {
                    pItem.MakeLeader();
                }
                
                if (IsPlayer1() && i == 0)
                {
                    pItem.IsMe();
                } 
                else if (!IsPlayer1() && i == 1)
                {
                    pItem.IsMe();
                }
            }
        }
    }
}