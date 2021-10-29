using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LevelControl class 앞에 추가
public class LevelData
{
    public struct Range
    {
        // 범위를 표현하는 구조체.
        public int min; // 범위의 최솟값.
        public int max; // 범위의 최댓값.
    };

    public float end_time; // 종료 시간.
    public float player_speed; // 플레이어의 속도.

    public Range floor_count; // 발판 블록 수의 범위.
    public Range height_diff; // 발판의 높이 범위.

    public LevelData()
    {
        this.end_time = 15.0f; // 종료 시간 초기화.
        this.player_speed = 6.0f; // 플레이어의 속도 초기화.
        this.floor_count.min = 10; // 발판 블록 수의 최솟값을 초기화.
        this.floor_count.max = 10; // 발판 블록 수의 최댓값을 초기화.
        this.height_diff.min = 0; // 발판 높이 변화의 최솟값을 초기화.
        this.height_diff.max = 0; // 발판 높이 변화의 최댓값을 초기화.
    }
}

public class LevelControl : MonoBehaviour
{
    // 만들어야 할 블록에 관한 정보를 모은 구조체.
    public struct CreationInfo
    {
        public Block.TYPE block_type; // 블록의 종류.
        public int max_count; // 블록의 최대 개수.
        public float height; // 블록을 배치할 높이.
        public int current_count; // 작성한 블록의 개수.
    };

    public CreationInfo previous_block; // 이전에 어떤 블록을 만들었는가.
    public CreationInfo current_block; // 지금 어떤 블록을 만들어야 하는가.
    public CreationInfo next_block; // 다음에 어떤 블록을 만들어야 하는가.
    public int block_count; // 생성한 블록의 총 수.
    public static int level;
    public bool isChanged;
    public Queue<Block.TYPE> typeQueue;

    private static LevelControl instance;
    public static LevelControl GetInstance()
    {
        if (instance == null)
        {
            instance = new LevelControl();
        }
        return instance;
    }

    private void Start()
    {
        block_count = 0;
        LevelControl.level = 1;
        Debug.Log("LEVEL: " + level);
        isChanged = false;

        typeQueue = new Queue<Block.TYPE>();
    }

    public void LevelUp()
    {
        if (isChanged == false)
        {
            isChanged = true;
            LevelControl.level += 1;
        }
    }

    //프로필 노트에 실제로 기록하는 처리를 한다.
    private void ClearNextBlock(ref CreationInfo block)
    {
        // 전달받은 블록(block)을 초기화.
        block.block_type = Block.TYPE.FLOOR;
        block.max_count = 15;
        block.height = 1;
        block.current_count = 0;
    }

    // 프로필 노트를 초기화한다.
    public void Initialize()
    {
        this.block_count = 0; // 블록의 총 수를 초기화.

        // 이전, 현재, 다음 블록을 각각 ClearNextBlock()에 넘겨서 초기화한다.
        this.ClearNextBlock(ref this.previous_block);
        this.ClearNextBlock(ref this.current_block);
        this.ClearNextBlock(ref this.next_block);
    }
    // ref: 참조에 의해 인수를 전달할 때는 호출하는 쪽과 호출되는
    // 쪽 모두 인수에 필요

    private void UpdateLevel(ref CreationInfo current, CreationInfo previous)
    {
        switch (LevelControl.level)
        {
            case 1:
                SetLevel1(ref current, previous);
                break;
            case 2:
                SetLevel2(ref current, previous);
                break;
            case 3:
                SetLevel3(ref current, previous);
                break;
        }
    }

    public void update()
    {
        // *Update()가 아님. CreateFloorBlock() 메서드에서 호출
        this.current_block.current_count++; // 이번에 만든 블록 개수를 증가.

        // 이번에 만든 블록 개수가 max_count 이상이면.
        if (this.current_block.current_count >= this.current_block.max_count)
        {
            this.previous_block = this.current_block;
            this.current_block = this.next_block;
            this.ClearNextBlock(ref this.next_block); // 다음에 만들 블록의 내용을 초기화.

            this.UpdateLevel(ref this.next_block, this.current_block); // 다음에 만들 블록을 설정.
            //this.UpdateLevel(ref this.next_block, this.current_block, passage_time);
        }
        this.block_count++; // 블록의 총 수를 증가.
    }

