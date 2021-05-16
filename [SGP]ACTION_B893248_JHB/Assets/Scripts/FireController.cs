using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    public float ACCELERATION = 10.0f; // 가속도.
    public float SPEED_MAX = 5.0f; // 속도의 최댓값.
    public float current_speed = 0.0f; // 현재 속도.
    Rigidbody rb;

    private bool is_grounded = false;

    // 산불의 레벨
    public enum LEVEL
    {
        ERROR = -1,
        LEVEL1 = 1,
        LEVEL2,
        LEVEL3
    };
    public LEVEL level;

    public PlayerControl player;
    private Vector3 velocity;
    private bool is_locked = true;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        player = GameObject.FindObjectOfType<PlayerControl>().GetComponent<PlayerControl>();
        level = LEVEL.LEVEL1;
    }


    void Update()
    {
        CheckDistance();

        //if (is_locked == false)
        //    CheckPosition();

        velocity = this.GetComponent<Rigidbody>().velocity; // 속도를 설정.
        this.current_speed = 5.0f;

        switch (this.level)
        {
            case LEVEL.LEVEL1:
                SPEED_MAX = 9f;
                break;
            case LEVEL.LEVEL2:
                SPEED_MAX = 10.5f;
                break;
            case LEVEL.LEVEL3:
                SPEED_MAX = 13f;
                break;
            case LEVEL.ERROR:
                SPEED_MAX = 0f;
                break;
        }
        ACCELERATION = SPEED_MAX;

        if (Input.GetKeyDown(KeyCode.Keypad1))
            level = LEVEL.LEVEL1;
        else if (Input.GetKeyDown(KeyCode.Keypad2))
            level = LEVEL.LEVEL2;
        else if (Input.GetKeyDown(KeyCode.Keypad3))
            level = LEVEL.LEVEL3;


        velocity.x += ACCELERATION * Time.deltaTime;

        // 속도가 최고 속도 제한을 넘으면.
        if (Mathf.Abs(velocity.x) > this.current_speed && is_locked)
        {
            // 최고 속도 제한 이하로 유지한다.
            velocity.x *= this.current_speed / Mathf.Abs(velocity.x);
        }

        this.GetComponent<Rigidbody>().velocity = velocity;
    }

    // 플레이어와의 거리 계산
    public LEVEL CheckDistance()
    {
        int distance = (int)Mathf.Abs(Vector3.Distance(this.transform.position, player.transform.position));

        if(distance > 30)
        {
            is_locked = false;
        }

        if (distance > 20)
            return LEVEL.LEVEL1;
        else if (distance > 10)
            return LEVEL.LEVEL2;
        else
            return LEVEL.LEVEL3;
    }

    //public void ChangeLevel()
    //{
    //        switch (this.level)
    //        {
    //            case LEVEL.LEVEL1:
    //                // 계산으로 구한 속도가 설정해야 할 속도를 넘으면.
    //                if (Mathf.Abs(velocity.x) > this.current_speed)
    //                {
    //                    // 넘지 않게 조정한다.
    //                    velocity.x *= this.current_speed / Mathf.Abs(velocity.x);
    //                }

    //                break;
    //            case LEVEL.LEVEL2: // 점프 중일 때.
    //                if (timer > 10)
    //                {
    //                    // 속도를 높인다.
    //                    velocity.x += PlayerControl.ACCELERATION * Time.deltaTime;
    //                }
    //                break;

    //            case LEVEL.LEVEL3:
    //                break;
    //        }
    //        // Rigidbody의 속도를 위에서 구한 속도로 갱신.
    //        // (이 행은 상태에 관계없이 매번 실행된다).
    //        this.GetComponent<Rigidbody>().velocity = velocity;
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Obstacle"))
        {
            Destroy(collision.gameObject);
            this.GetComponent<Rigidbody>().velocity = velocity;
        }

        if(collision.gameObject.CompareTag("Player"))
        {
            this.velocity = Vector3.zero;
            level = LEVEL.ERROR;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            is_grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            is_grounded = false;
        }
    }

    private void CheckPosition()
    {
        if(player.transform.position.y > this.transform.position.y || player.transform.position.x < this.transform.position.x)
        {
            Debug.Log("SETTING");
            this.transform.position.Set(player.transform.position.x - 20f, player.transform.position.y + 10f, 0);
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            is_locked = true;
        }
    }
}