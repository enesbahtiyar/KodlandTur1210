using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text logText;
    [SerializeField] TMP_InputField inputText;

    void Log(string message)
    {
        //yazıyı sonraki satıra geçireceğim
        logText.text += "\n";
        //mesajı yazdır
        logText.text += message;
    }


    private void Start()
    {
        //rastgele nickname
        PhotonNetwork.NickName = "Player" + Random.Range(0, 999);
        //bu oyuncunun adını log kısmına yazdıralım 
        Log("Player Name: " + PhotonNetwork.NickName);
        //oyun ayarları
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "0.1.0.23.42532.75.1";
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        if (!string.IsNullOrEmpty(inputText.text))
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                PhotonNetwork.NickName = inputText.text;
                Log("Player Name: " + PhotonNetwork.NickName);
            }
        }
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 15 });
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnConnectedToMaster()
    {
        Log("Connected to server");
    }

    public override void OnJoinedRoom()
    {
        Log("Joined Lobby");
        PhotonNetwork.LoadLevel("Lobby");
    }
}
