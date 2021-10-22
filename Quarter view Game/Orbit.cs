using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target;
    public float orbitSpeed;
    Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;

        transform.RotateAround
            (
            target.position, 
            Vector3.up, 
            orbitSpeed * Time.deltaTime
            );//공전함수 공전할타겟, 방향, 시간
        offset = transform.position - target.position;
    }
}
