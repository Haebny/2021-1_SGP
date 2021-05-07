using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControl : MonoBehaviour
{
    private BlockRoot block_root = null;
    private ScoreCounter score_counter = null;

    public enum STEP
    {
        NONE = -1,      // 상태 정보 없음
        PLAY = 0,       // 플레이 중
        CLEAR,          // 클리어
        NUM,            // 상태의 개수
    }

    public STEP step = STEP.NONE;       // 현재 상태
    public STEP next_step = STEP.NONE;  // 다음 상태
    public float step_timer = 0.0f;     // 경과 시간
    private float clear_time = 0.0f;    // 클리어 시간
    public GUIStyle guistyle;           // 폰트 스타일

    // Start is called before the first frame update
    void Start()
    {
        // BlockRoot 스크립트 가져오기
        this.block_root = this.gameObject.GetComponent<BlockRoot>();
        // Create() 메서드에서 초기 설정
        //this.block_root.Create();
        // BlockRoot 스크립트의 initialSetup() 호출
        this.block_root.InitialSetUp();
        // ScoreCounter 가져오기
        this.score_counter = this.gameObject.GetComponent<ScoreCounter>();
        this.next_step = STEP.PLAY;     // 다음 상태를 플레이 중으로 변경
        this.guistyle.fontSize = 24;    // 폰트 크기를 24로 변경
    }

    // Update is called once per frame
    void Update()
    {
        this.step_timer += Time.deltaTime;
        // 상태변화 대기
        if (this.next_step == STEP.NONE)
        {
            switch (this.step)
            {
                case STEP.PLAY:
                    // 클리어 조건을 만족하면
                    if (this.score_counter.IsGameClear())
                    {
                        this.next_step = STEP.CLEAR;    // 클리어 상태로 변경
                    }
                    break;
            }
        }

        //상태가 변했다면
        while (this.next_step != STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = STEP.NONE;
            switch (this.step)
            {
                case STEP.CLEAR:
                    // block_root를 정지
                    this.block_root.enabled = false;
                    // 경과 시간을 클리어 시간으로 설정
                    this.clear_time = this.step_timer;
                    break;
            }
            this.step_timer = 0.0f;
        }
    }

    private void OnGUI()
    {
        switch (this.step)
        {
            case STEP.PLAY:
                GUI.color = Color.black;
                // 경과 시간 표시
                GUI.Label(new Rect(40.0f, 10.0f, 200.0f, 20.0f),
                    "시간" + Mathf.CeilToInt(this.step_timer).ToString() + "초", guistyle);
                GUI.color = Color.white;
                break;
            case STEP.CLEAR:
                GUI.color = Color.black;
                // 클리어 문구 표시
                GUI.Label(new Rect(Screen.width / 2.0f - 80.0f, 20.0f, 200.0f, 20.0f),
                    "☆클리어-!☆", guistyle);
                // 클리어 시간 표시
                GUI.Label(new Rect(Screen.width / 2.0f - 80.0f, 40.0f, 200.0f, 20.0f),
                    "클리어 시간" + Mathf.CeilToInt(this.clear_time).ToString() + "초", guistyle);
                GUI.color = Color.white;
                break;
        }
    }
}
