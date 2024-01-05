using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Weapon : MonoBehaviourPunCallbacks
{
    [SerializeField] AudioSource shoot;
    [SerializeField] AudioClip bulletSound, noBulletSound, reload;

    //şarjordeki mermi sayısı
    protected int ammoCurrent;
    //şarjorün maksimum kapasitesi
    protected int ammoMax;
    //yedekte tutabileceğim mermi mıktarı
    protected int ammoBackPack;
    //güncellenecek ui texti
    [SerializeField] TMP_Text ammoText;

    //merminin çarptığı yerde iz bırakacak bir nesne
    [SerializeField] protected GameObject particle;
    //kamerayı tutmamız lazım
    [SerializeField] protected GameObject cam;
    //silahın modu
    protected bool isAuto = false;
    //atışlar arasındaki aralık ve süreyi zamanlama
    protected float coolDown = 0f;
    protected float timer = 0f;

    //ilk atış gecikmesiz olması
    private void Start()
    {
        timer = coolDown;
    }

    private void Update()
    {
        if(photonView.IsMine)
        {
            timer += Time.deltaTime;
            if (Input.GetMouseButton(0))
            {
                Shoot();
            }
            AmmoTextUpdate();

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (ammoCurrent != ammoMax || ammoBackPack != 0)
                {
                    shoot.PlayOneShot(reload);
                    Invoke("Reload", 1f);
                }
            }
        }
    }

    public void Shoot()
    {
        if(Input.GetMouseButtonDown(0) || isAuto)
        {
            if(timer >= coolDown)
            {
                if(ammoCurrent > 0)
                {
                    timer = 0f;
                    OnShoot();
                    ammoCurrent--;
                    shoot.PlayOneShot(bulletSound);
                    shoot.pitch = Random.Range(1f, 1.5f);
                }
                else
                {
                    if(!shoot.isPlaying)
                    {
                        shoot.PlayOneShot(noBulletSound);
                    }
                }
            }
        }
    }

    private void AmmoTextUpdate()
    {
        ammoText.text = ammoCurrent + "/" + ammoBackPack;
    }

    private void Reload()
    {
        int ammoNeed = ammoMax - ammoCurrent;
        if (ammoBackPack >= ammoNeed)
        {
            //ihtiyaç duyduğumuz mermiyi backpackten çıkardım 
            ammoBackPack -= ammoNeed;
            //şarjöre gerekli mermiyi ekle
            ammoCurrent += ammoNeed;
        }
        //eğer ihtiyacım olan mermi sayısı envanterimdeki mermi sayısından çoksa envanterimdeki bütün mermileri şarjöre koy envanteri sıfırla
        else
        {
            ammoCurrent += ammoBackPack;
            ammoBackPack = 0;
        }
    }

    //bu çocuklar tarafından override edilerek belirlenecek
    protected virtual void OnShoot()
    {

    }
}