    //public void LoadLevelData(TextAsset level_data_text)
    //{
    //    string level_texts = level_data_text.text; // 텍스트 데이터를 문자열로 가져온다.
    //    string[] lines = level_texts.Split('\n'); // 개행 코드 '\'마다 분할해서 문자열 배열에 넣는다.

    //    // lines 내의 각 행에 대해서 차례로 처리해 가는 루프.
    //    foreach (var line in lines)
    //    {
    //        if (line == "") // 행이 빈 줄이면.
    //        {
    //            continue; // 아래 처리는 하지 않고 반복문의 처음으로 점프한다.
    //        };

    //        //Debug.Log(line); // 행의 내용을 디버그 출력한다.
    //        string[] words = line.Split(); // 행 내의 워드를 배열에 저장한다.
    //        int n = 0;

    //        // LevelData형 변수를 생성한다.
    //        // 현재 처리하는 행의 데이터를 넣어 간다.
    //        LevelData level_data = new LevelData();

    //        // words내의 각 워드에 대해서 순서대로 처리해 가는 루프.
    //        foreach (var word in words)
    //        {
    //            if (word.StartsWith("#"))
    //            {
    //                // 워드의 시작문자가 #이면 루프 탈출.
    //                break;
    //            } 
    //            if (word == "")
    //            {
    //                // 워드가 텅 비었으면.
    //                continue;
    //            } 

    //            // 루프의 시작으로 점프한다.
    //            // n 값을 0, 1, 2,...7로 변화시켜 감으로써 8항목을 처리한다.
    //            // 각 워드를 플롯값으로 변환하고 level_data에 저장한다.
    //            switch (n)
    //            {
    //                case 0: level_data.end_time = float.Parse(word); break;
    //                case 1: level_data.player_speed = float.Parse(word); break;
    //                case 2: level_data.floor_count.min = int.Parse(word); break;
    //                case 3: level_data.floor_count.max = int.Parse(word); break;
    //                case 4: level_data.obstacle = int.Parse(word); break;
    //                case 5: level_data.height_diff.min = int.Parse(word); break;
    //                case 6: level_data.height_diff.max = int.Parse(word); break;
    //            }
    //            n++;
    //        }

    //        if (n >= 8)
    //        {
    //            // 8항목(이상)이 제대로 처리되었다면.
    //            this.level_datas.Add(level_data); // List 구조의 level_datas에 level_data를 추가한다.
    //        }
    //        else
    //        {
    //            // 그렇지 않다면(오류의 가능성이 있다).
    //            if (n == 0)
    //            {
    //                // 1워드도 처리하지 않은 경우는 주석이므로.
    //                // 문제없다. 아무것도 하지 않는다.
    //            }
    //            else
    //            { 
    //                // 그 이외이면 오류다.
    //                // 데이터 개수가 맞지 않다는 것을 보여주는 오류 메시지를 표시한다.
    //                Debug.LogError("[LevelData] Out of parameter.\n");
    //            }
    //        }
    //    }

    //    if (this.level_datas.Count == 0)
    //    { 
    //        // level_datas에 데이터가 하나도 없으면.
    //        Debug.LogError("[LevelData] Has no data.\n"); // 오류 메시지를 표시한다.
    //        this.level_datas.Add(new LevelData()); // level_datas에 기본 LevelData를 하나 추가해 둔다.
    //    }
    //}

    //private void UpdateLevel(ref CreationInfo current, CreationInfo previous, float passage_time)
    //{
    //    // 새 인수 passage_time으로 플레이 경과 시간을 받는다.
    //    // 레벨 1~레벨 5를 반복한다.
    //    float local_time = Mathf.Repeat(passage_time, this.level_datas[this.level_datas.Count - 1].end_time);

