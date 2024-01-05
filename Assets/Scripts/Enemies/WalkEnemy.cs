using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkEnemy : Enemy
{
    [SerializeField] float speed;

    [SerializeField] float detectionDistance;

    public override void Move()
    {
        //eğer düşman ve oyuncu arasındaki uzaklık bizim böceğin fark edebileceği menzildeyse 
        //ve düşman ve oyuncu arasındaki uzaklık düşmanın saldırı menzilinden uzak ise

        if(distance < detectionDistance && distance > attackDistance)
        {
            //oyuncuya bak
            transform.LookAt(player.transform);
            //koşma animasyonunu çalıştır
            anim.SetBool("Run", true);
            rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
        }
        else
        {
            anim.SetBool("Run", false);
        }
    }

    public override void Attack()
    {
        timer += Time.deltaTime;

        //eğer oyuncu ile düşman arasındaki mesafe saldırı menzili içerisindeyse ve timer cooldowndan büyükse saldır
        if(distance < attackDistance && timer > cooldown)
        {
            //timer'ı 0'la
            timer = 0;

            //oyuncunun canını azalt
            player.GetComponent<PlayerController>().ChangeHealth(damage);

            //saldırı animasyonunu çalıştır
            anim.SetBool("Attack", true);
        }
        else
        {
            anim.SetBool("Attack", false);
        }
    }
}
