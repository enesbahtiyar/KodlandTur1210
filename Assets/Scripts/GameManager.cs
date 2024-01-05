using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] List<Transform> playerSpawns = new List<Transform>();
    [SerializeField] List<Transform> walkSpawns = new List<Transform>();
    [SerializeField] List<Transform> turretSpawns = new List<Transform>();
    //oyuncu sayısını tutacak olan text objesine referans
    [SerializeField] public TMP_Text playersText;
    //oyuncuları tutacağımız depo
    GameObject[] players;
    //aktif oyuncular ölmeyenler
    List<string> activePlayers = new List<string>();
    int checkPlayer = 0;

    int randSpawn;
    private int previousPlayerCount;


    private void Start()
    {
        randSpawn = Random.Range(0, playerSpawns.Count);
        PhotonNetwork.Instantiate("Player", playerSpawns[randSpawn].position, playerSpawns[randSpawn].rotation);
        previousPlayerCount = PhotonNetwork.PlayerList.Length;
        Invoke("SpawnEnemy", 5f);
    }

    private void Update()
    {
        if(PhotonNetwork.PlayerList.Length < previousPlayerCount)
        {
            ChangePlayerList();
        }
        previousPlayerCount = PhotonNetwork.PlayerList.Length;
    }


    public void ChangePlayerList()
    {
        photonView.RPC("PlayerList", RpcTarget.All);
    }

    [PunRPC]
    public void PlayerList()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        activePlayers.Clear();

        foreach(GameObject player in players)
        {
            if(player.GetComponent<PlayerController>().dead == false)
            {
                activePlayers.Add(player.GetComponent<PhotonView>().Owner.NickName);
            }
        }

        playersText.text = "Players in game: " + activePlayers.Count.ToString();

        if(activePlayers.Count <= 1 && checkPlayer > 0)
        {
            PlayerPrefs.SetString("Winner", activePlayers[0]);
            var enemies = GameObject.FindGameObjectsWithTag("enemy");

            foreach(GameObject enemy in enemies)
            {
                enemy.GetComponent<Enemy>().ChangeHealth(10000);
            }
            Invoke("EndGame", 5f);
        }

        checkPlayer++;
    }


    public void SpawnEnemy()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            for(int i = 0; i < walkSpawns.Count; i++)
            {
                PhotonNetwork.Instantiate("WalkEnemy", walkSpawns[i].position, walkSpawns[i].rotation);
            }

            for (int i = 0; i < turretSpawns.Count; i++)
            {
                PhotonNetwork.Instantiate("Turret", turretSpawns[i].position, turretSpawns[i].rotation);
            }
        }
    }

    void EndGame()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }

    public void ExitGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
        ChangePlayerList();
    }

}
