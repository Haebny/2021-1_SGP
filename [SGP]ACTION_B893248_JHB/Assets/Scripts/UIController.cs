using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI Text;

    private PlayerControl player;
    private GameRoot root;
    private FireController fire;
    private Image image;
    private GameObject PausePanel;

    private Vector3 startPos;
    private float startTime;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerControl>();
        root = GameObject.FindObjectOfType<GameRoot>().GetComponent<GameRoot>();
        fire = GameObject.FindObjectOfType<FireController>().GetComponent<FireController>();
        image = this.transform.Find("Fire Image").GetComponent<Image>();
        PausePanel =this.transform.Find("Pause Panel").gameObject;

        startPos = player.transform.position;
    }

    private void Update()
    {
        ShowScore();
        ShowWarning();
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
            Time.timeScale = 0f;
            Text.text = "Go";
        }
        else
        {
            StartCoroutine(ResumeTimer(3));
            Text.text = "||";
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
    }

    // 열쇠와 대시의 개수를 알려주는 메소드
    public void CountItems()
    {
        // 대시 횟수
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
        FireController.STATE level = fire.CheckDistance();        

        if(level == FireController.STATE.LEVEL1)
        {
            image.sprite = Resources.Load<Sprite>("Image/fire1");
        }
        else if(level == FireController.STATE.LEVEL2)
        {
            image.sprite = Resources.Load<Sprite>("Image/fire2");
        }
        else
        {
            image.sprite = Resources.Load<Sprite>("Image/fire3");
        }
    }

    public void UsingDash()
    {
        player.UsingDash(PlayerControl.DASH_TYPE.SKILL);
    }
}
