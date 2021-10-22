using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpPower = 15.0f;
    public GameObject[] weapon;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public GameObject grenadeObj;

    public GameManager manager;

    public Camera followCamera;

    public int ammo;
    public int coin;
    public int health;
    public int hasGrenade;
    public int score;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenade;

    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool iDown;

    bool IsJump; //2단점프 방지
    bool IsDodge;
    bool isBorder;
    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool IsSwap;
    bool isFireReady;
    bool isDamage;
    bool isShop;
    bool isDie;


    bool fDown;
    bool gDown;
    bool rDown; //재장전
    bool isReload; //reload time

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    MeshRenderer[] meshs; //player는 머리 몸 팔 등등 많아서 배열로

    GameObject nearObj;
    public Weapon equipWeapon;

    int equipWeaponIndex = -1;
    float fireDelay;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>(); //자식오브젝트를 가져옴
        meshs = GetComponentsInChildren<MeshRenderer>(); //GetComponentsInChildren s가 붙음 배열만


        //PlayerPrefs.SetInt("Max Score", 112500);
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Grenade();
        Attack();
        Reload();
        Dodge();
        Swap();
        Interation();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal"); //h v 값은 project Setting inputAxis에서 받아옴
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1"); //마우스 왼
        gDown = Input.GetButtonDown("Fire2"); //마우스 오
        rDown = Input.GetButtonDown("Reload");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //대각선이 빠르면안되니 모든 방향값이 1로 고정

        if (IsDodge)
            moveVec = dodgeVec;

        if (IsSwap || isDie)
            moveVec = Vector3.zero;
        //if (wDown)
        //    transform.position += moveVec * moveSpeed * 0.3f * Time.deltaTime;
        //else
        //    transform.position += moveVec * moveSpeed * Time.deltaTime;

        if (!isBorder) //회전은 가능하도록
            transform.position += moveVec * moveSpeed * (wDown ? 0.3f : 1.0f) * Time.deltaTime;

        anim.SetBool("IsRun", moveVec != Vector3.zero);
        anim.SetBool("IsWalk", wDown);


    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);//나아갈방향으로 바라보게만들기

        //마우스 회전
        if (fDown && !isDie)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); //스크린에서 월드로 ray를 쏘는 함수
            RaycastHit rayHit;

            if (Physics.Raycast(ray, out rayHit, 100)) //return처럼 반환값을 변수에 저장하는 코드
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !IsJump && !IsSwap && !isDie)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetBool("IsJump", true);
            anim.SetTrigger("doJump");

            IsJump = true;
        }
    }

    void Grenade()
    {
        if (hasGrenade == 0)
            return;

        if (gDown && !isReload && !IsSwap && !isDie)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); //스크린에서 월드로 ray를 쏘는 함수
            RaycastHit rayHit;

            if (Physics.Raycast(ray, out rayHit, 100)) //return처럼 반환값을 변수에 저장하는 코드
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position + Vector3.forward * 2.0f, transform.rotation);
                //인스턴스화

                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenade--;
                grenades[hasGrenade].SetActive(false);//공전하는 수류탄 제거

            }
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay; //무기에 달려있는 공속보다 딜레이가 클시


        if (fDown && isFireReady && !IsDodge && !IsSwap && !isShop && !isDie)
        {
            equipWeapon.Use(); //조건이 충족되면 무기의 함수를 호출 및 사용
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot"); //애니메이션 설정
            fireDelay = 0;
        }


    }

    void Reload()
    {
        if (equipWeapon == null)
            return;

        if (equipWeapon.type == Weapon.Type.Melee)
            return;

        if (ammo == 0)
            return;

        if (rDown && !IsJump && !IsDodge && !IsSwap && isFireReady && !isShop && !isDie)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 0.5f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;

        isReload = false;

    }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !IsJump && !IsDodge && !IsSwap && !isDie)
        {
            dodgeVec = moveVec;
            moveSpeed *= 2;
            anim.SetTrigger("doDodge");
            IsDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        moveSpeed *= 0.5f;
        IsDodge = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("IsJump", false);
            IsJump = false;
        }
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;
        //weapon이 없을때 무기가 나타나지 않게

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;


        if ((sDown1 || sDown2 || sDown3) && !IsDodge && !IsJump && !isDie)
        {
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapon[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            IsSwap = true;

            Invoke("SwapOut", 0.5f);
        }
    }

    void SwapOut()
    {
        IsSwap = false;
    }

    void Interation()
    {
        if (iDown && nearObj != null)
        {
            if (nearObj.tag == "Weapon")
            {
                Item item = nearObj.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObj);
            }

            else if (nearObj.tag == "Shop")
            {
                Shop shop = nearObj.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }//상점 구현


        }
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero; //물리 회전속도
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);//선을 그림 //Scene에서 보임

        isBorder = Physics.Raycast(transform.position, transform.forward, 2, LayerMask.GetMask("Wall")); //벽에 부딪히면 bool값이 true가됨
    }

    private void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();

            switch (item.tpye)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;

                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;

                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    if (hasGrenade == maxHasGrenade)
                        return;
                    grenades[hasGrenade].SetActive(true);
                    hasGrenade += item.value;
                    break;
            }
            Destroy(other.gameObject);
        }

        else if (other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {

                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                //if (other.GetComponent<Rigidbody>() != null)
                //Destroy(other.gameObject);

                bool isBossAtk = other.name == "Boss Melee Aree";

                StartCoroutine(OnDamage(isBossAtk));
            }
            if (other.GetComponent<Rigidbody>() != null)
                Destroy(other.gameObject);
        }


    }

    IEnumerator OnDamage(bool isBossAtk)
    {
        isDamage = true;

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }

        if (isBossAtk)
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);

        if (health <= 0 && !isDie)
            OnDie();
        yield return new WaitForSeconds(1.0f);

        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;

        }
        if (isBossAtk)
            rigid.velocity = Vector3.zero;

        

    }

    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDie = true;
        manager.GameOver();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Shop")
        {
            nearObj = other.gameObject;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObj = null;
        else if (other.tag == "Shop")
        {
            Shop shop = nearObj.GetComponent<Shop>();
            if (shop != null)
                shop.Exit();
            isShop = false;
            nearObj = null;
        }
    }


}
