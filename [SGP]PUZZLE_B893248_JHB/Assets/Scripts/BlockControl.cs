using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public static float COLLISION_SIZE = 1.0f;      // 블록의 충돌 크기
    public static float VANISH_TIME = 3.0f;         // 불 붙고 사라질 때까지의 시간

    // 그리드에서의 좌표를 나타내는 구조체
    public struct Position
    {
        public int x;           // x좌표
        public int y;           // y좌표
    }

    // 블록의 색상
    public enum COLOR
    {
        None = -1,              // 색 지정 없음
        PINK = 0,               // 분홍색
        BLUE,                   // 파란색
        YELLOW,                 // 노란색
        GREEN,                  // 초록색
        MAGENTA,                // 마젠타
        ORANGE,                 // 주황색
        GRAY,                   // 회색
        NUM,                    // COLOR의 종류
        FIRST = PINK,           // 초기 컬러(PINK)
        LAST = ORANGE,          // 최종 컬러(ORNAGE)
        NORMAL_COLOR_NUM = GRAY // 보통 컬러(회색 이외의 색)의 수
    };

    // 블록의 방향
    public enum DIR4
    {
        NONE = -1,          // 방향 지정 없음
        RIGHT,              // 우
        LEFT,               // 좌
        UP,                 // 상
        DOWN,               // 하
        NUM,                // 방향의 개수
    };

    // 블록이 어떤 상태인지 알려주는 클래스
    public enum STEP
    {
        NONE = -1,          // 상태 정보 없음
        IDLE = 0,           // 대기
        GRABBED,            // 잡힘
        RELEASED,           // 놓아짐
        SLIDE,              // 슬라이드 중
        VACANT,             // 소멸 중
        RESPAWN,            // 재생성 중
        FALL,               // 낙하 중
        LONG_SLIDE,         // 크게 슬라이드 중
        NUM,                // 상태가 몇 종류인지 표시
    }

    public static int BLOCK_NUM_X = 9;      // 블록을 배치할 수 있는 X방향 최대 수
    public static int BLOCK_NUM_Y = 9;      // 블록을 배치할 수 있는 Y방향 최대 수
}

public class BlockControl : MonoBehaviour
{
    public Block.COLOR color = (Block.COLOR)0;          // 블록 색
    public BlockRoot block_root = null;                 // 블록의 신
    public Block.Position pos;                          // 블록 좌표

    public Block.STEP step = Block.STEP.NONE;               // 현재 상태
    public Block.STEP next_step = Block.STEP.NONE;          // 다음 상태
    private Vector3 position_offset_initial = Vector3.zero; // 교체 전 위치
    public Vector3 position_offset = Vector3.zero;          // 교체 후 위치

    public float vanish_timer = -1.0f;              // 블록이 소멸할 때 걸리는 시간
    public Block.DIR4 slide_dir = Block.DIR4.NONE;  // 슬라이드 방향
    public float step_timer = 0.0f;                 // 블록이 교체될 때의 이동시간

    public Material opague_material;        // 불투명 머티리얼
    public Material transparent_material;   // 반투명 머티리얼

    private struct StepFall
    {
        public float velocity;      // 낙하 속도
    }

    private StepFall fall;

    // Start is called before the first frame update
    void Start()
    {
        this.SetColor(this.color);      // 색 입히기
        this.next_step = Block.STEP.IDLE;   // 다음 블록을 대기중으로
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse_position;         // 마우스 위치
        this.block_root.UnprojectMousePosition(out mouse_position, Input.mousePosition);  // 마우스 위치 획득

        // 획득한 마우스 위치를 X와 Y만으로
        Vector2 mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y);

        // 타이머가 0 이상이면
        if (this.vanish_timer >= 0.0f)
        {
            // 타이머 값을 줄임
            this.vanish_timer -= Time.deltaTime;

            // 타이머가 0 미만이면
            if (this.vanish_timer < 0.0f)
            {
                if (this.step != Block.STEP.SLIDE)
                {
                    this.vanish_timer = -1.0f;
                    this.next_step = Block.STEP.VACANT; // 소멸 중
                }
                else
                {
                    this.vanish_timer = 0.0f;
                }
            }
        }