    //    // 현재 레벨을 구한다.
    //    int i;
    //    for (i = 0; i < this.level_datas.Count - 1; i++)
    //    {
    //        if (local_time <= this.level_datas[i].end_time)
    //        {
    //            break;
    //        }
    //    }

    //    this.level = i;
    //    current.block_type = Block.TYPE.FLOOR;
    //    current.max_count = 1;
    //    if (this.block_count >= 10)
    //    {
    //        // 현재 레벨용 레벨 데이터를 가져온다.
    //        LevelData level_data;
    //        level_data = this.level_datas[this.level];
    //        switch (previous.block_type)
    //        {
    //            case Block.TYPE.FLOOR: // 이전 블록이 바닥인 경우.
    //                current.block_type = Block.TYPE.OBSTACLE_F; // 이번엔 장애물을 만든다.

    //                // 장애물은 연속으로 생성되지 않음 = 1개만 생성
    //                current.max_count = 1;
    //                current.height = previous.height; // 높이를 이전과 같이 한다.
    //                break;
    //            case Block.TYPE.OBSTACLE_F: // 이전 블록이 장애물인 경우.
    //                current.block_type = Block.TYPE.FLOOR; // 이번엔 바닥을 만든다.

    //                // 바닥 길이의 최솟값~최댓값 사이의 임의의 값.
    //                current.max_count = Random.Range(level_data.floor_count.min, level_data.floor_count.max);

    //                // 바닥 높이의 최솟값과 최댓값을 구한다.
    //                int height_min = previous.height + level_data.height_diff.min;
    //                int height_max = previous.height + level_data.height_diff.max;
    //                height_min = Mathf.Clamp(height_min, HEIGHT_MIN, HEIGHT_MAX); // 최소와 최대값 사이를 강제로 지정
    //                height_max = Mathf.Clamp(height_max, HEIGHT_MIN, HEIGHT_MAX);

    //                // 바닥 높이 = 경사면이 생성된 만큼 낮음
    //                current.height = Random.Range(height_min, height_max);
    //                break;
    //        }
    //    }
    //}

    public void SetLevel1(ref CreationInfo current, CreationInfo previous)
    {
        int rand = Random.Range(1, 101);

        switch (previous.block_type)
        {
            // 이번 블록이 일반 평지일 경우.
            case Block.TYPE.FLOOR:
            case Block.TYPE.FLOOR1:
            case Block.TYPE.FLOOR2:
            case Block.TYPE.FLOOR3:
            case Block.TYPE.FLOOR4:

                // 일정 확률로 장애물 오브젝트 등장
                if (rand % 2 == 0)
                {
                    current.block_type = Block.TYPE.OBSTACLE_R; // 다음 번은 평지 장애물을 만든다.
                    current.max_count = 1; // 평지 장애물은 1개 만든다.
                    current.height = previous.height; // 높이를 이전과 같게 한다.
                }

                // 일정 확률로 열쇠 등장
                else if (rand % 3 == 0)
                {
                    current.block_type = Block.TYPE.KEY; // 다음 번은 열쇠를 만든다.
                    current.max_count = 1; // 열쇠는 1개 만든다.
                    current.height = previous.height; // 높이를 이전과 같게 한다.
                }

                // 일정 확률로 박스 등장
                else if (rand % 5 == 0)
                {
                    current.block_type = Block.TYPE.BOX; // 다음 번은 박스를 만든다.
                    current.max_count = 1; // 박스는 1개 만든다.
                    current.height = previous.height; // 높이를 이전과 같게 한다.
                }

                // 일정 확률로 급경사면 등장
                else if (rand % 4 == 0)
                {
                    current.block_type = Block.TYPE.SLOPE; // 다음 번은 경사면을 만든다.
                    current.max_count = 7; // 경사면은 7개 만든다.
                    current.height = previous.height - 3f; // 높이를 이전보다 낮게 한다
                }

                // 이외에는 일반 평지 등장
                else
                {
                    current.max_count = 15; // 평지의 최대개수
                    current.height = previous.height; // 높이를 이전과 같게 한다.
                }

                break;

            case Block.TYPE.OBSTACLE_R: // 이번 블록이 평지 장애물일 경우
            case Block.TYPE.KEY:
            case Block.TYPE.BOX:
                if (rand % 2 == 0)
                {
                    current.block_type = Block.TYPE.FLOOR; // 다음 번은 일반 평지를 만든다.
                    current.max_count = Random.Range(10, 18); // 평지의 최대개수
                }
                else
                {
                    current.block_type = Block.TYPE.SLOPE; // 다음 번은 경사면을 만든다.
                    current.max_count = 7; // 급경사면은 최대 7개 만든다.
                }
                break;
        }
    }

