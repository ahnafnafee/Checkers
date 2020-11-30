using System;
using System.Collections;
using Lobby.Scripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if ((Input.anyKey) && SceneManager.GetActiveScene().buildIndex == 0)
        {
            SceneManager.LoadScene(1);
        }
    }

    public void RestartGame()
    {
        PhotonNetwork.DestroyAll();
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadLobby()
    {
        Destroy(RoomManager.Instance.gameObject);
        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
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