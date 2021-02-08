using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    public float[] probability;     // ����� �����󵵸� �����ϴ� �迭
    public float heat_time;         // ���ҽð�

    // ������
    public LevelData()
    {
        // ����� ���� ���� ���� ũ��� ������ ���� Ȯ��
        this.probability = new float[(int)Block.COLOR.NORMAL_COLOR_NUM];

        // ��� ������ ����Ȯ���� �켱 �յ��ϰ� �صд�
        for (int i = 0; i < (int)Block.COLOR.NORMAL_COLOR_NUM; i++)
        {
            this.probability[i] = 1.0f / (float)Block.COLOR.NORMAL_COLOR_NUM;
        }
    }

    // ��� ������ ����Ȯ���� 0���� �����ϴ� �޼ҵ�
    public void Clear()
    {
        for (int i = 0; i < this.probability.Length; i++)
        {
            this.probability[i] = 0.0f;
        }
    }

    // ��� ������ ����Ȯ���� �հ踦 100%(=1.0)�� �ϴ� �޼ҵ�
    public void Normalize()
    {
        float sum = 0.0f;

        // ����Ȯ���� '�ӽ� �հ谪' ���
        for (int i = 0; i < this.probability.Length; i++)
        {
            sum += this.probability[i];
        }

        for (int i = 0; i < this.probability.Length; i++)
        {
            // ������ �������� '�ӽ� �հ谪'���� ������, �հ谡 100%(=1.0)
            this.probability[i] /= sum;

            // ���� �� ���� �����̸�
            if (float.IsInfinity(this.probability[i]))
            {
                this.Clear();                   // ��� Ȯ���� 0���� ����
                this.probability[0] = 1.0f;     // ������ ��Ҹ� 1.0
                break;                          // ���� Ż��
            }
        }
    }
}

public class LevelControl
{
    private List<LevelData> level_datas = null;   // �� ������ ���� ������
    private int select_level = 0;

    public void Initialize()
    {
        // List �ʱ�ȭ
        this.level_datas = new List<LevelData>();
    }

    public void LoadLevelData(TextAsset level_data_text)
    {
        // �ؽ�Ʈ �����͸� ���ڿ��� ����
        string level_texts = level_data_text.text;
        // ���� �ڵ�'\'���� ������, ���ڿ� �迭�� �������
        string[] lines = level_texts.Split('\n');
        // lines ���� �� �࿡ ���Ͽ� ���ʷ� ó���ذ��� ����
        foreach (var line in lines)
        {
            // ���� ���������
            if (line == "")
            {
                continue;   // ������ ó������ ����
            }

            string[] words = line.Split();  // �� ���� ���带 �迭�� ����
            int n = 0;

            // ���� ó���ϴ� ���� ������ �ֱ�
            LevelData level_data = new LevelData();

            // words���� �� ���忡 ���ؼ� ������� ó���ϴ� ����
            foreach (var word in words)
            {
                // �����ϴ� ���ڰ� #�� ���
                if(word.StartsWith("#"))
                {
                    break;
                }

                // ���尡 ����ִ� ���
                if (word == "")
                {
                    continue;
                }

                // 'n'�� ���� �����Ͽ� �ϰ� ���� �׸� ó��
                // �� ���带 float������ ��ȯ�ϰ� level_data�� ����
                switch (n)
                {
                    case 0:
                        level_data.probability[(int)Block.COLOR.PINK] = float.Parse(word);
                        break;
                    case 1:
                        level_data.probability[(int)Block.COLOR.BLUE] = float.Parse(word);
                        break;
                    case 2:
                        level_data.probability[(int)Block.COLOR.GREEN] = float.Parse(word);
                        break;
                    case 3:
                        level_data.probability[(int)Block.COLOR.ORANGE] = float.Parse(word);
                        break;
                    case 4:
                        level_data.probability[(int)Block.COLOR.YELLOW] = float.Parse(word);
                        break;
                    case 5:
                        level_data.probability[(int)Block.COLOR.MAGENTA] = float.Parse(word);
                        break;
                    case 6:
                        level_data.heat_time = float.Parse(word);
                        break;
                }
                n++;
            }

            // 8�׸�(�̻�)�� ����� ó���� ���
            if(n>=7)
            {
                // ���� Ȯ���� �հ谡 ��Ȯ�� 100%�� �ǵ��� �ϰ� ����
                level_data.Normalize();

                // List ������ level_datas�� level_data�� �߰�
                this.level_datas.Add(level_data);
            }
            // ���� ���ɼ��� �ִ� ���
            else
            {
                // 1���嵵 ó������ ���� ��� == �ּ�
                if(n==0)
                {
                    // ��������
                }
                else
                {
                    // �������� ������ ���� �ʴ´ٴ� ���� �޽��� ǥ��
                    Debug.LogError("[LevelData] Out of parameter.\n");
                }
            }
        }

        // level_datas�� �����Ͱ� �ϳ��� ������
        if (this.level_datas.Count == 0)
        {
            // ���� �޽��� ǥ��
            Debug.LogError("[LevelData] Has no data.\n");
            // level_datas�� LevelData�� �ϳ� �߰�
            this.level_datas.Add(new LevelData());
        }

    }

    public void SelectLevel()
    {
        // 0~���� ������ ���� ���Ƿ� ����
        this.select_level = Random.Range(0, this.level_datas.Count);
        Debug.Log("select level = " + this.select_level.ToString());
    }

    public LevelData GetCurrentLevelData()
    {
        // ���õ� ������ ���� ������ ��ȯ
        return this.level_datas[this.select_level];
    }

    public float GetVanishTime()
    {
        // ���õ� ������ ���ҽð��� ��ȯ
        return (this.level_datas[this.select_level].heat_time);
    }
}
