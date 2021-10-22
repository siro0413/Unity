using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };
    public Type tpye;
    public int value;

    Rigidbody rigid;
    SphereCollider sphereCollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>(); //���� ������Ʈ�� �����͸� ������
    }


    private void Update()
    {
        transform.Rotate(Vector3.up * 30 * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true; //�ܺ� �����ۿ� ���Ը����
            sphereCollider.enabled = false;

        }
    }

}
