using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class TextUpdate : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] TMP_Text playerNickname;
    int health = 100;


    private void Start()
    {
        if(photonView.IsMine)
        {
            playerNickname.text = photonView.Controller.NickName + "\n" + "Health: " + health.ToString();
            photonView.RPC("RotateName", RpcTarget.Others);
        }
    }

    [PunRPC]
    public void RotateName()
    {
        playerNickname.GetComponent<RectTransform>().localScale = new Vector3(-1, 1, 1);
    }

    public void SetHealth(int newHealth)
    {
        //can değerini güncelledik
        health = newHealth;
        //ui yazısını güncelleyelim
        playerNickname.text = photonView.Controller.NickName + "\n" + "Health: " + health.ToString();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (int)stream.ReceiveNext();
            playerNickname.text = photonView.Controller.NickName + "\n" + "Health: " + health.ToString();
        }
    }
}
