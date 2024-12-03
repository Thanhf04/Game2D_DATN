using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class JoinGame : MonoBehaviourPunCallbacks
{
    public TMP_InputField code;
    private string maphong;

    public void CreateRoom()
    {
        maphong = Random.Range(100000, 999999).ToString();
        PhotonNetwork.CreateRoom(maphong, new RoomOptions() { MaxPlayers = 2, IsVisible = true, IsOpen = true }, TypedLobby.Default, null);
        Debug.Log(maphong);
    }

    public void JoinRoom()
    {
        if (code.text != "")
        {
            PhotonNetwork.JoinRoom(code.text);
        }
    }

    public override void OnJoinedRoom()
    {
        // Lưu mã phòng vào PlayerPrefs
        PlayerPrefs.SetString("RoomCode", maphong);
        PhotonNetwork.LoadLevel(3);
    }
}
