using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // 점프에 필요한 전역변수 선언 먼저.
    public static float ACCELERATION = 10.0f; // 가속도.
    public static float SPEED_MIN = 4.0f; // 속도의 최솟값.
    public static float SPEED_MAX = 8.0f; // 속도의 최댓값.
    public static float JUMP_HEIGHT_MAX = 3.0f; // 점프 높이.
    public static float JUMP_KEY_RELEASE_REDUCE = 0.5f; // 점프 후의 감속도.
    public static float NARAKU_HEIGHT = -5.0f;  // 나락 높이

    public enum STEP
    { // Player의 각종 상태를 나타내는 자료형 (*열거체)
        NONE = -1, // 상태정보 없음.
        RUN = 0, // 달린다.
        JUMP, // 점프.
        MISS, // 실패.
        NUM, // 상태가 몇 종류 있는지 보여준다(=3).
    };

    public STEP step = STEP.NONE; // Player의 현재 상태.
    public STEP next_step = STEP.NONE; // Player의 다음 상태.
    public float step_timer = 0.0f; // 경과 시간.
    [SerializeField]private bool is_landed = false; // 착지했는가.
    [SerializeField] private bool is_colided = false; // 뭔가와 충돌했는가.
    [SerializeField]private bool is_key_released = false; // 버튼이 떨어졌는가.

    public float current_speed = 0.0f; // 현재 속도.
    public LevelControl level_control = null; // LevelControl이 저장됨.

    private float click_timer = 1.0f; // 버튼이 눌린 후의 시간
    private float CLICK_GRACE_TIME = 0.5f; // 점프하고 싶은 의사를 받아들일 시간

    void Start()
    {
        this.next_step = STEP.RUN;
    }

    void Update()
    {
        Vector3 velocity = this.GetComponent<Rigidbody>().velocity; // 속도를 설정.
        this.current_speed = this.level_control.GetPlayerSpeed();
        this.CheckLanded(); // 착지 상태인지 체크.

        switch (this.step)
        {
            case STEP.RUN:
            case STEP.JUMP:
                // 현재 위치가 한계치보다 아래면.
                if (this.transform.position.y < NARAKU_HEIGHT)
                {
                    this.next_step = STEP.MISS; // '실패' 상태로 한다.
                }
                break;
        }

        this.step_timer += Time.deltaTime; // 경과 시간을 진행한다.

        if (Input.GetMouseButtonDown(0))
        {
            // 버튼이 눌렸으면.
            this.click_timer = 0.0f; // 타이머를 리셋.
        }
        else
        {
            if (this.click_timer >= 0.0f)
            {
                // 그렇지 않으면.
                this.click_timer += Time.deltaTime; // 경과 시간을 더한다.
            }
        }

        // 다음 상태가 정해져 있지 않으면 상태의 변화를 조사한다.
        if (this.next_step == STEP.NONE)
        {
            switch (this.step)
            {
                // Player의 현재 상태로 분기.
                case STEP.RUN: // 달리는 중일 때.
                    //if (!this.is_landed)
                    //{
                    //    // 달리는 중이고 착지하지 않은 경우 아무것도 하지 않는다.
                    //}
                    //else
                    //{
                    //    if (Input.GetMouseButtonDown(0))
                    //    {
                    //        // 달리는 중이고 착지했고 왼쪽 버튼이 눌렸다면.
                    //        // 다음 상태를 점프로 변경.
                    //        this.next_step = STEP.JUMP;
                    //    }
                    //}
                    if (0.0f <= this.click_timer && this.click_timer <= CLICK_GRACE_TIME)
                    {
                        if (this.is_landed)
                        {
                            // 착지했다면.
                            this.click_timer = -1.0f; // 버튼이 눌려있지 않음을 나타내는 -1.0f로.
                            this.next_step = STEP.JUMP; // 점프 상태로 한다.
                        }
                    }
                    break;
                case STEP.JUMP: // 점프 중일 때.
                    if (this.is_landed)
                    {
                        // 점프 중이고 착지했다면 다음 상태를 주행 중으로 변경.
                        this.next_step = STEP.RUN;
                    }
                    break;
            }
        }

        // '다음 정보'가 '상태 정보 없음'이 아닌 동안(상태가 변할 때만).
        while (this.next_step != STEP.NONE)
        {
            this.step = this.next_step; // '현재 상태'를 '다음 상태'로 갱신.
            this.next_step = STEP.NONE; // '다음 상태'를 '상태 없음'으로 변경.
            switch (this.step)
            { 
                // 갱신된 '현재 상태'가.
                case STEP.JUMP: // '점프'일 때.
                    // 최고 도달점 높이(JUMP_HEIGHT_MAX)까지 점프할 수 있는 속도를 계산.
                    velocity.y = Mathf.Sqrt(2.0f * 9.8f * PlayerControl.JUMP_HEIGHT_MAX);
                    // '버튼이 떨어졌음을 나타내는 플래그'를 클리어한다.
                    this.is_key_released = false;
                    break;
            }

            // 상태가 변했으므로 경과 시간을 제로로 리셋.
            this.step_timer = 0.0f;
        }

        // 상태별로 매 프레임 갱신 처리.
        switch (this.step)
        {
            case STEP.RUN: // 달리는 중일 때.
                // 속도를 높인다.
                velocity.x += PlayerControl.ACCELERATION * Time.deltaTime;

                // 계산으로 구한 속도가 설정해야 할 속도를 넘으면.
                if (Mathf.Abs(velocity.x) > this.current_speed)
                {
                    // 넘지 않게 조정한다.
                    velocity.x *= this.current_speed / Mathf.Abs(velocity.x);
                }

                break;
            case STEP.JUMP: // 점프 중일 때.
                do
                {
                    // '버튼이 떨어진 순간'이 아니면.
                    if (!Input.GetMouseButtonUp(0))
                    {
                        break; // 아무것도 하지 않고 루프를 빠져나간다.
                    }
                    // 이미 감속된 상태면(두 번이상 감속하지 않도록).
                    if (this.is_key_released)
                    {
                        break; // 아무것도 하지 않고 루프를 빠져나간다.
                    }
                    // 상하방향 속도가 0 이하면(하강 중이라면).
                    if (velocity.y <= 0.0f)
                    {
                        break; // 아무것도 하지 않고 루프를 빠져나간다.
                    }
                    // 버튼이 떨어져 있고 상승 중이라면 감속 시작.
                    // 점프의 상승은 여기서 끝.
                    velocity.y *= JUMP_KEY_RELEASE_REDUCE;
                    this.is_key_released = true;
                } while (false);
                break;

            case STEP.MISS:
                // 가속도(ACCELERATION)를 빼서 Player의 속도를 느리게 해 간다.
                velocity.x -= PlayerControl.ACCELERATION * Time.deltaTime;
                if (velocity.x < 0.0f)
                { // Player의 속도가 마이너스면.
                    velocity.x = 0.0f; // 0으로 한다.
                }
                break;
        }
        // Rigidbody의 속도를 위에서 구한 속도로 갱신.
        // (이 행은 상태에 관계없이 매번 실행된다).
        this.GetComponent<Rigidbody>().velocity = velocity;
    }

    private void CheckLanded() // 착지했는지 조사
    {
        this.is_landed = false; // 일단 false로 설정.

        do
        {
            Vector3 s = this.transform.position; // Player의 현재 위치.
            Vector3 e = s + Vector3.down * 1.0f; // s부터 아래로 1.0f로 이동한 위치.
            RaycastHit hit;

            // s부터 e 사이에 아무것도 없을 때. *out: method 내에서 생선된 값을 반환때 사용.
            if(!Physics.Linecast(s, e, out hit)) {
                break; // 아무것도 하지 않고 do~while 루프를 빠져나감(탈출구로).
            }

            // s부터 e 사이에 뭔가 있을 때 아래의 처리가 실행.
            if(this.step == STEP.JUMP) { // 현재, 점프 상태라면.
                if(this.step_timer < Time.deltaTime * 3.0f) { // 경과 시간이 3.0f 미만이라면.
                    break; // 아무것도 하지 않고 do~while 루프를 빠져나감(탈출구로).
                }
            }

            // s부터 e 사이에 뭔가 있고 JUMP 직후가 아닐 때만 아래가 실행.
            this.is_landed = true;

        } while (false);
        // 루프의 탈출구.
    }

    public bool IsPlayEnd() // 게임이 끝났는지 판정.
    {
        bool ret = false;
        switch (this.step)
        {
            case STEP.MISS: // MISS 상태라면.
                ret = true; // '죽었어요'(true)라고 알려줌.
                break;
        }
        return (ret);
    }
}
