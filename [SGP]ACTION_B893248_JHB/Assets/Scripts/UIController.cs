using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI Text;

    private PlayerControl player;
    private GameRoot root;
    private Fire fire;
    private LevelControl levelControl;
    private Image image;
    private GameObject PausePanel;

    private Vector3 startPos;
    private float startTime;

    public bool levelUP = false;

    private void Start()
    {
        player = GameObject.FindObjectOfType<PlayerControl>().GetComponent<PlayerControl>();
        root = GameObject.FindObjectOfType<GameRoot>().GetComponent<GameRoot>();
        fire = GameObject.FindObjectOfType<Fire>().GetComponent<Fire>();
        levelControl = GameObject.FindObjectOfType<LevelControl>().GetComponent<LevelControl>();
        image = this.transform.Find("Fire Image").GetComponent<Image>();
        PausePanel =this.transform.Find("Pause Panel").gameObject;

        startPos = player.transform.position;
    }

    private void Update()
    {
        if(player.GameOver)
        {
            GoToEnd();
        }

        CountItems();
        ShowScore();
        ShowWarning();
        ShowMission();

        if (player.LevelUp == true)
        {
            levelUP = true;
            levelControl.LevelUp();
            player.LevelUp = false;
        }

        //if (Input.GetKeyDown(KeyCode.Keypad4))
        //    PlayerPrefs.SetInt("Cat", 0);
        //if (Input.GetKeyDown(KeyCode.Keypad5))
        //    PlayerPrefs.SetInt("Chicken", 0);
    }

    // 일시정지 기능 메소드
    public void PauseGame()
    {
        Text = this.transform.Find("Pause Button").transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        // Pause 기능
        if (!PausePanel.activeSelf)
        {
            // Pause 패널 띄우기
            PausePanel.SetActive(!PausePanel.activeSelf);
            TextMeshProUGUI resumeTime = this.transform.Find("Pause Panel").GetChild(0).GetComponent<TextMeshProUGUI>();
            resumeTime.text = "Pause Game";
            Time.timeScale = 0f;
            Text.text = "Go";
        }
        else
        {
            StartCoroutine(ResumeTimer(3));
            Text.text = "||";
            StopCoroutine("ResumeTimer");
        }
    }

    // sec만큼의 시간을 기다리는 타이머 코루틴
    IEnumerator ResumeTimer(int sec)
    {
        TextMeshProUGUI resumeTime = this.transform.Find("Pause Panel").GetChild(0).GetComponent<TextMeshProUGUI>();
        int time = 0;

        while (time < sec)
        {
            resumeTime.text = (sec - time).ToString();

            yield return new WaitForSecondsRealtime(1);
            time++;
        }

        Time.timeScale = 1f;

        // Pause 패널 없애기
        PausePanel.SetActive(!PausePanel.activeSelf);
        yield return null;
    }

    // 열쇠와 돌진의 개수를 알려주는 메소드
    public void CountItems()
    {
        // 돌진 횟수
        Text = this.transform.Find("Dash Button").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Text.text = "x " + player.Skill.ToString();

        // 열쇠 개수
        Text = this.transform.Find("Key Image").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Text.text = "x " + player.Key.ToString();
    }

    // 획득 점수와 거리를 알려주는 메소드
    public void ShowScore()
    {
        // 메인 점수
        Text = this.transform.Find("Score Text").GetComponent<TextMeshProUGUI>();
        Text.text = ((int)player.Score).ToString();

        // 달린 거리
        Text = this.transform.Find("Meter Text").GetComponent<TextMeshProUGUI>();
        Text.text = ((int)player.Meter).ToString() + " m";
    }

    // 산불과 플레이어와이 거리를 기반으로 경고해주는 메소드
    public void ShowWarning()
    {
        FireController.LEVEL level = fire.CheckDistance();        

        if(level == FireController.LEVEL.LEVEL1)
        {
            image.sprite = Resources.Load<Sprite>("Image/Icons/fire1");
        }
        else if(level == FireController.LEVEL.LEVEL2)
        {
            image.sprite = Resources.Load<Sprite>("Image/Icons/fire2");
        }
        else
        {
            image.sprite = Resources.Load<Sprite>("Image/Icons/fire3");
        }
    }

    public void UsingDash()
    {
        player.UsingDash(PlayerControl.DASH_TYPE.SKILL);
    }

    //public void GoToTitle()
    //{
    //    SceneManager.LoadScene("TitleScene");
    //}

    public void GoToEnd()
    {
        SceneManager.LoadScene("ResultScene");
    }

    public void ShowMission()
    {
        int count = 0;

        // 고양이를 보유하고 있지 않을 때
        if (PlayerPrefs.GetInt("Cat") == 0)
        {
            //첫번째 미션
            count = PlayerPrefs.GetInt("Tumbling3");

            if (player.Is_perfect == true)
            {
                player.Is_perfect = false;
                PlayerPrefs.SetInt("Tumbling3", count + 1);
                count = PlayerPrefs.GetInt("Tumbling3");
            }
            Text = transform.Find("Mission Panel").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            if (count >= 3) // 성공 시 녹색으로 표시
            {
                Text.color = Color.green;
                count = 3;
            }

            Text.text = "- 텀블링 성공하기 (" + count.ToString() + "/3)";


            //두번째 미션
            count = PlayerPrefs.GetInt("Crushing3");

            if (player.Is_crushing == true)
            {
                player.Is_crushing = false;
                PlayerPrefs.SetInt("Crushing3", count + 1);
                count = PlayerPrefs.GetInt("Crushing3");
            }
            Text = transform.Find("Mission Panel").transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            if (count >= 3) // 성공 시 녹색으로 표시
            { 
                Text.color = Color.green;
                count = 3;
            }

            Text.text = "- 장애물 파괴하기 (" + count.ToString() + "/3)";

            //// 세번째 미션
            //count = PlayerPrefs.GetInt("Hopping1");

            //if (player.Is_hopping == true)
            //{
            //    PlayerPrefs.SetInt("Hopping1", count + 1);
            //    count = PlayerPrefs.GetInt("Hopping1");
            //}
            //Text = transform.Find("Mission Panel").transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            //Text.text = "- 장애물 밟기 (" + count.ToString() + "/1)";

            //if (count >= 1) // 성공 시 녹색으로 표시
            //{ 
            //    Text.color = Color.green;
            //    count = 1;
            //}
        }
    }
}
