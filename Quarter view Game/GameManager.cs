using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Game manager에 필요한 변서
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public GameObject itemShop;
    public GameObject gunShop;
    public GameObject stageZone;

    public int stage;
    public float playTime;
    public bool isBattle;

    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;

    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;


    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;

    public Text maxScoreText;
    public Text scoreText;
    public Text stageText;
    public Text playTimeText;

    public Text playerHp;
    public Text playerAmmo;
    public Text playerCoin;

    public Image weapon1;
    public Image weapon2;
    public Image weapon3;
    public Image weaponR;

    public Text monA;
    public Text monB;
    public Text monC;

    public GameObject bossHpGroup;
    public RectTransform bossHpBar;
    public Text curScoreText;
    public Text bestScoreText;

    private void Awake()
    {
        enemyList = new List<int>();
        maxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("Max Score"));

        if (PlayerPrefs.HasKey("Max Score"))
            PlayerPrefs.SetInt("MaxScore", 0);
    }

    public void GameStart()
    {

        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    public void StageStart()
    {
        itemShop.SetActive(false);
        gunShop.SetActive(false);
        stageZone.SetActive(false);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);


        isBattle = true;
        StartCoroutine(InBattle());
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);

        overPanel.SetActive(true);

        curScoreText.text = scoreText.text;

        int maxScore = PlayerPrefs.GetInt("Max Score");
        if(player.score > maxScore)
        {
            bestScoreText.gameObject.SetActive(true);
            PlayerPrefs.GetInt("Max Score", player.score);

        }
    }

    public void ReStart()
    {
        SceneManager.LoadScene(0);
    }

    public void StageEnd()
    {
        player.transform.position = Vector3.up * 0.8f;

        itemShop.SetActive(true);
        gunShop.SetActive(true);
        stageZone.SetActive(true);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        isBattle = false;
        stage++;

    }

    IEnumerator InBattle()
    {
        if (enemyCntA < 0)
            enemyCntA = 0;
        if (enemyCntB < 0)
            enemyCntB = 0;
        if (enemyCntC < 0)
            enemyCntC = 0;

        if (stage % 5 == 0)
        {
            enemyCntD++;
            GameObject instantenemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            enemy enemy = instantenemy.GetComponent<enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            boss = instantenemy.GetComponent<Boss>();
        }
        else
        {

            for (int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                switch (ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }

            }

        

            while (enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);
                GameObject instantenemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);

                enemy enemys = instantenemy.GetComponent<enemy>();
                enemys.target = player.transform;

                enemys.manager = this;

                enemyList.RemoveAt(0);
                yield return new WaitForSeconds(4f);
                //1프레임에 다나올수도있음
            }
        }

        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0) //update역활
        {
            yield return null;

        }
        yield return new WaitForSeconds(5.0f);
        boss = null;
        StageEnd();
    }


    private void Update()
    {
        if (isBattle)
            playTime += Time.deltaTime;


    }

    private void LateUpdate()
    {
        //Update가 끝난후 호출되는 함수
        scoreText.text = string.Format("{0:n0}", player.score);
        stageText.text = "STAGE " + stage;

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int sec = (int)(playTime % 60);
        playTimeText.text =
                  string.Format("{0:00}", hour) +
            ":" + string.Format("{0:00}", min) +
            ":" + string.Format("{0:00}", sec);


        playerHp.text = player.health + " / " + player.maxHealth;
        playerCoin.text = string.Format("{0:n0}", player.coin);

        if (player.equipWeapon == null)
            playerAmmo.text = "- / " + player.ammo;
        else if (player.equipWeapon.type == Weapon.Type.Melee)
            playerAmmo.text = "- / " + player.ammo;
        else
            playerAmmo.text = player.equipWeapon.curAmmo + " / " + player.ammo;
        //총알부분

        weapon1.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponR.color = new Color(1, 1, 1, player.hasGrenade > 0 ? 1 : 0);

        monA.text = enemyCntA.ToString();
        monB.text = enemyCntB.ToString();
        monC.text = enemyCntC.ToString();

        if (boss != null)
        {
            bossHpGroup.SetActive(true);
            bossHpBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
        }

        else
        {
            bossHpGroup.SetActive(false);
        }

    }

}
