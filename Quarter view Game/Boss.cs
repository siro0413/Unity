using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;

    Vector3 lookVec;
    Vector3 tauntVec;

    public bool isLook;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>(); //mesh�� �ڽ��̶� 
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true;
        StartCoroutine(Think());
    }

    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }


        if (isLook) //look player
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            lookVec = new Vector3(h, 0, v) * 5.0f;
            transform.LookAt(target.position + lookVec); //look player target

        }

        else
            nav.SetDestination(tauntVec);//Taunt�Ҷ� ��ǥ�����̵�
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int ranAction = Random.Range(0, 5);//0~5Random
        switch (ranAction)
        {
            case 0:
            case 1: //�̻��� �߻�
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3: //�� �߻�
                StartCoroutine(RockShot());
                break;
            case 4: //���� ����
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;


        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;


        yield return new WaitForSeconds(2.0f);
        StartCoroutine(Think());
    }

    IEnumerator RockShot()
    {
        isLook = false; //�����Ͽ��� ��� Ÿ��������
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);

        yield return new WaitForSeconds(3.0f);
        isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator Taunt()
    {
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false; //player�ǰݽ� �浹 ����
        anim.SetTrigger("doTaunt");
        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1.0f);
        isLook = true;
        //nav.isStopped = true;

        boxCollider.enabled = true;

        StartCoroutine(Think());
    }

}
