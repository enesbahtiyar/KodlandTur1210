using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.PlasticSCM.Editor;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text chatText;
    [SerializeField] TMP_InputField inputText;
    [SerializeField] GameObject startButton;

    private void Start()
    {
        if(PlayerPrefs.HasKey("Winner") && PhotonNetwork.IsMasterClient)
        {
            string winner = PlayerPrefs.GetString("Winner");
            photonView.RPC("ShowMessage", RpcTarget.All, "Kazanan Kişi " + winner);
            PlayerPrefs.DeleteAll();
        }


        if(!PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(false);
        }
    }


    public void Send()
    {
        if (string.IsNullOrEmpty(inputText.text))
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            photonView.RPC("ShowMessage", RpcTarget.All, PhotonNetwork.NickName + ": " + inputText.text);

            inputText.text = string.Empty;
        }
    }

    [PunRPC]
    void ShowMessage(string message)
    {
        //yazıyı sonraki satıra geçireceğim
        chatText.text += "\n";
        //mesajı yazdır
        chatText.text += message;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        ShowMessage(otherPlayer.NickName + " has left the room");
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        ShowMessage(newPlayer.NickName + " has joined the room");
    }
}
