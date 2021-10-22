using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;

    public GameObject[] itemObj;
    public int[] itemPrice;
    public Transform[] itemPos;

    public Text talkText;

    public string[] talkData;

    Player enterPalyer;

    public void Enter(Player player)
    {
        enterPalyer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int index)
    {
        int price = itemPrice[index];
        if (price > enterPalyer.coin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());

            return;
        }

        enterPalyer.coin -= price;

        Vector3 ranVec = Vector3.right * Random.Range(-3, 3)
            + Vector3.forward * Random.Range(-3, 3);

        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);
    }


    IEnumerator Talk()
    {
        talkText.text = talkData[1];

        yield return new WaitForSeconds(2.0f);

        talkText.text = talkData[0];
        yield return new WaitForSeconds(2.0f);

    }
}
