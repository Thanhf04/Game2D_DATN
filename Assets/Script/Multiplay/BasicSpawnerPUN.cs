using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BasicSpawnerPUN : MonoBehaviourPunCallbacks
{
    public Button button;
    private void Start()
    {
        // Kết nối đến Photon
        PhotonNetwork.ConnectUsingSettings();
        button.onClick.AddListener(Join);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    // public override void OnJoinedLobby()
    // {
    //     SceneManager.LoadScene(2);
    // }

    public void Join(){
        SceneManager.LoadScene(2);
    }

}
