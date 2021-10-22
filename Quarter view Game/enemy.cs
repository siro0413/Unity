using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemy : MonoBehaviour
{
    public enum Type { A, B, C, D };
    public Type enemyType;

    public int maxHealth;
    public int curHealth;
    public int score;
    public GameObject[] coins;

    public GameManager manager;

    public bool isChase; //����
    public bool isAttack; //����
    public bool isDead; //dead
    public BoxCollider meleeArea;
    public GameObject bullet;



    public Transform target;

    public BoxCollider boxCollider;
    public Rigidbody rigid;

    public NavMeshAgent nav; //UnityEngine.AI �߰��ؾߵ�

    public MeshRenderer[] meshs; //��� ���׸���

    public Animator anim;

    private void Update()
    {
        if (nav.enabled && enemyType != Type.D)//�׺� Ȱ���ɋ���
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }

    }



    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero; //���� ȸ���ӵ�
            rigid.angularVelocity = Vector3.zero; //���� ȸ���ӵ�
        }
    }

    void Targeting()
    {
        if (enemyType != Type.D && !isDead)
        {

            float targetRadius = 0f;
            float targetRange = 0f;

            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3.0f;
                    break;

                case Type.B:
                    targetRadius = 1.0f;
                    targetRange = 6.0f;
                    break;

                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25.0f;
                    break;

            }



            RaycastHit[] rayHits = Physics.SphereCastAll
                (transform.position,
                targetRadius,
                transform.forward,
                targetRange,
                LayerMask.GetMask("Player")); //������ ���� �����;��ϴ� all //��ġ ���� ���� ���� layer


            if (rayHits.Length > 0 && !isAttack) //�������̰ų� ���ݹ����϶�
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {

        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:

                yield return new WaitForSeconds(0.2f);

                meleeArea.enabled = true;

                yield return new WaitForSeconds(1.0f);

                meleeArea.enabled = false;

                yield return new WaitForSeconds(1.0f);
                break;

            case Type.B:
                yield return new WaitForSeconds(0.1f);
                meleeArea.enabled = true;
                rigid.AddForce(transform.forward * 50, ForceMode.Impulse);

                yield return new WaitForSeconds(0.5f);

                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1.5f);

                break;

            case Type.C:

                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20f;

                yield return new WaitForSeconds(2f);

                break;

        }

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);



    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>(); //mesh�� �ڽ��̶� 
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if (enemyType != Type.D)
            Invoke("ChaseStart", 2.0f);
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reacVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reacVec, false));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;

            Vector3 rectVec = transform.position - other.transform.position; //�˹� ó��

            StartCoroutine(OnDamage(rectVec));
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;

            Vector3 rectVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(rectVec));

        }
    }


    IEnumerator OnDamage(Vector3 rectVec, bool isGrenade = true)
    {

        //
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red; //�ǰݽ� ���󺯰�
        if (curHealth > 0)
            yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white; //���� ���׾����� ü���� ���󺹱�

            rectVec = rectVec.normalized; //�˹�
            rectVec += Vector3.up;
            rigid.AddForce(rectVec * 15, ForceMode.Impulse);

        }
        else
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 14; //������ ������ ����
            isDead = true;
            isChase = false;
            nav.enabled = false; //������׼� �����ϱ�����

            anim.SetTrigger("doDie");

            Player player = target.GetComponent<Player>();
            player.score += score;
            int ranCoin = Random.Range(0, 3);
            Instantiate(coins[ranCoin], transform.position, Quaternion.identity);

            switch (enemyType)
            {
                case Type.A:
                    manager.enemyCntA--;
                    break;
                case Type.B:
                    manager.enemyCntB--;
                    break;
                case Type.C:
                    manager.enemyCntC--;
                    break;
                case Type.D:
                    manager.enemyCntD--;
                    break;
            }



            if (isGrenade)
            {
                rectVec = rectVec.normalized; //�˹�
                rectVec += Vector3.up * 3;

                rigid.freezeRotation = false; //freezenROtation Ǯ��
                rigid.AddForce(rectVec * 5, ForceMode.Impulse);
                rigid.AddTorque(rectVec * 15, ForceMode.Impulse);
            }
            else
            {
                rectVec = rectVec.normalized; //�˹�
                rectVec += Vector3.up;
                rigid.AddForce(rectVec * 5, ForceMode.Impulse);

            }
            Destroy(gameObject, 4.0f);
        }
        //if (enemyType != Type.D)
        //    Destroy(gameObject);

    }


}
