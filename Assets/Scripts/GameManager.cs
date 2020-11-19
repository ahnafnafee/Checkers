using System.Collections;
using Lobby.Scripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.anyKey) && SceneManager.GetActiveScene().buildIndex == 0)
        {
            SceneManager.LoadSceneAsync(1);
        }
    }

    IEnumerator DoSwitchLevel (int level)
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }

        SceneManager.LoadSceneAsync(level);
    }

    public void LoadLobby()
    {
        // StartCoroutine(DisconnectAndLoad());
        
        
        // PhotonNetwork.LoadLevel(1);
        
        PhotonNetwork.LeaveRoom();
        
        // PhotonNetwork.LeaveRoom();
        // PhotonNetwork.LeaveLobby();
        // PhotonNetwork.Disconnect();
    }
    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    // IEnumerator DisconnectAndLoad()
    // {
    //     PhotonNetwork.LeaveRoom();
    //     
    //
    //     while (PhotonNetwork.InRoom && PhotonNetwork.IsConnected)
    //     {
    //         Debug.Log("Disconnecting");
    //         yield return null;
    //     }
    //     SceneManager.LoadScene(1);
    //     
    // }

    // public override void OnDisconnected(DisconnectCause cause)
    // {
    //     Debug.Log(cause);
    //     SceneManager.LoadSceneAsync(1);
    // }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadSceneAsync(1);
        // SceneManager.LoadScene(1);
        // base.OnLeftRoom();
    }
    

    public void RestartScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().path);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}