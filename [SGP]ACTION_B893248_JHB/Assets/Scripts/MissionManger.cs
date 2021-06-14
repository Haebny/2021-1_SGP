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

    private bool isClear = false; 
    public bool isSetup = false; 

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
        TABLE,
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

        // 저장 변수 세팅
        SetPreferences();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1 && isSetup == false)
        {
            isSetup = true;
            isClear= false;
            PlayerPrefs.SetInt("Clear", 0);
            PlayerPrefs.SetInt("Tumbling", 0);     // 텀블링 초기화
            PlayerPrefs.SetInt("Crushing", 0);     // 장애물 파괴 초기화
            PlayerPrefs.SetInt("Hopping", 0);     // 장애물 밟기 초기화
        }

        if (isClear == true)
            return;
        
        // 플레이어 받아오기
        if (player == null && SceneManager.GetActiveScene().buildIndex == 1)
        {
            this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
            CreateMission();
        }
        else
            isClear = false;

        // 플레이어가 없다면 반환
        if (player == null)
            return;

        CheckMission();
    }

    private void SetPreferences()
    {
        PlayerPrefs.SetInt("Clear", 0);

        // 탑승물-침대 획득 유무
        if (PlayerPrefs.HasKey("Bed") == false)
            PlayerPrefs.SetInt("Bed", 0);

        // 캐릭터-고양이 획득 유무
        if (PlayerPrefs.HasKey("Cat") == false)
            PlayerPrefs.SetInt("Cat", 0);

        // 탑승물-욕조 획득 유무
        if (PlayerPrefs.HasKey("Table") == false)
            PlayerPrefs.SetInt("Table", 0);

        // 캐릭터-닭 획득 유무
        if (PlayerPrefs.HasKey("Chicken") == false)
            PlayerPrefs.SetInt("Chicken", 0);

        // 탑승물-슈퍼카 획득 유무
        if (PlayerPrefs.HasKey("Car") == false)
            PlayerPrefs.SetInt("Car", 0);
    }

    private void CreateMission()
    {
        // 침대를 얻지 못했다면
        if (PlayerPrefs.GetInt("Bed") == 0)
        {
            PlayerPrefs.SetInt("Tumbling", 0);     // 텀블링 3회
            PlayerPrefs.SetInt("Crushing", 0);     // 장애물 파괴 1회
        }

        // 고양이를 얻지 못했다면
        else if (PlayerPrefs.GetInt("Cat") == 0)
        {
            PlayerPrefs.SetInt("Tumbling", 0);     // 텀블링 3회
            PlayerPrefs.SetInt("Crushing", 0);     // 장애물 파괴 3회
        }

        // 욕조를 얻지 못했다면
        else if(PlayerPrefs.GetInt("Table")==0)
        {
            PlayerPrefs.SetInt("Tumbling", 0);     // 텀블링 5회
            PlayerPrefs.SetInt("Crushing", 0);     // 장애물 파괴 3회
            PlayerPrefs.SetInt("Hopping", 0);      // 장애물 밟기 1회
        }

        // 닭을 얻지 못했다면
        else if (PlayerPrefs.GetInt("Chicken") == 0)
        {
            PlayerPrefs.SetInt("Tumbling", 0);     // 텀블링 5회
            PlayerPrefs.SetInt("Crushing", 0);     // 장애물 파괴 5회
            PlayerPrefs.SetInt("Hopping", 0);      // 장애물 밟기 3회
        }

        // 슈퍼카를 얻지 못했다면
        else if (PlayerPrefs.GetInt("Car") == 0)
        {
            PlayerPrefs.SetInt("Tumbling", 0);     // 텀블링 5회
            PlayerPrefs.SetInt("Crushing", 0);     // 장애물 파괴 5회
            PlayerPrefs.SetInt("Hopping", 0);      // 장애물 밟기 5회
        }

        PlayerPrefs.SetInt("Clear", 0);
    }

    private void CheckMission()
    {
        if (isClear == true)
            return;

        // 1. 침대를 얻지 못했다면
        if (PlayerPrefs.GetInt("Bed") == 0 && isClear == false)
        {
            // 텀블링 3회
            if (PlayerPrefs.GetInt("Tumbling") >= 3)
            {
                if(m1==false)
                {
                    m1 = true;
                    PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 500);
                }
            }

            // 장애물 파괴 1회
            if (PlayerPrefs.GetInt("Crushing") >= 1)
            {
                if (m2 == false)
                {
                    m2 = true;
                    PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 500);
                }
            }

            // 미션 클리어
            if (m1==true && m2==true)
            {
                PlayerPrefs.SetInt("Bed", 1);
                PlayerPrefs.SetInt("Clear", 1);
                isClear = true;
                PlayerPrefs.SetInt("mCount", 1);
            }
        }

        // 2. 고양이를 얻지 못했다면
        else if(PlayerPrefs.GetInt("Cat")== 0 && isClear == false)
        {
            if (PlayerPrefs.GetInt("Tumbling") >= 3)
            {
                if (m1 == false)
                {
                    m1 = true;
                    PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 500);
                }
            }

            if (PlayerPrefs.GetInt("Crushing") >= 3)
            {
                if (m2 == false)
                {
                    m2 = true;
                    PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 500);
                }
            }

            // 미션 클리어
            if (m1 && m2)
            {
                PlayerPrefs.SetInt("Cat", 1);
                PlayerPrefs.SetInt("Clear", 1);
                isClear = true;
                PlayerPrefs.SetInt("aCount", 1);
            }
        }

        // 3. 욕조를 얻지 못했다면
        else if (PlayerPrefs.GetInt("Table") == 0 && isClear == false)
        {
            // 텀블링 3회
            if (PlayerPrefs.GetInt("Tumbling") >= 5)
            {
                if (m1 == false)
                {
                    m1 = true;
                    PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 500);
                }
            }

            // 장애물 파괴 1회
            if (PlayerPrefs.GetInt("Crushing") >= 3)
            {
                if (m2 == false)
                {
                    m2 = true;
                    PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 500);
                }
            }

            // 장애물 밟기 
            if (PlayerPrefs.GetInt("Hopping") >= 1)
            {
                if (m3 == false)
                {
                    m3 = true;
                    PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 500);
                }
            }

            // 미션 클리어
            if (m1 == true && m2 == true && m3==true)
            {
                PlayerPrefs.SetInt("Table", 1);
                PlayerPrefs.SetInt("Clear", 1);
                isClear = true;
                PlayerPrefs.SetInt("mCount", 2);
            }
        }

        // 4. 닭을 얻지 못했다면
        else if (PlayerPrefs.GetInt("Chicken") == 0 && isClear == false)
        {
            if (PlayerPrefs.GetInt("Tumbling") >= 5)
            {
                if (m1 == false)
                {
                    m1 = true;
                    PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 500);
                }
            }

            if (PlayerPrefs.GetInt("Crushing") >= 5)
            {
                if (m2 == false)
                {
                    m2 = true;
                    PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 500);
                }
            }

            if (PlayerPrefs.GetInt("Hopping") >= 3)
            {
                if (m3 == false)
                {
                    m3 = true;
                    PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 500);
                }
            }

            // 미션 클리어
            if (m1 && m2 && m3)
            {
                PlayerPrefs.SetInt("Chicken", 1);
                PlayerPrefs.SetInt("Clear", 1);
                isClear = true;
                PlayerPrefs.SetInt("aCount", 2);
            }
        }

        // 5. 슈퍼카를 얻지 못했다면
        else if (PlayerPrefs.GetInt("Car") == 0 && isClear == false)
        {
            if (PlayerPrefs.GetInt("Tumbling") >= 5)
            {
                if (m1 == false)
                {
                    m1 = true;
                    PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 500);
                }
            }

            if (PlayerPrefs.GetInt("Crushing") >= 5)
            {
                if (m2 == false)
                {
                    m2 = true;
                    PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 500);
                }
            }

            if (PlayerPrefs.GetInt("Hopping") >= 5)
            {
                if (m3 == false)
                {
                    m3 = true;
                    PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 500);
                }
            }

            // 미션 클리어
            if (m1 && m2 && m3)
            {
                PlayerPrefs.SetInt("Car", 1);
                PlayerPrefs.SetInt("Clear", 1);
                isClear = true;
                PlayerPrefs.SetInt("mCount", 3);
            }
        }

        else
        {
            m1 = false;
            m2 = true;
            m3 = false;
        }
    }

    private void OnDisable()
    {
        m1 = false;
        m2 = false;
        m3 = false;
    }
}