        this.step_timer += Time.deltaTime;
        float slide_time = 0.2f;

        if (this.next_step == Block.STEP.NONE)
        {
            switch (this.step)
            {
                case Block.STEP.SLIDE:
                    if (this.step_timer >= slide_time)
                    {
                        // 슬라이드 중인 블록 소멸 시 VACANT 상태로 이행
                        if (this.vanish_timer == 0.0f)
                        {
                            this.next_step = Block.STEP.VACANT;
                        }
                        else
                        {
                            this.next_step = Block.STEP.IDLE;
                        }
                    }
                    break;
                case Block.STEP.IDLE:
                    this.GetComponent<Renderer>().enabled = true;
                    break;
                case Block.STEP.FALL:
                    if (this.position_offset.y <= 0.0f)
                    {
                        this.next_step = Block.STEP.IDLE;
                        this.position_offset.y = 0.0f;
                    }
                    break;
            }
        }

        // '다음 블록'상태가 '정보 없음' 이외인 동안 = '다음 블록' 상태가 변경될 경우
        while (this.next_step != Block.STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = Block.STEP.NONE;

            switch (this.step)
            {
                case Block.STEP.IDLE:           // 대기
                    this.position_offset = Vector3.zero;
                    // 블록 표시 크기를 보통 크기로
                    this.transform.localScale = Vector3.one * 1.0f;
                    break;
                case Block.STEP.GRABBED:        // 잡힘
                    // 블록 표시 크기를 크게
                    this.transform.localScale = Vector3.one * 1.2f;
                    break;
                case Block.STEP.RELEASED:       // 놓아짐
                    this.position_offset = Vector3.zero;
                    // 블록 표시 크기를 보통 크기로
                    this.transform.localScale = Vector3.one * 1.0f;
                    break;
                case Block.STEP.VACANT:
                    this.position_offset = Vector3.zero;
                    this.SetVisible(false);     // 블록을 표시하지 않음
                    break;
                case Block.STEP.RESPAWN:
                    //색을 랜덤선택하여 블록의 색으로 설정
                    int color_index = Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
                    this.SetColor((Block.COLOR)color_index);
                    this.next_step = Block.STEP.IDLE;
                    break;
                case Block.STEP.FALL:
                    this.SetVisible(true);      // 블록 표시
                    this.fall.velocity = 0.0f;  // 낙하 속도 리셋
                    break;
            }
            this.step_timer = 0.0f;
        }

        switch (this.step)
        {
            case Block.STEP.GRABBED:
                // 항상 슬라이드 방향 확인
                this.slide_dir = this.CalcSlideDir(mouse_position_xy);
                break;
            case Block.STEP.SLIDE:
                // 블록 이동 처리
                float rate = this.step_timer / slide_time;
                rate = Mathf.Min(rate, 1.0f);
                rate = Mathf.Sin(rate * Mathf.PI / 2.0f);
                this.position_offset = Vector3.Lerp(this.position_offset_initial, Vector3.zero, rate);
                break;
            case Block.STEP.FALL:
                // 속도에 중력 부여
                this.fall.velocity += Physics.gravity.y * Time.deltaTime * 0.3f;
                // 수직 방향 위치 계산
                this.position_offset.y += this.fall.velocity * Time.deltaTime;
                // 다 내려왔다면
                if (this.position_offset.y < 0.0f)
                {
                    this.position_offset.y = 0.0f;  // 그 자리에 정지
                }
                break;
        }

        // 그리드 좌표를 실제 좌표(씬 좌표)로 변하고 position_offset 추가
        Vector3 position = BlockRoot.CalcBlockPosition(this.pos) + this.position_offset;
        // 실제 위치를 새로운 위치로 변경
        this.transform.position = position;

