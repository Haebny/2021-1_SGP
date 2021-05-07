using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRoot : MonoBehaviour
{
    public GameObject BlockPrefab = null;           // 만들어낼 블록의 프리팹
    public BlockControl[,] blocks;                  // 그리드

    private GameObject main_camera = null;          // 메인 카메라
    private BlockControl grabbed_block = null;      // 잡은 블록

    private ScoreCounter score_counter = null;      // 점수 카운터 
    protected bool is_vanishing_prev = false;       // 앞에서 발화했는지

    public TextAsset levelData = null;              // 레벨 데이터의 텍스트 저장
    public LevelControl level_control;              // LevelControl 저장

    // Start is called before the first frame update
    void Start()
    {
        // 카메라로부터 마우스 커서를 통과하는 광선을 쏘기 위해 메인카메라 필요
        this.main_camera = GameObject.FindGameObjectWithTag("MainCamera");
        this.score_counter = this.gameObject.GetComponent<ScoreCounter>();
    }

    // Update is called once per frame
    void Update()
    {
        /// 마우스 좌표와 겹치는지 확인
        Vector3 mouse_position;     // 마우스 위치
        this.UnprojectMousePosition(out mouse_position, Input.mousePosition);   // 마우스 위치를 가져옴
        // 가져온 마우스 위치를 하나의 Vector2로 모음
        Vector2 mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y);

        /// 잡을 수 있는 상태의 블록을 잡음
        // 잡은 블록이 비어있는 경우
        if (this.grabbed_block == null)
        {
            if (!this.IsAnyFallingBlock())
            {
                // 마우스 버튼을 누름
                if (Input.GetMouseButtonDown(0))
                {
                    foreach (BlockControl block in this.blocks)
                    {
                        // 블록을 잡을 수 없는 경우
                        if (!block.IsGrabbable())
                        {
                            continue;   // 루프의 처음으로 점프
                        }

                        // 마우스 위치가 블록 영역 밖이면
                        if (!block.IsContainedPosition(mouse_position_xy))
                        {
                            continue;   // 루프의 처음으로 점프
                        }

                        // 처리 중인 블록을 grabbed_block에 등록
                        this.grabbed_block = block;

                        // 잡았을 때의 처리를 실행
                        this.grabbed_block.BeginGrab();
                        break;
                    }
                }
            }
        }
        // 블록을 잡았을 때
        else
        {
            do
            {
                // 슬라이드할 곳의 블록을 가져옴
                BlockControl swap_target = this.GetNextBlock(grabbed_block, grabbed_block.slide_dir);

                // 슬라이드할 곳의 블록이 비어있으면
                if (swap_target == null)
                {
                    break;  // 루프 탈출
                }
                // 슬라이드할 곳의 블록이 잡을 수 없는 상태면
                if (!swap_target.IsGrabbable())
                {
                    break;  // 루프 탈출
                }

                // 현재 위치에서 슬라이드 위치까지의 거리를 얻음
                float offset = this.grabbed_block.CalcDirOffset(mouse_position_xy, this.grabbed_block.slide_dir);

                // 수리 거리가 블록 크기의 절반보다 작다면
                if (offset < Block.COLLISION_SIZE / 2.0f)
                {
                    break;  // 루프 탈출
                }

                this.SwapBlock(grabbed_block, grabbed_block.slide_dir, swap_target);
                this.grabbed_block = null;  // 블록을 놓고 있음
            } while (false);

            // 마우스 버튼이 눌려 있지 않으면
            if (!Input.GetMouseButton(0))
            {
                //블록을 놓았을 때의 처리를 실행
                this.grabbed_block.EndGrab();

                // grabbed_block을 비움
                this.grabbed_block = null;
            }

            // 낙하 중 또는 슬라이드 중이면
            if (this.IsAnyFallingBlock() || this.IsAnySlidingBlock())
            {
                // 아무것도 하지 않음
            }
            else
            {
                int ignite_count = 0;   // 불이 붙은 개수

                // 그래드 내 모든 블록에 대해 처리
                foreach (BlockControl block in this.blocks)
                {
                    // 대기중이면
                    if (!block.IsIdle())
                    {
                        continue;   // 점프 후 다음 블록 처리
                    }

                    // 세로 또는 가로에 같은 색 블록이 세 개 이상 나열되어있다면
                    if (this.CheckConnection(block))
                    {
                        ignite_count++; // 불이 붙은 개수 증가
                    }
                }


                // 불이 붙은 개수가 0보다 크면(한 군데라도 맞춰진 곳이 있으면)
                if (ignite_count > 0)
                {
                    // 연속 점화가 아니라면, 점화 횟수 리셋
                    if (!this.is_vanishing_prev)
                    {
                        this.score_counter.ClearIgniteCount();
                    }

                    // 점화 횟수 증가
                    this.score_counter.AddIgniteCount(ignite_count);
                    // 합계 점수 갱신
                    this.score_counter.UpdateTotalScore();

                    // 한 군데라도 맞춰진 곳이 있으면 실행
                    int block_count = 0;    // 불이 붙는 중인 블록의 개수

                    // 그리드 내의 모든 블록에 대해 처리
                    foreach (BlockControl block in this.blocks)
                    {
                        // 타는 중이면
                        if (block.IsVanishing())
                        {
                            block.RewindVanishTimer();  // 재점화
                            block_count++;
                        }
                    }
                }
            }

        }

        // 하나라도 연소 중인 블록이 있는지 확인
        bool is_vanishing = this.IsAnyVanishingBlock();

        // 조건이 만족되면 블록을 떨어뜨림
        do
        {
            // 연소 중인 블록이 있다면
            if (is_vanishing)
            {
                break;  // 낙하하지 않음
            }
            // 교체 중인 블록이 있다면
            if (this.IsAnySlidingBlock())
            {
                break;  // 낙하하지 않음
            }

            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                // 열에 교체 중인 블록이 있다면 그 열은 처리하지 않고 다음 열로 진행
                if (this.IsSlidingBlockInColumn(x))
                {
                    continue;
                }

                // 그 열에 있느 블록을 위에서부터 검사
                for (int y = 0; y < Block.BLOCK_NUM_Y - 1; y++)
                {
                    // 지정 블록이 비표시라면 다음 블록으로
                    if (!this.blocks[x, y].IsVacant())
                    {
                        continue;
                    }

                    // 지정 블록 아래에 있는 블록을 검사
                    for (int y1 = y + 1; y1 < Block.BLOCK_NUM_Y; y1++)
                    {
                        // 아래에 있는 블록이 비표시면 다음 블록으로 넘어감
                        if (this.blocks[x, y1].IsVacant())
                        {
                            continue;
                        }

                        // 블록 교체
                        this.FallBlock(this.blocks[x, y], Block.DIR4.UP, this.blocks[x, y1]);
                        break;
                    }
                }
            }

            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                int fall_start_y = Block.BLOCK_NUM_Y;
                for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
                {
                    // 비표시 블록이 아니라면 다음 블록으로
                    if (!this.blocks[x, y].IsVacant())
                    {
                        continue;
                    }

                    this.blocks[x, y].BeginRespawn(fall_start_y);   // 블록 재생성
                    fall_start_y++;
                }
            }

        } while (false);

        this.is_vanishing_prev = is_vanishing;
    }

    public void InitialSetUp()
    {
        // 그리드의 크기: 9x9
        this.blocks = new BlockControl[Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y];
        // 블록의 색 번호
        int color_index = 0;
        Block.COLOR color = Block.COLOR.FIRST;

        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)         // 처음부터 마지막 행까지
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)    // 왼쪽부터 오른쪽까지
            {
                // BlockPrefab의 인스턴스를 씬에 생성
                GameObject game_object = Instantiate(this.BlockPrefab) as GameObject;
                // 생성한 블록의 BlockControl 클래스 가져오기
                BlockControl block = game_object.GetComponent<BlockControl>();
                // 블록을 그리드에 저장
                this.blocks[x, y] = block;

                // 블록의 위치 정보(그리드 좌표) 설정
                block.pos.x = x;
                block.pos.y = y;
                // 각 BlockControl이 연계할 GameRoot는 자신
                block.block_root = this;

                // 그리드 좌표를 실제 위치(씬 좌표)로 변환
                Vector3 position = BlockRoot.CalcBlockPosition(block.pos);
                // 씬의 블록 위치 이동
                block.transform.position = position;
                // 블록의 색 변경
                block.SetColor((Block.COLOR)color_index);
                //// 현재 출현 확률을 바탕으로 색 결정
                //color = this.SelectBlockColor();
                //block.SetColor(color);
                // 블록의 이름을 설정 (추후 블록 정보 확인에 필요)
                block.name = "block(" + block.pos.x.ToString() + "," + block.pos.y.ToString() + ")";
                // 전체 색 중 하나의 색을 임의로 선택
                color_index = Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
            }
        }
    }

    // 지정된 그리드 좌표로 씬에서의 좌표 계산
    public static Vector3 CalcBlockPosition(Block.Position pos)
    {
        // 배치할 왼쪽 위 구석 위치를 초기값으로 설정
        Vector3 position = new Vector3(-(Block.BLOCK_NUM_X / 2.0f - 0.5f), -(Block.BLOCK_NUM_Y / 2.0f - 0.5f), 0.0f);

        // 초깃값 + 그리드 좌표 x 블록 크기
        position.x += (float)pos.x * Block.COLLISION_SIZE;
        position.y += (float)pos.y * Block.COLLISION_SIZE;

        return position;          //씬에서의 좌표 반환
    }

    public bool UnprojectMousePosition(out Vector3 world_position, Vector3 mouse_position)
    {
        bool result;

        // 카메라에 대해 뒤를 향하는 판 작성(Vecotr3.back)
        // 블록의 절반 크기만큼 앞에 둠
        Plane plane = new Plane(Vector3.back, new Vector3(0.0f, 0.0f, -Block.COLLISION_SIZE / 2.0f));

        // 카메라와 마우스 통과하는 레이 생성
        Ray ray = this.main_camera.GetComponent<Camera>().ScreenPointToRay(mouse_position);

        float depth;

        // 광선(ray)이 판(plane)에 닿았다면
        if (plane.Raycast(ray, out depth))       // depth에 정보 기록
        {
            // 인수 world_position을 마우스 위치로 덮어씌움
            world_position = ray.origin + ray.direction * depth;
            result = true;
        }
        // 닿지 않았다면
        else
        {
            // 인수 world_position을 0인 벡터로 덮어씌움
            world_position = Vector3.zero;
            result = false;
        }

        return result;
    }

    public BlockControl GetNextBlock(BlockControl block, Block.DIR4 dir)
    {
        // 슬라이드할 곳의 블록을 여기에 저장
        BlockControl next_block = null;

        switch (dir)
        {
            case Block.DIR4.RIGHT:
                // 그래드 내 범위
                if (block.pos.x < Block.BLOCK_NUM_X - 1)
                {
                    next_block = this.blocks[block.pos.x + 1, block.pos.y];
                }
                break;
            case Block.DIR4.LEFT:
                // 그래드 내 범위
                if (block.pos.x > 0)
                {
                    next_block = this.blocks[block.pos.x - 1, block.pos.y];
                }
                break;
            case Block.DIR4.UP:
                // 그래드 내 범위
                if (block.pos.x > 0)
                {
                    next_block = this.blocks[block.pos.x, block.pos.y + 1];
                }
                break;
            case Block.DIR4.DOWN:
                // 그래드 내 범위
                if (block.pos.x > 0)
                {
                    next_block = this.blocks[block.pos.x, block.pos.y - 1];
                }
                break;
        }

        return next_block;
    }

    public static Vector3 GetDirVector(Block.DIR4 dir)
    {
        Vector3 vec = Vector3.zero;

        switch (dir)
        {
            case Block.DIR4.RIGHT:
                vec = Vector3.right;    // 오른쪽 1칸 이동
                break;
            case Block.DIR4.LEFT:
                vec = Vector3.left;     // 왼쪽 1칸 이동
                break;
            case Block.DIR4.UP:
                vec = Vector3.up;       // 위쪽 1칸 이동
                break;
            case Block.DIR4.DOWN:
                vec = Vector3.down;     // 아래쪽 1칸 이동
                break;
            case Block.DIR4.NUM:
                break;
        }

        vec *= Block.COLLISION_SIZE;    // 블록의 크기와 곱함

        return vec;
    }

    public static Block.DIR4 GetOppositDir(Block.DIR4 dir)
    {
        Block.DIR4 opposit = dir;

        switch (dir)
        {
            case Block.DIR4.RIGHT:
                opposit = Block.DIR4.LEFT;
                break;
            case Block.DIR4.LEFT:
                opposit = Block.DIR4.RIGHT;
                break;
            case Block.DIR4.UP:
                opposit = Block.DIR4.DOWN;
                break;
            case Block.DIR4.DOWN:
                opposit = Block.DIR4.UP;
                break;
        }

        return opposit;
    }

    // 블록을 교체하는 함수
    public void SwapBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
    {
        // 블록의 색을 각각 기억
        Block.COLOR color0 = block0.color;
        Block.COLOR color1 = block1.color;

        // 블록의 확대율을 각각 기억
        Vector3 scale0 = block0.transform.localScale;
        Vector3 scale1 = block1.transform.localScale;

        // 블록의 사라지는 시간을 각각 기억
        float vanish_timer0 = block0.vanish_timer;
        float vanish_timer1 = block1.vanish_timer;

        // 블록의 이동할 위치를 각각 구함
        Vector3 offset0 = BlockRoot.GetDirVector(dir);
        Vector3 offset1 = BlockRoot.GetDirVector(BlockRoot.GetOppositDir(dir));

        // 색 교체
        block0.SetColor(color1);
        block1.SetColor(color0);

        // 확대율 교체
        block0.transform.localScale = scale1;
        block1.transform.localScale = scale0;

        // 사라지는 시간을 교체
        block0.vanish_timer = vanish_timer1;
        block1.vanish_timer = vanish_timer0;
        block0.BeginSlide(offset0); // 원래 블록 이동 시작
        block1.BeginSlide(offset1); // 이동할 위치의 블록 이동 시작
    }

    // 인수로 받은 블록이 세 개의 블록 안에 들어가는지 파악하느 메서드
    public bool CheckConnection(BlockControl start)
    {
        bool result = false;
        int normal_block_num = 0;

        // 인수인 블록이 불붙은 다음이 아니면
        if (!start.IsVanishing())
        {
            normal_block_num = 1;
        }

        // 그리드 좌표를 기억
        int rx = start.pos.x;
        int lx = start.pos.x;


        // 블록의 왼 쪽 검사
        for (int x = lx - 1; x > 0; x--)
        {
            BlockControl next_block = this.blocks[x, start.pos.y];

            // 색이 다르면
            if (next_block.color != start.color)
            {
                break;  // 루프를 빠져나감
            }

            // 낙하 중이면
            if (next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL)
            {
                break;  // 루프를 빠져나감
            }

            // 슬라이드 중이면
            if (next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE)
            {
                break;  // 루프를 빠져나감
            }

            // 불붙은 상태가 아니면
            if (!next_block.IsVanishing())
            {
                normal_block_num++; // 검사용 카운터를 증가
            }

            lx = x;
        }

        // 블록의 오른쪽을 검사
        for (int x = rx + 1; x < Block.BLOCK_NUM_X; x++)
        {
            BlockControl next_block = this.blocks[x, start.pos.y];

            // 색이 다르면
            if (next_block.color != start.color)
            {
                break;
            }

            // 낙하 중이면
            if (next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL)
            {
                break;  // 루프를 빠져나감
            }

            // 슬라이드 중이면
            if (next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE)
            {
                break;  // 루프를 빠져나감
            }

            // 불붙은 상태가 아니면
            if (!next_block.IsVanishing())
            {
                normal_block_num++; // 검사용 카운터를 증가
            }

            rx = x;
        }

        do
        {
            // 오른쪽 블록의 그리드 번호 -
            // 왼쪽 블록의 그리드 번호 +
            // 중앙 블록(1)을 더한 수가 3 미만이면
            if (rx - lx + 1 < 3)
            {
                break;
            }

            // 불붙지 않은 블록이 하나도 없으면
            if (normal_block_num == 0)
            {
                break;  // 루프를 빠져나감
            }

            for (int x = lx; x < rx + 1; x++)
            {
                this.blocks[x, start.pos.y].ToVanishing();
                result = true;
            }
        } while (false);

        normal_block_num = 0;
        if (!start.IsVanishing())
        {
            normal_block_num = 1;
        }

        int uy = start.pos.y;
        int dy = start.pos.y;

        // 블록 위쪽을 검사
        for (int y = dy - 1; y > 0; y--)
        {
            BlockControl next_block = this.blocks[start.pos.x, y];

            // 색이 다르면
            if (next_block.color != start.color)
            {
                break;
            }

            // 낙하 중이면
            if (next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL)
            {
                break;  // 루프를 빠져나감
            }

            // 슬라이드 중이면
            if (next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE)
            {
                break;  // 루프를 빠져나감
            }

            // 불붙은 상태가 아니면
            if (!next_block.IsVanishing())
            {
                normal_block_num++; // 검사용 카운터를 증가
            }

            dy = y;
        }

        // 블록의 아래를 검사
        for (int y = uy + 1; y < Block.BLOCK_NUM_Y; y++)
        {
            BlockControl next_block = this.blocks[start.pos.x, y];

            // 색이 다르면
            if (next_block.color != start.color)
            {
                break;
            }

            // 낙하 중이면
            if (next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL)
            {
                break;  // 루프를 빠져나감
            }

            // 슬라이드 중이면
            if (next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE)
            {
                break;  // 루프를 빠져나감
            }

            // 불붙은 상태가 아니면
            if (!next_block.IsVanishing())
            {
                normal_block_num++; // 검사용 카운터를 증가
            }

            uy = y;
        }

        do
        {
            if (uy - dy + 1 < 3)
            {
                break;
            }

            if (normal_block_num == 0)
            {
                break;
            }

            for (int y = dy; y < uy + 1; y++)
            {
                this.blocks[start.pos.x, y].ToVanishing();
                result = true;
            }
        } while (false);

        return result;
    }

    // 불붙는 중인 블록이 하나라도 있으면 true를 반환
    private bool IsAnyVanishingBlock()
    {
        bool result = false;
        foreach (BlockControl block in this.blocks)
        {
            if (block.vanish_timer > 0.0f)
            {
                result = true;
                break;
            }
        }

        return result;
    }

    // 슬라이드 중인 블록이 하나라도 있으면 true를 반환
    private bool IsAnySlidingBlock()
    {
        bool result = false;
        foreach (BlockControl block in this.blocks)
        {
            if (block.step == Block.STEP.SLIDE)
            {
                result = true;
                break;
            }
        }

        return result;
    }

    // 낙하 중인 블록이 하나라도 있으면 true를 반환
    private bool IsAnyFallingBlock()
    {
        bool result = false;
        foreach (BlockControl block in this.blocks)
        {
            if (block.step == Block.STEP.FALL)
            {
                result = true;
                break;
            }
        }

        return result;
    }

    // 낙하 시 위아래 블록 교체
    public void FallBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
    {
        // block0와 block1의 색, 크기, 사라질 때까지 걸리는 시간, 표시, 비표시, 상태 기록
        Block.COLOR color0 = block0.color;
        Block.COLOR color1 = block1.color;
        Vector3 scale0 = block0.transform.localScale;
        Vector3 scale1 = block1.transform.localScale;
        float vanish_timer0 = block0.vanish_timer;
        float vanish_timer1 = block1.vanish_timer;
        bool visible0 = block0.IsVisible();
        bool visible1 = block1.IsVisible();
        Block.STEP step0 = block0.step;
        Block.STEP step1 = block1.step;

        // block0와 block1의 각종 속성 교체
        block0.SetColor(color1);
        block1.SetColor(color0);
        block0.transform.localScale = scale1;
        block1.transform.localScale = scale0;
        block0.vanish_timer = vanish_timer1;
        block1.vanish_timer = vanish_timer0;
        block0.SetVisible(visible1);
        block1.SetVisible(visible0);
        block0.step = step1;
        block1.step = step0;
        block0.BeginFall(block1);
    }

    // 지정 그리드 좌표의 열에 슬라이드 중인 블록이 있는지 확인하는 함수
    private bool IsSlidingBlockInColumn(int x)
    {
        bool result = false;
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            // 슬라이드 중인 블록이 있으면
            if (this.blocks[x, y].IsSliding())
            {
                result = true;
                break;
            }
        }

        return result;
    }

    //public void Create()
    //{
    //    this.level_control = new LevelControl();
    //    this.level_control.Initialize();                    // 레벨 데이터 초기화
    //    this.level_control.LoadLevelData(this.levelData);   // 데이터 읽기
    //    this.level_control.SelectLevel();                   // 레벨 선택
    //}

    //public Block.COLOR SelectBlockColor()
    //{
    //    Block.COLOR color = Block.COLOR.FIRST;
    //    // 이번 레벨의 레벨 데이터를 가져옴
    //    //LevelData level_data = this.level_control.GetCurrentLevelData();
    //    float rand = Random.Range(0.0f, 1.0f);  // 0.0~1.0 사이의 난수
    //    float sum = 0.0f;
    //    int i = 0;

    //    // 블록의 종류 전체를 처리하는 루프
    //    for (i = 0; i < level_data.probability.Length - 1; i++)
    //    {
    //        if (level_data.probability[i] == 0.0f)
    //        {
    //            continue;   // 출현 확률이 0이면 루프의 처음으로 점프
    //        }
    //        sum += level_data.probability[i];   // 출현 확률을 더함

    //        // 합계가 난숫값을 웃돌면
    //        if (rand < sum)
    //        {
    //            break;  // 루프를 빠져나옴
    //        }
    //    }

    //    color = (Block.COLOR)i;     // i번째 색 반환
    //    return color;
    //}
}