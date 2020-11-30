using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby.Scripts
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        public static RoomManager Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        public override void OnEnable()
        {
            if (SceneManager.GetActiveScene().buildIndex >= 2)
            {
                base.OnEnable();
                SceneManager.sceneLoaded += OnSceneLoaded;
            }

        }

        public override void OnDisable()
        {
            if (SceneManager.GetActiveScene().buildIndex >= 2)
            {
                base.OnDisable();
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.buildIndex == 2)
            {
                // Debug.Log("Instantiated PlayerPrefab");
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero,
                    Quaternion.identity);
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            Debug.Log(otherPlayer.NickName + " has left the game");
        }
    }
}