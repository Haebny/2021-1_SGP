using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleScript : MonoBehaviour
{
    private void Start()
    {
        Screen.SetResolution(Screen.width, Screen.width * 16 / 9,  false);
        SetBestScore();
        if (PlayerPrefs.GetInt("Bed") == 1)
        {
            GameObject mount = Camera.main.transform.Find("Mount").gameObject;
            mount.SetActive(true);
            Rigidbody[] rbs = mount.transform.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rbs)
            {
                rb.useGravity = false;
            }
        }
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

    // 데이터 삭제
    public void DeleteData()
    {
        PlayerPrefs.DeleteAll();
        SetBestScore();
    }
}