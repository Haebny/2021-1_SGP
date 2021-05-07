using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public struct Count
    {
        public int ignite;      // 연쇄 수
        public int score;       // 점수
        public int total_score; // 합계 점수
    };

    public Count last;          // 최종 점수
    public Count best;          // 최고 점수

    public static int QUOTA_SCORE = 1000;   // 클리어에 필요한 점수
    public GUIStyle guistyle;               // 폰트 스타일

    // Start is called before the first frame update
    void Start()
    {
        this.last.ignite = 0;
        this.last.score = 0;
        this.last.total_score = 0;
        this.guistyle.fontSize = 16;
    }

    private void OnGUI()
    {
        int x = 20;
        int y = 50;
        GUI.color = Color.black;
        this.PrintValue(x + 20, y, "연쇄 카운트", this.last.ignite);
        y += 30;
        this.PrintValue(x + 20, y, "가산 스코어", this.last.score);
        y += 30;
        this.PrintValue(x + 20, y, "최종 스코어", this.last.total_score);
        y += 30;
    }

    public void PrintValue(int x, int y, string label, int value)
    {
        // label 표시
        GUI.Label(new Rect(x, y, 100, 20), label, guistyle);
        y += 15;

        // 다음 행에 value 표시
        GUI.Label(new Rect(x + 20, y, 100, 20), value.ToString(), guistyle);
        y += 15;
    }

    // 연쇄 횟수 가산
    public void AddIgniteCount(int count)
    {
        this.last.ignite += count;  // 연쇄 수에 count합산
        this.UpdateScore();         // 점수 계산
    }

    // 연쇄 횟수를 리셋
    public void ClearIgniteCount()
    {
        this.last.ignite = 0;       // 연쇄 횟수 리셋
    }

    // 더할 점수 계산
    private void UpdateScore()
    {
        this.last.score = this.last.ignite * 10;    // 점수 갱신
    }

    // 합계 점수 갱신
    public void UpdateTotalScore()
    {
        this.last.total_score += this.last.score;
    }

    // 게임 클리어 판정(SceneControl에서 사용)
    public bool IsGameClear()
    {
        bool is_clear = false;

        // 현재 합계 점수가 클리어 기준보다 크면
        if (this.last.total_score > QUOTA_SCORE)
        {
            is_clear = true;
        }

        return is_clear;
    }
}
