using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public struct Count
    {
        public int ignite;      // ���� ��
        public int score;       // ����
        public int total_score; // �հ� ����
    };

    public Count last;          // ���� ����
    public Count best;          // �ְ� ����

    public static int QUOTA_SCORE = 1000;   // Ŭ��� �ʿ��� ����
    public GUIStyle guistyle;               // ��Ʈ ��Ÿ��

    // Start is called before the first frame update
    void Start()
    {
        this.last.ignite = 0;
        this.last.score = 0;
        this.last.total_score = 0;
        this.guistyle.fontSize = 16;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        int x = 20;
        int y = 50;
        GUI.color = Color.black;
        this.PrintValue(x + 20, y, "���� ī��Ʈ", this.last.ignite);
        y += 30;
        this.PrintValue(x + 20, y, "���� ���ھ�", this.last.ignite);
        y += 30;
        this.PrintValue(x + 20, y, "���� ���ھ�", this.last.ignite);
        y += 30;
    }

    public void PrintValue(int x, int y, string label, int value)
    {
        // label ǥ��
        GUI.Label(new Rect(x, y, 100, 20), label, guistyle);
        y += 15;

        // ���� �࿡ value ǥ��
        GUI.Label(new Rect(x + 20, y, 100, 20), value.ToString(), guistyle);
        y += 15;
    }

    // ���� Ƚ�� ����
    public void AddIgniteCount(int count)
    {
        this.last.ignite += count;  // ���� ���� count�ջ�
        this.UpdateScore();         // ���� ���
    }

    // ���� Ƚ���� ����
    public void ClearIgniteCount()
    {
        this.last.ignite = 0;       // ���� Ƚ�� ����
    }

    // ���� ���� ���
    private void UpdateScore()
    {
        this.last.score = this.last.ignite * 10;    // ���� ����
    }

    // �հ� ���� ����
    public void UpdateTotalScore()
    {
        this.last.total_score += this.last.score;
    }

    // ���� Ŭ���� ����(SceneControl���� ���)
    public bool IsGameClear()
    {
        bool is_clear = false;

        // ���� �հ� ������ Ŭ���� ���غ��� ũ��
        if(this.last.total_score > QUOTA_SCORE)
        {
            is_clear = true;
        }

        return is_clear;
    }
}
