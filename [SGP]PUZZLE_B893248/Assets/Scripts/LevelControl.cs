using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    public float[] probability;     // 블록의 출현빈도를 저장하는 배열
    public float heat_time;         // 연소시간

    // 생성자
    public LevelData()
    {
        // 블록의 종류 수와 같은 크기로 데이터 영역 확보
        this.probability = new float[(int)Block.COLOR.NORMAL_COLOR_NUM];

        // 모든 종류의 출현확률을 우선 균등하게 해둔다
        for (int i = 0; i < (int)Block.COLOR.NORMAL_COLOR_NUM; i++)
        {
            this.probability[i] = 1.0f / (float)Block.COLOR.NORMAL_COLOR_NUM;
        }
    }

    // 모든 종류의 출현확률을 0으로 리셋하는 메소드
    public void Clear()
    {
        for (int i = 0; i < this.probability.Length; i++)
        {
            this.probability[i] = 0.0f;
        }
    }

    // 모든 종류의 출현확률의 합계를 100%(=1.0)로 하는 메소드
    public void Normalize()
    {
        float sum = 0.0f;

        // 출현확률의 '임시 합계값' 계산
        for (int i = 0; i < this.probability.Length; i++)
        {
            sum += this.probability[i];
        }

        for (int i = 0; i < this.probability.Length; i++)
        {
            // 각각의 출현율을 '임시 합계값'으로 나누면, 합계가 100%(=1.0)
            this.probability[i] /= sum;

            // 만약 그 값이 무한이면
            if (float.IsInfinity(this.probability[i]))
            {
                this.Clear();                   // 모든 확률을 0으로 리셋
                this.probability[0] = 1.0f;     // 최초의 요소만 1.0
                break;                          // 루프 탈출
            }
        }
    }
}

public class LevelControl
{
    private List<LevelData> level_datas = null;   // 각 레벨의 레벨 데이터
    private int select_level = 0;

    public void Initialize()
    {
        // List 초기화
        this.level_datas = new List<LevelData>();
    }

    public void LoadLevelData(TextAsset level_data_text)
    {
        // 텍스트 데이터를 문자열로 받음
        string level_texts = level_data_text.text;
        // 개행 코드'\'마다 나누어, 문자열 배열에 집어넣음
        string[] lines = level_texts.Split('\n');
        // lines 안의 각 행에 대하여 차례로 처리해가는 루프
        foreach (var line in lines)
        {
            // 행이 비어있으면
            if (line == "")
            {
                continue;   // 루프의 처음으로 점프
            }

            string[] words = line.Split();  // 행 내의 워드를 배열에 저장
            int n = 0;

            // 현재 처리하는 행의 데이터 넣기
            LevelData level_data = new LevelData();

            // words내의 각 워드에 대해서 순서대로 처리하는 루프
            foreach (var word in words)
            {
                // 시작하는 문자가 #인 경우
                if(word.StartsWith("#"))
                {
                    break;
                }

                // 워드가 비어있는 경우
                if (word == "")
                {
                    continue;
                }

                // 'n'의 값을 변경하여 일곱 개의 항목 처리
                // 각 워드를 float값으로 변환하고 level_data에 저장
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

            // 8항목(이상)이 제대로 처리된 경우
            if(n>=7)
            {
                // 출현 확률의 합계가 정확히 100%가 되도록 하고 나서
                level_data.Normalize();

                // List 구조의 level_datas에 level_data를 추가
                this.level_datas.Add(level_data);
            }
            // 오류 가능성이 있는 경우
            else
            {
                // 1워드도 처리하지 않은 경우 == 주석
                if(n==0)
                {
                    // 문제없음
                }
                else
                {
                    // 데이터의 개수가 맞지 않는다는 오류 메시지 표시
                    Debug.LogError("[LevelData] Out of parameter.\n");
                }
            }
        }

        // level_datas에 데이터가 하나도 없으면
        if (this.level_datas.Count == 0)
        {
            // 오류 메시지 표시
            Debug.LogError("[LevelData] Has no data.\n");
            // level_datas에 LevelData를 하나 추가
            this.level_datas.Add(new LevelData());
        }

    }

    public void SelectLevel()
    {
        // 0~패턴 사이의 값을 임의로 선택
        this.select_level = Random.Range(0, this.level_datas.Count);
        Debug.Log("select level = " + this.select_level.ToString());
    }

    public LevelData GetCurrentLevelData()
    {
        // 선택된 패턴의 레벨 데이터 반환
        return this.level_datas[this.select_level];
    }

    public float GetVanishTime()
    {
        // 선택된 패턴의 연소시간을 반환
        return (this.level_datas[this.select_level].heat_time);
    }
}
