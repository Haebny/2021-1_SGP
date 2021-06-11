using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScript : MonoBehaviour
{
    private void Start()
    {
        Screen.SetResolution(Screen.width, Screen.width * 16 / 9,  false);
        SetBestScore();
    }

    public void GameStart()
    {
        SceneManager.LoadScene("MainScene");
    }

    public int GetBestScore()
    {
        int a = PlayerPrefs.GetInt("BestScore");
        int b = PlayerPrefs.GetInt("TotalScore");

        return (a > b) ? a : b;
    }

    public void SetBestScore()
    {
        TextMeshProUGUI bestRecord = GameObject.Find("UI").transform.Find("Record Text").GetComponent<TextMeshProUGUI>();
        bestRecord.text = "Best Record : " + GetBestScore().ToString();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // 데이터 삭제
    public void DeleteData()
    {
        PlayerPrefs.DeleteAll();
    }


}