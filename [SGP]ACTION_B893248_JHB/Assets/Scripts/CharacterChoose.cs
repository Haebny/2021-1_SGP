using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterChoose : MonoBehaviour
{
    public static int index;

    private void Start()
    {
        CharacterChoose.index = 0;
        ChooseCharacter();
    }

    public void ChooseCharacter()
    {
        GameObject player = GameObject.Find("Player");
        string name = " ";
        if (CharacterChoose.index < 0)
            CharacterChoose.index = Mathf.Abs(player.transform.childCount + CharacterChoose.index);

        for (int i = 0; i < player.transform.childCount; i++)
        {
            if (i == CharacterChoose.index)
            {
                player.transform.GetChild(i).gameObject.SetActive(true);
                name = player.transform.GetChild(i).gameObject.name;
            }
            else
                player.transform.GetChild(i).gameObject.SetActive(false);
        }

        TextMeshProUGUI tmPro = GameObject.Find("UI").transform.Find("Animal").GetChild(0).GetComponent<TextMeshProUGUI>();
        tmPro.text = "Animal : " + name;

        PlayerPrefs.SetString("Animal", name);
    }

    public void CharacterNextButton()
    {
        if (PlayerPrefs.GetInt("Cat") == 0)
            return;

        CharacterChoose.index++;
        if(CharacterChoose.index >1)
            CharacterChoose.index = CharacterChoose.index % 2;
        ChooseCharacter();
    }

    public void CharacterPrevButton()
    {
        if (PlayerPrefs.GetInt("Cat") == 0)
            return;

        CharacterChoose.index--;
        ChooseCharacter();
    }
}
