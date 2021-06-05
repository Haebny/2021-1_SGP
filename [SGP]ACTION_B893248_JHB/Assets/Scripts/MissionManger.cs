using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManger : MonoBehaviour
{
    private PlayerControl player = null;

    public bool m1 = false;
    public bool m2 = false;
    public bool m3 = false;

    // 싱글톤으로 구현
    private static MissionManger instance = null;
    public static MissionManger Instance
    {
        get
        {
            if (null == instance)
            {
                //게임 인스턴스가 없다면 하나 생성해서 넣어준다.
                instance = new MissionManger();
            }
            return instance;
        }
    }

    public enum MISSION
    {
        BED = 0,
        CAT = 1,
        BATH,
        CHICKEN,
        CAR
    }

    private MISSION MissionType;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        PlayerPrefs.SetInt("Tumbling5", 0);
        PlayerPrefs.SetInt("Hopping5", 0);
        PlayerPrefs.SetInt("Rewards", 0);
        PlayerPrefs.SetInt("Clear", 0);
        if (PlayerPrefs.HasKey("Cat") == false)
            PlayerPrefs.SetInt("Cat", 0);
        //PlayerPrefs.SetInt("Chicken", 0);
    }

    // Update is called once per frame
    void Update()
    {
        // 플레이어 받아오기
        if(player == null && SceneManager.GetActiveScene().buildIndex == 1)
        {
            this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
            CreateMission();
        }

        // 플레이어가 없다면 반환
        if (player == null)
            return;

        CheckMission();
    }

    private void CreateMission()
    {
        // 고양이를 얻지 못했다면
        if(PlayerPrefs.GetInt("Cat") == 0)
        {
            PlayerPrefs.SetInt("Tumbling3", 0);
            PlayerPrefs.SetInt("Crushing3", 0);
            //PlayerPrefs.SetInt("Hopping1", 0);
        }
        //else if(PlayerPrefs.GetInt("CHICKENT")==0)
        //{
        //    PlayerPrefs.SetInt("Tumbling3", 0);
        //    PlayerPrefs.SetInt("Crushing5", 0);
        //    PlayerPrefs.SetInt("Hopping3", 0);
        //    PlayerPrefs.SetInt("Clear", 0);
        //}

        PlayerPrefs.SetInt("Clear", 0);
    }

    private void CheckMission()
    {
        // 고양이를 얻지 못했다면
        if (PlayerPrefs.GetInt("Cat") == 0)
        {
            if (PlayerPrefs.GetInt("Tumbling3") >= 3)
            {
                if(m1==false)
                {
                    m1 = true;
                    PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 500);
                }
            }
            if (PlayerPrefs.GetInt("Crushing3") >= 3)
            {
                if (m2 == false)
                {
                    m2 = true;
                    PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 500);
                }
            }
            //if (PlayerPrefs.GetInt("Hopping1") >= 1)
            //    m3 = true;

            // 미션 클리어
            if (m1==true && m2==true)
            {
                Debug.Log("GET CAT");
                PlayerPrefs.SetInt("Cat", 1);
                PlayerPrefs.SetInt("Clear", 1);
            }
        }
    }

    private void OnDisable()
    {
        m1 = false;
        m2 = false;
        m3 = false;
    }
}
