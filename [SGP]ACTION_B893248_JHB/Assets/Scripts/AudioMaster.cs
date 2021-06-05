using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMaster : MonoBehaviour
{
    public AudioSource BGM;
    public Slider BGMSlider;
    public Slider SFXSlider;
    private float bgmVol = 0.5f;
    private Sprite[] sprites;

    private void Start()
    {
        // 볼륨 크기 가져오기
        if (PlayerPrefs.HasKey("BGM") == false)
            PlayerPrefs.SetFloat("BGM", 0.5f);

        bgmVol = PlayerPrefs.GetFloat("BGM");

        BGMSlider.value = bgmVol;
        BGM.volume = BGMSlider.value;

        sprites = new Sprite[3];
        sprites[0] = Resources.Load<Sprite>("Image/Icons/fire1") as Sprite;
        sprites[1] = Resources.Load<Sprite>("Image/Icons/fire2") as Sprite;
        sprites[2] = Resources.Load<Sprite>("Image/Icons/fire3") as Sprite;
    }

    public void AudioControl()
    {
        BGM.volume = BGMSlider.value;

        bgmVol = BGMSlider.value;
        PlayerPrefs.SetFloat("BGM", bgmVol);

        HideGauge(BGMSlider);
    }

    private void Update()
    {
        AudioControl();
    }

    public void HideGauge(Slider slider)
    {
        if (slider.value <= 0)
            slider.transform.Find("Fill Area").gameObject.SetActive(false);
        else
        {
            slider.transform.Find("Fill Area").gameObject.SetActive(true);
            if (slider.value >= 0.7f)
                slider.transform.Find("Handle Slide Area").transform.GetChild(0).GetComponent<Image>().sprite = sprites[2];
            else if (slider.value < 0.7f && slider.value >=0.3f)
                slider.transform.Find("Handle Slide Area").transform.GetChild(0).GetComponent<Image>().sprite = sprites[1];
            else
                slider.transform.Find("Handle Slide Area").transform.GetChild(0).GetComponent<Image>().sprite = sprites[0];
        }
    }
}