    public void SetLevel2(ref CreationInfo current, CreationInfo previous)
    {
        int rand = Random.Range(1, 101);

        switch (previous.block_type)
        {
            // 이번 블록이 일반 평지일 경우.
            case Block.TYPE.FLOOR: 
            case Block.TYPE.FLOOR1:
            case Block.TYPE.FLOOR2:
            case Block.TYPE.FLOOR3:
            case Block.TYPE.FLOOR4:

                // 일정 확률로 장애물 오브젝트 등장
                if (rand % 2 == 0)
                {
                    if (rand % 3 == 0)
                        current.block_type = Block.TYPE.OBSTACLE_R;     // 바위 생성.
                    else
                        current.block_type = Block.TYPE.OBSTACLE_T;     // 묘비 생성.
                    current.max_count = 1; // 평지 장애물은 1개 만든다.
                    current.height = previous.height; // 높이를 이전과 같게 한다.
                }

                // 일정 확률로 열쇠 등장
                else if (rand % 3 == 0)
                {
                    current.block_type = Block.TYPE.KEY; // 다음 번은 급경사면을 만든다.
                    current.max_count = 1; // 열쇠는 1개 만든다.
                    current.height = previous.height; // 높이를 이전과 같게 한다.
                }

                // 일정 확률로 박스 등장
                else if (rand % 5 == 0)
                {
                    current.block_type = Block.TYPE.BOX; // 다음 번은 박스를 만든다.
                    current.max_count = 1; // 박스는 1개 만든다.
                    current.height = previous.height; // 높이를 이전과 같게 한다.
                }

                // 일정 확률로 급경사면 등장
                else if (rand % 4 == 0)
                {
                    current.block_type = Block.TYPE.SLOPE; // 다음 번은 경사면을 만든다.
                    current.max_count = 14; // 경서면의 최대개수
                    current.height = previous.height - 3f; // 높이를 이전보다 낮게 한다
                }

                // 이외에는 일반 평지 등장
                else
                {
                    current.max_count = 10; // 평지의 최대개수
                    current.height = previous.height; // 높이를 이전과 같게 한다.
                }

                break;

            case Block.TYPE.OBSTACLE_R: // 이번 블록이 평지 장애물일 경우
            case Block.TYPE.OBSTACLE_T: // 이번 블록이 평지 장애물일 경우
            case Block.TYPE.KEY:
            case Block.TYPE.BOX:
                if (rand % 2 == 0)
                {
                    current.block_type = Block.TYPE.FLOOR; // 다음 번은 일반 평지를 만든다.
                    current.max_count = Random.Range(5, 10); // 평지의 최대개수
                }
                else
                {
                    current.block_type = Block.TYPE.SLOPE; // 다음 번은 경사면을 만든다.
                    current.max_count = 10; // 경사면의 최대개수
                }
                break;
        }
    }

