using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{

    Rigidbody rigidbody;
    float angularPower = 2;
    float scaleValue = 0.5f;
    bool isShoot;
    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());

    }

    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f);
        isShoot = true;
    }

    IEnumerator GainPower()
    {
        while(!isShoot)
        {
            angularPower += 0.02f;
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue;
            rigidbody.AddTorque(transform.right * angularPower, ForceMode.Acceleration); //지속증가
            yield return null;//while에는 무조건 포함
        }
    }
}
