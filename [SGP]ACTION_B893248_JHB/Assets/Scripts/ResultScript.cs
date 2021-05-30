using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ResultScript : MonoBehaviour
{
    private TextMeshProUGUI tmPro;
    public GameObject NewText;
    public GameObject RewardPanel;
    public GameObject Cat;
    //public GameObject Chicken;
    private string name;
    [SerializeField]private bool isShowing;

    private enum REWARD
    {
        NONE = 0,
        CAT,
        CHICKEN,
        BED,
        TUB,

    }

    private void Start()
    {
        Time.timeScale = 1.0f;
        GetResultScore();
        if (PlayerPrefs.GetInt("Clear") == 0)
            isShowing = false;
        else
        {
            isShowing = true;
            ShowReward();
        }
    }

    private void FixedUpdate()
    {
        if (isShowing && name != " ")
        {
            RewardPanel.transform.Find("Reward").transform.Find(name).transform.Rotate(new Vector3(0, 10f * Time.fixedDeltaTime, 0));
        }
    }

    public void GetResultScore()
    {
        tmPro = GameObject.Find("UI").transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        tmPro.text = "SCORE		" + PlayerPrefs.GetInt("Score").ToString();

        tmPro = GameObject.Find("UI").transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        tmPro.text = "DISTANCE	" + PlayerPrefs.GetInt("Distance").ToString();

        tmPro = GameObject.Find("UI").transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        tmPro.text = "TOTAL SCORE	" + PlayerPrefs.GetInt("TotalScore").ToString();

        if (PlayerPrefs.GetInt("TotalScore") > PlayerPrefs.GetInt("BestScore"))
        {
            NewText.SetActive(true);
            PlayerPrefs.SetInt("BestScore", PlayerPrefs.GetInt("TotalScore"));
        }
    }

    public void EndGame()
    {
        SceneManager.LoadScene("TitleScene");
    }

    // 도전과제를 모두 성공했을 경우 보상 창을 띄움
    public void ShowReward()
    {
        // 보상(클리어) 창 보이기
        RewardPanel.SetActive(true);
        int type = 0;
        if (PlayerPrefs.HasKey("Cat") && PlayerPrefs.GetInt("Cat") == 1)
        {
            type = 1;
        }
        if (PlayerPrefs.HasKey("Chicken") && PlayerPrefs.GetInt("Chicken") == 1)
        {
            type = 2;
        }

        // 고양이 보상
        if (type == 1)
        {
            GameObject reward = Instantiate(Cat, RewardPanel.transform.Find("Reward").transform) as GameObject;
            reward.GetComponent<PlayerControl>().enabled = false;
            reward.GetComponent<Rigidbody>().useGravity = false;
            reward.GetComponent<AudioSource>().playOnAwake = true;
            name = reward.gameObject.name;
        }
        //else if (type == 2)
        //{
        //    GameObject reward = Instantiate(Chicken, RewardPanel.transform.Find("Reward").transform.position, Quaternion.identity) as GameObject;
        //    reward.GetComponent<PlayerControl>().enabled = false;
        //    reward.GetComponent<Rigidbody>().useGravity = false;
        //    reward.GetComponent<AudioSource>().playOnAwake = true;
        //}
        else
        {
            name = " ";
        }
    }
}
