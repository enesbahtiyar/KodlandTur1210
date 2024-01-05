using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Enemy : MonoBehaviourPunCallbacks
{
    [SerializeField] protected int health;
    [SerializeField] protected float attackDistance;
    [SerializeField] protected int damage;
    [SerializeField] protected float cooldown;
    [SerializeField] Image healthBar;

    protected GameObject player;
    protected GameObject[] players;
    protected Animator anim;
    protected Rigidbody rb;

    protected float distance;
    protected float timer;
    bool dead = false;
    float storage;

    public virtual void Move()
    {

    }

    public virtual void Attack()
    {

    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        //player = FindObjectOfType<PlayerController>().gameObject;
        CheckPlayers();
        storage = health;
    }

    void CheckPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        Invoke("CheckPlayers", 3f);
    }

    private void Update()
    {
        //aşağıdaki kod eski kod 
        /*
        distance = Vector3.Distance(this.transform.position, player.transform.position);

        if(!dead)
        {
            Attack();
        }
        */


        //burası yeni hali 

        //kocuman sayı ataması sonsuz
        float closestDistance = Mathf.Infinity;
        foreach(GameObject closestPlayer in players)
        {
            float checkDistance = Vector3.Distance(closestPlayer.transform.position, this.transform.position);
            if(checkDistance < closestDistance)
            {
                if (closestPlayer.GetComponent<PlayerController>().dead == false)
                {
                    player = closestPlayer;
                    closestDistance = checkDistance;
                }
            }
        }

        if(player != null)
        {
            distance = Vector3.Distance(this.transform.position, player.transform.position);
            if(!dead)
            {
                Attack();
            }
        }
    }

    private void FixedUpdate()
    {
        if(!dead && player)
        {
            Move();
        }
    }

    public void GetDamage(int count)
    {
        photonView.RPC("ChangeHealth", RpcTarget.All, count);
    }


    [PunRPC]
    public void ChangeHealth(int count)
    {
        health -= count;
        float fillPercent = health / storage;
        healthBar.fillAmount = fillPercent;


        if(health <= 0)
        {
            dead = true;
            //yaradınq öldüyse colliderını kapatıyoruz çarpmamak için
            GetComponent<Collider>().enabled = false;
            anim.enabled = true;
            //yaradınqın ölüm animasyonunu çalıştır
            anim.SetBool("Die", true);
        }
    }
}
