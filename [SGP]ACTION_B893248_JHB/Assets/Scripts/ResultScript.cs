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

    public GameObject Bed;
    public GameObject Cat;
    public GameObject Table;
    public GameObject Chicken;
    public GameObject Car
        ;
    private new string name;
    [SerializeField]private bool isShowing;

    private enum REWARD
    {
        NONE = 0,
        BED,
        CAT,
        TABLE,
        CHICKEN,
        CAR
    }
    private REWARD type;

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

        MissionManger.Instance.isSetup = false;
    }

    private void FixedUpdate()
    {
        if (isShowing && name != " ")
        {
            Camera.main.transform.Find("Reward").transform.Find(name).transform.Rotate(new Vector3(0, 10f * Time.fixedDeltaTime, 0));
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
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("Distance", 0);
        PlayerPrefs.SetInt("TotalScore", 0);
        SceneManager.LoadScene("TitleScene");
    }

    // 도전과제를 모두 성공했을 경우 보상 창을 띄움
    public void ShowReward()
    {
        // 보상(클리어) 창 보이기
        RewardPanel.SetActive(true);
        if (PlayerPrefs.HasKey("Bed") && PlayerPrefs.GetInt("Bed") == 1)
        {
            type = REWARD.BED;
        }
        if (PlayerPrefs.HasKey("Cat") && PlayerPrefs.GetInt("Cat") == 1)
        {
            type = REWARD.CAT;
        }
        if (PlayerPrefs.HasKey("Table") && PlayerPrefs.GetInt("Table") == 1)
        {
            type = REWARD.TABLE;
        }
        if (PlayerPrefs.HasKey("Chicken") && PlayerPrefs.GetInt("Chicken") == 1)
        {
            type = REWARD.CHICKEN;
        }
        if (PlayerPrefs.HasKey("Car") && PlayerPrefs.GetInt("Car") == 1)
        {
            type = REWARD.CAR;
        }

        // 보상에 따른 오브젝트 인스턴싱
        switch (type)
        {
            case REWARD.NONE:
                break;
            case REWARD.BED:
                GameObject bed = Instantiate(Bed, Camera.main.transform.Find("Reward").transform) as GameObject;
                bed.GetComponent<Rigidbody>().useGravity = false;
                bed.GetComponent<AudioSource>().playOnAwake = true;
                name = bed.gameObject.name;
                break;
            case REWARD.CAT:
                GameObject cat = Instantiate(Cat, Camera.main.transform.Find("Reward").transform) as GameObject;
                cat.GetComponent<PlayerControl>().enabled = false;
                cat.GetComponent<Rigidbody>().useGravity = false;
                cat.GetComponent<AudioSource>().playOnAwake = true;
                name = cat.gameObject.name;
                break;
            case REWARD.TABLE:
                GameObject table = Instantiate(Table, Camera.main.transform.Find("Reward").transform) as GameObject;
                table.GetComponent<Rigidbody>().useGravity = false;
                table.GetComponent<AudioSource>().playOnAwake = true;
                name = table.gameObject.name;
                break;
            case REWARD.CHICKEN:
                GameObject chicken = Instantiate(Chicken, Camera.main.transform.Find("Reward").transform) as GameObject;
                chicken.GetComponent<PlayerControl>().enabled = false;
                chicken.GetComponent<Rigidbody>().useGravity = false;
                chicken.GetComponent<AudioSource>().playOnAwake = true;
                name = chicken.gameObject.name;
                break;
            case REWARD.CAR:
                GameObject car = Instantiate(Car, Camera.main.transform.Find("Reward").transform) as GameObject;
                car.GetComponent<Rigidbody>().useGravity = false;
                car.GetComponent<AudioSource>().playOnAwake = true;
                name = car.gameObject.name;
                break;
            default:
                name = " ";
                break;
        }

        // 문자열 "(Clone)" 삭제
        string reward = name;
        int index = 0;
        foreach (var cut in reward)
        {
            if (cut == '(')
                break;
            index++;
        }
        reward = reward.Remove(index);

        // 얻은 보상의 종류 출력
        if (type == REWARD.CAT || type == REWARD.CHICKEN)
            RewardPanel.transform.Find("Type Text").GetComponent<TextMeshProUGUI>().text = "New Animal : " + reward;
        else
            RewardPanel.transform.Find("Type Text").GetComponent<TextMeshProUGUI>().text = "New Mount : " + reward;
    }
}