        this.SetColor(this.color);
        if (this.vanish_timer >= 0.0f)
        {
            // 현재 레벨의 연소시간으로 설정
            //float vanish_time = this.block_root.level_control.GetVanishTime();

            // 현재 색과 흰색의 중간 색
            Color color0 = Color.Lerp(this.GetComponent<Renderer>().material.color, Color.white, 0.5f);
            // 현재 색과 검은색의 중간 색
            Color color1 = Color.Lerp(this.GetComponent<Renderer>().material.color, Color.black, 0.5f);

            // 불붙는 연출 시간이 절반을 지나면
            if (this.vanish_timer < Block.VANISH_TIME / 2.0f)
            {
                // 투명도 설정
                color0.a = this.vanish_timer / (Block.VANISH_TIME / 2.0f);
                color1.a = color0.a;

                // 반투명 머티리얼을 적용
                this.GetComponent<Renderer>().material = this.transparent_material;
            }

            // vanish_timer가 줄어들수록 1에 가까워짐
            float rate = 1.0f - this.vanish_timer / Block.VANISH_TIME;

            // 서서히 색을 바꿈
            this.GetComponent<Renderer>().material.color = Color.Lerp(color0, color1, rate);
        }

    }

    public void SetColor(Block.COLOR color)
    {
        this.color = color;         // 지정된 색을 멤버 변수에 보관
        Color color_value;         // 색

        switch (this.color)         // 색칠할 색에 따라 분류
        {
            default:
            case Block.COLOR.PINK:
                color_value = new Color(1.0f, 0.5f, 0.5f);
                break;
            case Block.COLOR.BLUE:
                color_value = Color.blue;
                break;
            case Block.COLOR.YELLOW:
                color_value = Color.yellow;
                break;
            case Block.COLOR.GREEN:
                color_value = Color.green;
                break;
            case Block.COLOR.MAGENTA:
                color_value = Color.magenta;
                break;
            case Block.COLOR.ORANGE:
                color_value = new Color(1.0f, 0.46f, 0.0f);
                break;
        }

        this.GetComponent<Renderer>().material.color = color_value;
    }

    // 블록이 잡혔을 때 호출
    public void BeginGrab()
    {
        this.next_step = Block.STEP.GRABBED;
    }

    // 블록이 놓아졌을 때 호출
    public void EndGrab()
    {
        this.next_step = Block.STEP.IDLE;
    }

    // 블록을 잡을 수 있는지 상태 확인 함수
    public bool IsGrabbable()
    {
        bool is_grabbable = false;
        switch (this.step)
        {
            case Block.STEP.IDLE:           // 대기 상태만 잡을 수 있음
                is_grabbable = true;
                break;
        }

        return is_grabbable;
    }

    // 지정된 마우스 좌표가 자신과 겹치는지 반환
    public bool IsContainedPosition(Vector2 position)
    {
        bool result = false;
        Vector3 center = this.transform.position;
        float h = Block.COLLISION_SIZE / 2.0f;

        do
        {
            // X좌표가 자신과 겹치지 않으면 break로 루프 탈출
            if (position.x < center.x - h || center.x + h < position.x)
            {
                break;
            }

            // Y좌표가 자신과 겹치지 않으면 break로 루프 탈출
            if (position.y < center.y - h || center.y + h < position.y)
            {
                break;
            }

            // X좌표, Y좌표 모두 겹쳐있으면 겹쳐있음
            result = true;
        }
        while (false);

        return result;
    }

    public Block.DIR4 CalcSlideDir(Vector2 mouse_position)
    {
        Block.DIR4 dir = Block.DIR4.NONE;

        // 지정된 mouse_position과 현재 위치의 차를 나타내는 벡터
        Vector2 vec = mouse_position - new Vector2(this.transform.position.x, this.transform.position.y);

        //벡터의 크기가 0.1보다 크면 슬라이드로 판정
        if (vec.magnitude > 0.1f)
        {
            if (vec.y > vec.x)
            {
                if (vec.y > -vec.x)
                {
                    dir = Block.DIR4.UP;
                }
                else
                {
                    dir = Block.DIR4.LEFT;
                }
            }
            else
            {
                if (vec.y > -vec.x)
                {
                    dir = Block.DIR4.RIGHT;
                }
                else
                {
                    dir = Block.DIR4.DOWN;
                }
            }
        }

        return dir;
    }

    public float CalcDirOffset(Vector2 position, Block.DIR4 dir)
    {
        float offset = 0.0f;

        // 지정된 위치와 블록의 현재의 위치의 차를 나타내는 벡터
        Vector2 vec = position - new Vector2(this.transform.position.x, this.transform.position.y);

        // 지정 방향에 따라
        switch (dir)
        {
            case Block.DIR4.RIGHT:
                offset = vec.x;
                break;
            case Block.DIR4.LEFT:
                offset = -vec.x;
                break;
            case Block.DIR4.UP:
                offset = vec.y;
                break;
            case Block.DIR4.DOWN:
                offset = -vec.y;
                break;
        }

        return offset;
    }

    public void BeginSlide(Vector3 offset)
    {
        this.position_offset_initial = offset;
        this.position_offset = this.position_offset_initial;

        // 상태를 SLIDE로 변경
        this.next_step = Block.STEP.SLIDE;
    }

    // '사라지는데 걸리는 시간'을 규정값으로 리셋
    public void ToVanishing()
    {
        this.vanish_timer = Block.VANISH_TIME;
        // 현재 레벨의 연소시간으로 설정
        //float vanish_time = this.block_root.level_control.GetVanishTime();
        //this.vanish_timer = vanish_time;
    }

    // vanish_timer가 0보다 크면 true
    public bool IsVanishing()
    {
        bool isVanishing = (this.vanish_timer > 0.0f);
        return isVanishing;
    }

    // '사라지는데 걸리는 시간'을 규정값으로 리셋
    public void RewindVanishTimer()
    {
        this.vanish_timer = Block.VANISH_TIME;

        // 현재 레벨의 연소시간으로 설정
        //float vanish_time = this.block_root.level_control.GetVanishTime();
        //this.vanish_timer = vanish_time;
    }

    // 그리기 가능(renderer.enabled==true)이면 표시
    public bool IsVisible()
    {
        bool isVisible = this.GetComponent<Renderer>().enabled;
        return isVisible;
    }

    // 그리기 가능 설정
    public void SetVisible(bool isVisible)
    {
        this.GetComponent<Renderer>().enabled = isVisible;
    }

    // 대기상태 확인 함수
    public bool IsIdle()
    {
        bool isIdle = false;

        // 현재 블록은 대기중, 다음 블록은 없으면
        if (this.step == Block.STEP.IDLE && this.next_step == Block.STEP.NONE)
        {
            isIdle = true;
        }

        return isIdle;
    }

    // 낙하 시작 처리 함수
    public void BeginFall(BlockControl start)
    {
        this.next_step = Block.STEP.FALL;
        // 지정된 브록에서 좌표 계산
        this.position_offset.y = (float)(start.pos.y - this.pos.y) * Block.COLLISION_SIZE;
    }

    // 색을 바꿔 낙하상태 표현 및 지정 위치 재배치 함수
    public void BeginRespawn(int start_pos_y)
    {
        // 지정 위치까지 y좌표 이동
        this.position_offset.y = (float)(start_pos_y - this.pos.y) * Block.COLLISION_SIZE;
        this.next_step = Block.STEP.FALL;
        int color_index = Random.Range((int)Block.COLOR.FIRST, (int)Block.COLOR.LAST + 1);
        this.SetColor((Block.COLOR)color_index);

        // 현재 레벨의 출현 확률을 바탕으로 블록 색 결정
        //Block.COLOR color = this.block_root.SelectBlockColor();
        //this.SetColor(color);
    }

    // 블록이 비표시로 되어있는지 확인하는 함수
    public bool IsVacant()
    {
        bool is_vacant = false;
        if (this.step == Block.STEP.VACANT && this.next_step == Block.STEP.NONE)
        {
            is_vacant = true;
        }

        return is_vacant;
    }

    // 교체중(슬라이드 중)인지 확인하는 함수
    public bool IsSliding()
    {
        bool is_sliding = (this.position_offset.x != 0.0f);
        return is_sliding;
    }
}
