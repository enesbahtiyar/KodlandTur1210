using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGun : Weapon
{
    private void Start()
    {
        coolDown = 0.1f;
        isAuto = true;
        ammoCurrent = 100;
        ammoMax = 100;
        ammoBackPack = 200;
    }

    protected override void OnShoot()
    {
        Vector3 rayStartPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Vector3 drift = new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), Random.Range(-30, 30));

        Ray ray = cam.GetComponent<Camera>().ScreenPointToRay(rayStartPosition + drift);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject bullet = Instantiate(particle, hit.point, hit.transform.rotation);
            Destroy(bullet, 1f);

            if (hit.collider.CompareTag("enemy"))
            {
                hit.collider.gameObject.GetComponent<Enemy>().GetDamage(10);
            }
            if(hit.collider.CompareTag("Player"))
            {
                hit.collider.gameObject.GetComponent<PlayerController>().GetDamage(10);
            }
        }
    }
}