    public void SetLevel3(ref CreationInfo current, CreationInfo previous)
    {
        int rand = Random.Range(1, 101);

        switch (previous.block_type)
        {
            case Block.TYPE.FLOOR: // 이번 블록이 일반 평지일 경우.
            case Block.TYPE.FLOOR1: // 이번 블록이 일반 평지일 경우.
            case Block.TYPE.FLOOR2: // 이번 블록이 일반 평지일 경우.
            case Block.TYPE.FLOOR3: // 이번 블록이 일반 평지일 경우.
            case Block.TYPE.FLOOR4: // 이번 블록이 일반 평지일 경우.

                // 일정 확률로 장애물 오브젝트 등장
                if (rand % 2 == 0)
                {
                    if (rand % 3 == 0)
                        current.block_type = Block.TYPE.OBSTACLE_R;     // 바위 생성.
                    else if (rand % 3 == 1)
                        current.block_type = Block.TYPE.OBSTACLE_T;     // 묘비 생성.
                    else
                        current.block_type = Block.TYPE.OBSTACLE_F;     // 펜스 생성.
                    current.max_count = 1; // 평지 장애물은 1개 만든다.
                }

                // 일정 확률로 열쇠 등장
                else if (rand % 3 == 0)
                {
                    current.block_type = Block.TYPE.KEY; // 다음 번은 급경사면을 만든다.
                    current.max_count = 1; // 열쇠는 1개 만든다.
                    current.height = previous.height; // 높이를 이전과 같게 한다.
                }

                // 일정 확률로 박스 등장
                else if (rand % 5 == 0)
                {
                    current.block_type = Block.TYPE.BOX; // 다음 번은 박스를 만든다.
                    current.max_count = 1; // 박스는 1개 만든다.
                    current.height = previous.height; // 높이를 이전과 같게 한다.
                }

                // 일정 확률로 급경사면 등장
                else if (rand % 4 == 0)
                {
                    current.block_type = Block.TYPE.SLOPE; // 다음 번은 경사면을 만든다.
                    current.max_count = 15; // 경사면을 만든다.
                    current.height = previous.height - 3f; // 높이를 이전보다 낮게 한다
                }

                // 이외에는 일반 평지 등장
                else
                {
                    current.max_count = 5; // 일반 평지를 만든다.
                    current.height = previous.height; // 높이를 이전과 같게 한다.
                }

                break;

            case Block.TYPE.OBSTACLE_R: // 이번 블록이 바위 장애물일 경우
            case Block.TYPE.OBSTACLE_T: // 이번 블록이 묘비 장애물일 경우
            case Block.TYPE.OBSTACLE_F: // 이번 블록이 펜스 장애물일 경우
            case Block.TYPE.KEY:
            case Block.TYPE.BOX:
                if (rand % 2 == 0)
                {
                    current.block_type = Block.TYPE.FLOOR; // 다음 번은 일반 평지를 만든다.
                    current.max_count = Random.Range(3, 8); // 평지의 최대개수
                }
                else
                {
                    current.block_type = Block.TYPE.SLOPE; // 다음 번은 경사면을 만든다.
                    current.max_count = Random.Range(10, 20); // 경사면의 최대개수
                }
                break;
        }
    }

    public void SetFloorType(ref CreationInfo current)
    {
        if (typeQueue.Count > 3)
            typeQueue.Dequeue();

        int type = Random.Range(0, 5);
        while (true)
        {
            if ((type == 1 || type == 2) && typeQueue.Contains((Block.TYPE)type))
            {
                type = Random.Range(0, 5);
            }
            else
            {
                typeQueue.Enqueue((Block.TYPE)type);
                break;
            }
        }

        // 다음 번은 일반 평지를 만든다.
        switch (type)
        {
            case 0:
                current.block_type = Block.TYPE.FLOOR;
                break;
            case 1:
                current.block_type = Block.TYPE.FLOOR1;
                break;
            case 2:
                current.block_type = Block.TYPE.FLOOR2;
                break;
            case 3:
                current.block_type = Block.TYPE.FLOOR3;
                break;
            case 4:
                current.block_type = Block.TYPE.FLOOR4;
                break;
            default:
                current.block_type = Block.TYPE.FLOOR;
                break;
        }

        typeQueue.Enqueue(current.block_type);
    }
}