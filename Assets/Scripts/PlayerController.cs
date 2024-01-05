using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public enum Weapons
    {
        None,
        Pistol,
        Rifle,
        MiniGun
    }

    Weapons weapons = Weapons.None;

    [SerializeField] Image pistolUi, rifleUi, miniGunUi, cursor;
    [SerializeField] AudioSource characterSounds;
    [SerializeField] AudioClip jump;
    [SerializeField] float jumpForce = 7f;
    [SerializeField] float movementSpeed = 5.0f;
    [SerializeField] float shiftSpeed = 10.0f;
    [SerializeField] GameObject rifle, pistol, miniGun;
    float currentSpeed;
    Rigidbody rb;
    Vector3 direction;
    bool isGrounded = true;
    float stamina = 5f;
    Animator anim;
    bool isRifle, isPistol, isMiniGun;
    int health;
    public bool dead;
    //text update içerisindeki sethealth metoduna erişmek için textupdate kod dosyasına referans sağladık
    TextUpdate textUpdate;
    GameManager gameManager;

    private void Start()
    {
        currentSpeed = movementSpeed;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        textUpdate = GetComponent<TextUpdate>();
        gameManager = FindObjectOfType<GameManager>();
        gameManager.ChangePlayerList();
        health = 100;


        if(!photonView.IsMine)
        {
            //kamerayı oyuncu dışında olanlardan kapatma
            transform.Find("Main Camera").gameObject.SetActive(false);
            transform.Find("Canvas").gameObject.SetActive(false);
            //benim haricimdeki kodları kapat
            this.enabled = false;
        }
    }

    private void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        direction = new Vector3(moveHorizontal, 0, moveVertical);
        direction = transform.TransformDirection(direction);

        if (direction.x != 0 || direction.z != 0)
        {
            anim.SetBool("Run", true);
            if(!characterSounds.isPlaying && isGrounded)
            {
                characterSounds.Play();
            }
        }
        else
        {
            anim.SetBool("Run", false);
            characterSounds.Stop();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            characterSounds.Stop();
            AudioSource.PlayClipAtPoint(jump, transform.position);
            anim.SetBool("Jump", true);
            isGrounded = false;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (stamina > 0)
            {
                stamina -= Time.deltaTime;
                currentSpeed = shiftSpeed;
            }
            else
            {
                currentSpeed = movementSpeed;
            }
        }
        else if (!Input.GetKey(KeyCode.LeftShift))
        {
            stamina += Time.deltaTime;
            currentSpeed = movementSpeed;
        }
        if (stamina > 5)
        {
            stamina = 5;
        }
        else if (stamina < 0)
        {
            stamina = 0;
        }

        if(Input.GetKeyDown(KeyCode.Alpha1) && isPistol)
        {
            //ChooseWeapon(Weapons.Pistol);
            //weapons = Weapons.Pistol;
            photonView.RPC("ChooseWeapon", RpcTarget.All, Weapons.Pistol);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2) && isRifle)
        {
            //ChooseWeapon(Weapons.Rifle);
            //weapons = Weapons.Rifle;
            photonView.RPC("ChooseWeapon", RpcTarget.All, Weapons.Rifle);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3) && isMiniGun)
        {
            //ChooseWeapon(Weapons.MiniGun);
            //weapons = Weapons.MiniGun;
            photonView.RPC("ChooseWeapon", RpcTarget.All, Weapons.MiniGun);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //ChooseWeapon(Weapons.None);
            //weapons = Weapons.None;
            photonView.RPC("ChooseWeapon", RpcTarget.All, Weapons.None);
        }

        Debug.Log(weapons.ToString());
        if(weapons != Weapons.None)
        {
            cursor.enabled = true;
        }
        else
        {
            cursor.enabled = false;
        }
    }


    [PunRPC]
    public void ChooseWeapon(Weapons weapon)
    {
        anim.SetBool("Pistol", weapon == Weapons.Pistol);
        anim.SetBool("Assault", weapon == Weapons.Rifle);
        anim.SetBool("MiniGun", weapon == Weapons.MiniGun);
        anim.SetBool("NoWeapon", weapon == Weapons.None);
        pistol.SetActive(weapon == Weapons.Pistol);
        rifle.SetActive(weapon == Weapons.Rifle);
        miniGun.SetActive(weapon == Weapons.MiniGun);
        weapons = weapon;
    }

    public void GetDamage(int count)
    {
        photonView.RPC("ChangeHealth", RpcTarget.All, count);
    }

    [PunRPC]
    public void ChangeHealth(int count)
    {
        health -= count;
        textUpdate.SetHealth(health);
        if(health <= 0)
        {
            //ölme animasyonu çalıştır
            anim.SetBool("Die", true);
            //elimizdeki silahı bıraktık
            transform.Find("Main Camera").GetComponent<ThirdPersonCamera>().isSpectator = true;
            ChooseWeapon(Weapons.None);
            //bu kod dosyasını kapat
            this.enabled = false;
            dead = true;
            gameManager.ChangePlayerList();
        }
    }



    public void ChooseWeapon(string weapons)
    {
        switch(weapons)
        {
            case "Pistol":
                anim.SetBool("Pistol", true);
                anim.SetBool("Rifle", false);
                anim.SetBool("MiniGun", false);
                anim.SetBool("NoWeapon", false);
                rifle.SetActive(false);
                pistol.SetActive(true);
                miniGun.SetActive(false);
                break;
            case "Rifle":
                anim.SetBool("Pistol", false);
                anim.SetBool("Rifle", true);
                anim.SetBool("MiniGun", false);
                anim.SetBool("NoWeapon", false);
                rifle.SetActive(true);
                pistol.SetActive(false);
                miniGun.SetActive(false);
                break;
            case "MiniGun":
                anim.SetBool("Pistol", false);
                anim.SetBool("Rifle", false);
                anim.SetBool("MiniGun", true);
                anim.SetBool("NoWeapon", false);
                rifle.SetActive(false);
                pistol.SetActive(false);
                miniGun.SetActive(true);
                break;
            case "NoWeapon":
                anim.SetBool("Pistol", false);
                anim.SetBool("Rifle", false);
                anim.SetBool("MiniGun", false);
                anim.SetBool("NoWeapon", true);
                rifle.SetActive(false);
                pistol.SetActive(false);
                miniGun.SetActive(false);
                break;
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + direction * currentSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        anim.SetBool("Jump", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case "pistol":
                if(!isPistol)
                {
                    isPistol = true;
                    pistolUi.color = Color.white;
                    ChooseWeapon(Weapons.Pistol);
                    //weapons = Weapons.Pistol;
                }
                break;
            case "rifle":
                if(!isRifle)
                {
                    isRifle = true;
                    rifleUi.color = Color.white;
                    ChooseWeapon(Weapons.Rifle);
                    //weapons = Weapons.Rifle;
                }
                break;
            case ("minigun"):
                if (!isMiniGun)
                {
                    isMiniGun = true;
                    miniGunUi.color = Color.white;
                    ChooseWeapon(Weapons.MiniGun);
                    //weapons = Weapons.MiniGun;
                }
                break;
            default:
                Debug.Log("boncuk");
                break;
        }
        Destroy(other.gameObject);
    }
}
