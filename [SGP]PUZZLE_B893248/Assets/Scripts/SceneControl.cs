using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControl : MonoBehaviour
{
    private BlockRoot block_root = null;
    private ScoreCounter score_counter = null;

    public enum STEP
    {
        NONE = -1,      // ���� ���� ����
        PLAY = 0,       // �÷��� ��
        CLEAR,          // Ŭ����
        NUM,            // ������ ����
    }

    public STEP step = STEP.NONE;       // ���� ����
    public STEP next_step = STEP.NONE;  // ���� ����
    public float step_timer = 0.0f;     // ��� �ð�
    private float clear_time = 0.0f;    // Ŭ���� �ð�
    public GUIStyle guistyle;           // ��Ʈ ��Ÿ��

    // Start is called before the first frame update
    void Start()
    {
        // BlockRoot ��ũ��Ʈ ��������
        this.block_root = this.gameObject.GetComponent<BlockRoot>();
        // Create() �޼��忡�� �ʱ� ����
        this.block_root.Create();
        // BlockRoot ��ũ��Ʈ�� initialSetup() ȣ��
        this.block_root.InitialSetUp();
        // ScoreCounter ��������
        this.score_counter = this.gameObject.GetComponent<ScoreCounter>();
        this.next_step = STEP.PLAY;     // ���� ���¸� �÷��� ������ ����
        this.guistyle.fontSize = 24;    // ��Ʈ ũ�⸦ 24�� ����
    }

    // Update is called once per frame
    void Update()
    {
        this.step_timer += Time.deltaTime;
        // ���º�ȭ ���
        if(this.next_step == STEP.NONE)
        {
            switch (this.step)
            {
                case STEP.PLAY:
                    // Ŭ���� ������ �����ϸ�
                    if(this.score_counter.IsGameClear())
                    {
                        this.next_step = STEP.CLEAR;    // Ŭ���� ���·� ����
                    }
                    break;
            }
        }

        //���°� ���ߴٸ�
        while(this.next_step != STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = STEP.NONE;
            switch (this.step)
            {
                case STEP.CLEAR:
                    // block_root�� ����
                    this.block_root.enabled = false;
                    // ��� �ð��� Ŭ���� �ð����� ����
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
                // ��� �ð� ǥ��
                GUI.Label(new Rect(40.0f, 10.0f, 200.0f, 20.0f),
                    "�ð�" + Mathf.CeilToInt(this.step_timer).ToString() + "��", guistyle);
                GUI.color = Color.white;
                break;
            case STEP.CLEAR:
                GUI.color = Color.black;
                // Ŭ���� ���� ǥ��
                GUI.Label(new Rect(Screen.width / 2.0f - 80.0f, 20.0f, 200.0f, 20.0f),
                    "��Ŭ����-!��", guistyle);
                // Ŭ���� �ð� ǥ��
                GUI.Label(new Rect(Screen.width / 2.0f - 80.0f, 40.0f, 200.0f, 20.0f),
                    "Ŭ���� �ð�" + Mathf.CeilToInt(this.clear_time).ToString() + "��", guistyle);
                GUI.color = Color.white;
                break;
        }
    }
}
