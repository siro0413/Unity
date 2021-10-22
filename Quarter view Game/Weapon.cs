using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate;

    public int maxAmmo; //최대 탄창
    public int curAmmo; //현재 탄창

    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;

    //코루틴
    public void Use()
    {
        if (type == Type.Melee)
        {
            //Swing();
            StopCoroutine("Swing");
            StartCoroutine("Swing"); //코루틴 불러올때 함수

        }

        else if (type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing() //IEnumerator 열거형 함수 클래스
    {

        //1
        yield return new WaitForSeconds(0.1f); //yield 결과값을 반환 (0.1초 대기)
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        //2
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        //3
        yield return new WaitForSeconds(0.5f);
        trailEffect.enabled = false;
        //yield는 여러개 사용 가능

        //yield break;//코루틴 정지
    }

    IEnumerator Shot()
    {
        //1. 총알발사
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 100;

        yield return null;

        //2. 탄피 배출
        GameObject intantcase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantBullet.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-5, -4) + Vector3.up * Random.Range(4, 5);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);//즉발적
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);//총알 회전

    }

    //Use() 메인 루틴 -> Swing()호출 서부 루틴 -> Use() 메인 루틴 -교차 실행
    //Use() 메인 루틴 + Swing() 코루틴 (Co-Op) - 같이 실행
}
