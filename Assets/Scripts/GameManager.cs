using System.Collections;
using Lobby.Scripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks, IInRoomCallbacks
{

    // Update is called once per frame
    void Update()
    {
        if ((Input.anyKey) && SceneManager.GetActiveScene().buildIndex == 0)
        {
            SceneManager.LoadSceneAsync(1);
        }
    }

    public void RestartGame()
    {
        PhotonNetwork.DestroyAll();
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void LoadLobby()
    {
        StartCoroutine(DisconnectAndLoad());
    }
    
    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            Debug.Log($"Within While Loop: {PhotonNetwork.IsConnected}");
            yield return null;
        }

        SceneManager.LoadScene(1);
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