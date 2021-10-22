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
        rigid.velocity = Vector3.zero; //굴러가는 속도 zero
        rigid.angularVelocity = Vector3.zero; //회전도 zero
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        //범위 피격
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15.0f, Vector3.up, 0,LayerMask.GetMask("Enemy")); //범위에 모든걸 가져와야하니 all //위치 범위 방향 길이 layer

        foreach(RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5.0f);
    }



}
