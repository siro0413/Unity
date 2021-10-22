using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3.0f);
        rigid.velocity = Vector3.zero; //�������� �ӵ� zero
        rigid.angularVelocity = Vector3.zero; //ȸ���� zero
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        //���� �ǰ�
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15.0f, Vector3.up, 0,LayerMask.GetMask("Enemy")); //������ ���� �����;��ϴ� all //��ġ ���� ���� ���� layer

        foreach(RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5.0f);
    }



}
