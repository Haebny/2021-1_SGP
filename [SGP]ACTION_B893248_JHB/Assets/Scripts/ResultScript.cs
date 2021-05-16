using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ResultScript : MonoBehaviour
{
    private TextMeshProUGUI tmPro;
    public GameObject newText;

    private void Start()
    {
        GetResultScore();
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
            newText.SetActive(true);
            PlayerPrefs.SetInt("BestScore", PlayerPrefs.GetInt("TotalScore"));
        }
    }

    public void EndGame()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
