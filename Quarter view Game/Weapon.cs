using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate;

    public int maxAmmo; //�ִ� źâ
    public int curAmmo; //���� źâ

    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;

    //�ڷ�ƾ
    public void Use()
    {
        if (type == Type.Melee)
        {
            //Swing();
            StopCoroutine("Swing");
            StartCoroutine("Swing"); //�ڷ�ƾ �ҷ��ö� �Լ�

        }

        else if (type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing() //IEnumerator ������ �Լ� Ŭ����
    {

        //1
        yield return new WaitForSeconds(0.1f); //yield ������� ��ȯ (0.1�� ���)
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        //2
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        //3
        yield return new WaitForSeconds(0.5f);
        trailEffect.enabled = false;
        //yield�� ������ ��� ����

        //yield break;//�ڷ�ƾ ����
    }

    IEnumerator Shot()
    {
        //1. �Ѿ˹߻�
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 100;

        yield return null;

        //2. ź�� ����
        GameObject intantcase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantBullet.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-5, -4) + Vector3.up * Random.Range(4, 5);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);//�����
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);//�Ѿ� ȸ��

    }

    //Use() ���� ��ƾ -> Swing()ȣ�� ���� ��ƾ -> Use() ���� ��ƾ -���� ����
    //Use() ���� ��ƾ + Swing() �ڷ�ƾ (Co-Op) - ���� ����
}
