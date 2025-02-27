﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    static private GameObject model;
    static public int Current;

    private void Awake()
    {
        if (this.transform.name == "Start Button" || this.transform.name == "Quit Button")
            return;
        Current = 0;
        model = this.transform.parent.Find("HTP1").gameObject;
    }

    private void Start()
    {
        if (this.transform.name == "Start Button" || this.transform.name == "Quit Button")
            return;
        Time.timeScale = 1.0f;
        StartCoroutine(ChangeIcon());
    }

    void OnEnable()
    {
        if (this.transform.name == "Start Button" || this.transform.name == "Quit Button")
            return;
        if (ButtonController.Current == 2 || ButtonController.Current == 3)
            return;

        StartCoroutine(ChangeIcon());
    }

    void OnDisable()
    {
        if (this.transform.name == "Start Button" || this.transform.name == "Quit Button")
            return;
        if (ButtonController.Current == 2 || ButtonController.Current == 3)
            return;

        StopCoroutine(ChangeIcon());
    }

    // 다음 조작법을 보여주기
    public void NextHTP()
    {
        StopCoroutine(ChangeIcon());

        model.SetActive(false);
        ButtonController.Current++;
        if (ButtonController.Current < 0)
            ButtonController.Current = 4 + ButtonController.Current;
        ButtonController.Current %= 4;

        ChangeImage();
        return;
    }

    // 이전 조작법을 보여주기
    public void PrevHTP()
    {
        StopCoroutine(ChangeIcon());

        model.SetActive(false);
        ButtonController.Current--;
        if (ButtonController.Current < 0)
            ButtonController.Current = 4 + ButtonController.Current;
        ButtonController.Current %= 4;

        ChangeImage();
        return;
    }

    private void ChangeImage()
    {
        // 현재 인덱스에 맞는 설명으로 Update
        switch (ButtonController.Current)
        {
            case 0:
                model = this.transform.parent.Find("HTP1").gameObject;
                model.SetActive(true);
                break;
            case 1:
                model = this.transform.parent.Find("HTP2").gameObject;
                model.SetActive(true);
                break;
            case 2:
                model = this.transform.parent.Find("HTP3").gameObject;
                model.SetActive(true);
                break;
            case 3:
                model = this.transform.parent.Find("HTP4").gameObject;
                model.SetActive(true);
                break;
            default:
                break;
        }

        if (ButtonController.Current == 2 || ButtonController.Current == 3)
            return;

        StartCoroutine(ChangeIcon());
        return;
    }

    IEnumerator ChangeIcon()
    {
        int i = 0;
        Sprite change = Resources.Load<Sprite> ("Image/Icons/touch_icon") as Sprite; ;
        bool isTouch = true;

        if (model.name == "HTP2")
        {
            change = Resources.Load<Sprite>("Image/Icons/hold_icon") as Sprite;
            isTouch = false;
        }
        Sprite hand = Resources.Load<Sprite>("Image/Icons/hand_icon") as Sprite;

        Image image = model.transform.GetChild(0).GetComponent<Image>();

        while(true)
        {
            if (i % 2 == 0)
            {
                image.sprite = hand;
                yield return new WaitForSecondsRealtime(1.0f);
            }
            else
            {
                image.sprite = change;
                if(isTouch)
                    yield return new WaitForSecondsRealtime(0.5f);
                else
                    yield return new WaitForSecondsRealtime(1.5f);
            }

            i++;
        }
    }

    public void GameStart()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
