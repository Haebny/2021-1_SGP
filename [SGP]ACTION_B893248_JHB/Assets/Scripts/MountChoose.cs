using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MountChoose : MonoBehaviour
{
    public static int index;

    private void Start()
    {
        MountChoose.index = 0;
        ChooseMount();
    }

    private void Update()
    {
        ChooseMount();
    }

    public void ChooseMount()
    {
        // 선택범위 설정(획득 리워드만 탐색 가능)
        GameObject mount = GameObject.Find("Main Camera").transform.Find("Mount").gameObject;
        string name = " ";

        for (int i = 0; i < mount.transform.childCount; i++)
        {
            if (i == MountChoose.index)
            {
                mount.transform.GetChild(i).gameObject.SetActive(true);
                name = mount.transform.GetChild(i).gameObject.name;
            }
            else
                mount.transform.GetChild(i).gameObject.SetActive(false);
        }

        TextMeshProUGUI tmPro = GameObject.Find("UI").transform.Find("Mount").GetChild(0).GetComponent<TextMeshProUGUI>();
        tmPro.text = "Mount : " + name;

        PlayerPrefs.SetString("Mount", name);
    }

    public void CharacterNextButton()
    {
        if (PlayerPrefs.GetInt("Bed") == 0)
            return;

        MountChoose.index++;
        if (MountChoose.index >= PlayerPrefs.GetInt("mCount"))
            MountChoose.index = PlayerPrefs.GetInt("mCount");
        ChooseMount();
    }

    public void CharacterPrevButton()
    {
        if (PlayerPrefs.GetInt("Bed") == 0)
            return;

        MountChoose.index--;
        if (MountChoose.index < 0)
            MountChoose.index = 0;
        ChooseMount();
    }
}
