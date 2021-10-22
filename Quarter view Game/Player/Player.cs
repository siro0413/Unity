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

    bool IsJump; //2������ ����
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
    bool rDown; //������
    bool isReload; //reload time

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    MeshRenderer[] meshs; //player�� �Ӹ� �� �� ��� ���Ƽ� �迭��

    GameObject nearObj;
    public Weapon equipWeapon;

    int equipWeaponIndex = -1;
    float fireDelay;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>(); //�ڽĿ�����Ʈ�� ������
        meshs = GetComponentsInChildren<MeshRenderer>(); //GetComponentsInChildren s�� ���� �迭��


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
        hAxis = Input.GetAxisRaw("Horizontal"); //h v ���� project Setting inputAxis���� �޾ƿ�
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1"); //���콺 ��
        gDown = Input.GetButtonDown("Fire2"); //���콺 ��
        rDown = Input.GetButtonDown("Reload");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //�밢���� ������ȵǴ� ��� ���Ⱚ�� 1�� ����

        if (IsDodge)
            moveVec = dodgeVec;

        if (IsSwap || isDie)
            moveVec = Vector3.zero;
        //if (wDown)
        //    transform.position += moveVec * moveSpeed * 0.3f * Time.deltaTime;
        //else
        //    transform.position += moveVec * moveSpeed * Time.deltaTime;

        if (!isBorder) //ȸ���� �����ϵ���
            transform.position += moveVec * moveSpeed * (wDown ? 0.3f : 1.0f) * Time.deltaTime;

        anim.SetBool("IsRun", moveVec != Vector3.zero);
        anim.SetBool("IsWalk", wDown);


    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);//���ư��������� �ٶ󺸰Ը����

        //���콺 ȸ��
        if (fDown && !isDie)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); //��ũ������ ����� ray�� ��� �Լ�
            RaycastHit rayHit;

            if (Physics.Raycast(ray, out rayHit, 100)) //returnó�� ��ȯ���� ������ �����ϴ� �ڵ�
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
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); //��ũ������ ����� ray�� ��� �Լ�
            RaycastHit rayHit;

            if (Physics.Raycast(ray, out rayHit, 100)) //returnó�� ��ȯ���� ������ �����ϴ� �ڵ�
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position + Vector3.forward * 2.0f, transform.rotation);
                //�ν��Ͻ�ȭ

                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenade--;
                grenades[hasGrenade].SetActive(false);//�����ϴ� ����ź ����

            }
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay; //���⿡ �޷��ִ� ���Ӻ��� �����̰� Ŭ��


        if (fDown && isFireReady && !IsDodge && !IsSwap && !isShop && !isDie)
        {
            equipWeapon.Use(); //������ �����Ǹ� ������ �Լ��� ȣ�� �� ���
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot"); //�ִϸ��̼� ����
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
        //weapon�� ������ ���Ⱑ ��Ÿ���� �ʰ�

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
            }//���� ����


        }
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero; //���� ȸ���ӵ�
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);//���� �׸� //Scene���� ����

        isBorder = Physics.Raycast(transform.position, transform.forward, 2, LayerMask.GetMask("Wall")); //���� �ε����� bool���� true����
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
