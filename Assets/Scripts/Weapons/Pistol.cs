using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    private void Start()
    {
        coolDown = 0.1f;
        isAuto = false;
        //başlangıçta şarjoründe 6 tane mermi var 
        ammoCurrent = 6;
        //şarjöründe maksimum 6 tane mermi olabilir
        ammoMax = 6;
        //yedek olarak maksimumum 36 tane mermi taşıyabilirsin
        ammoBackPack = 36;
    }

    protected override void OnShoot()
    {
        Vector3 rayStartPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = cam.GetComponent<Camera>().ScreenPointToRay(rayStartPosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            GameObject bullet = Instantiate(particle, hit.point, hit.transform.rotation);
            Destroy(bullet, 1f);
            if (hit.collider.CompareTag("enemy"))
            {
                hit.collider.gameObject.GetComponent<Enemy>().GetDamage(10);
            }
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.gameObject.GetComponent<PlayerController>().GetDamage(10);
            }
        }
    }
}
