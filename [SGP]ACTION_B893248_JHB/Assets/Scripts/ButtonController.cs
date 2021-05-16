using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    static private GameObject model;
    static public int Current;

    private void Awake()
    {
        Current = 0;
        model = this.transform.parent.Find("HTP1").gameObject;
    }

    private void Start()
    {
        StartCoroutine(ChangeIcon());
    }

    void OnEnable()
    {
        if (ButtonController.Current == 2)
            return;

        StartCoroutine(ChangeIcon());
    }

    void OnDisable()
    {
        if (ButtonController.Current == 2)
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
            ButtonController.Current = 3 + ButtonController.Current;
        ButtonController.Current %= 3;

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
            ButtonController.Current = 3 + ButtonController.Current;
        ButtonController.Current %= 3;

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
            //case 3:
            //    model = this.transform.parent.Find("HTP4").gameObject;
            //    model.SetActive(true);
            //    break;
            default:
                break;
        }

        if (ButtonController.Current == 2)
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
                yield return new WaitForSeconds(1.0f);
            }
            else
            {
                image.sprite = change;
                if(isTouch)
                    yield return new WaitForSeconds(0.5f);
                else
                    yield return new WaitForSeconds(1.5f);
            }

            i++;
        }
    }
}
