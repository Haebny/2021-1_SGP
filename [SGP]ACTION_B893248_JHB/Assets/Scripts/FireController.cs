using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    public float ACCELERATION = 5.0f; // 가속도.
    public float SPEED_MAX = 7.0f; // 속도의 최댓값.
    public float SPEED_MIN = 7.0f; // 속도의 최댓값.
    public float current_speed = 6.0f; // 현재 속도.
    Rigidbody rb;

    public GameObject Fire;
    public GameObject FireEffect;

    // 산불의 레벨
    public enum LEVEL
    {
        ERROR = -1,
        LEVEL1 = 1,
        LEVEL2,
        LEVEL3
    };
    public LEVEL level;

    private PlayerControl player;
    public Vector3 velocity;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        player = GameObject.FindObjectOfType<PlayerControl>().GetComponent<PlayerControl>();
        level = LEVEL.LEVEL1;
    }

    void Update()
    {
        MovePosition();
        SetPosition();

        if (Input.GetKeyDown(KeyCode.Keypad1))
            level = LEVEL.LEVEL1;
        else if (Input.GetKeyDown(KeyCode.Keypad2))
            level = LEVEL.LEVEL2;
        else if (Input.GetKeyDown(KeyCode.Keypad3))
            level = LEVEL.LEVEL3;

        switch (this.level)
        {
            case LEVEL.LEVEL1:
                SPEED_MAX = 8f;
                SPEED_MIN = 7.5f;
                this.current_speed = 7.5f;
                break;
            case LEVEL.LEVEL2:
                SPEED_MAX = 9f;
                SPEED_MIN = 7.5f;
                this.current_speed = 8f;
                break;
            case LEVEL.LEVEL3:
                SPEED_MAX = 9.5f;
                SPEED_MIN = 8f;
                this.current_speed = 8.5f;
                break;
            case LEVEL.ERROR:
                SPEED_MAX = 0f;
                SPEED_MIN = 0f;
                break;
        }
    }

    private void FixedUpdate()
    {
        velocity = this.GetComponent<Rigidbody>().velocity; // 속도를 설정.
        velocity.x += this.current_speed * Time.fixedDeltaTime * 1.5f;

        // 속도가 최고 속도 제한을 넘으면.
        if (Mathf.Abs(velocity.x) > SPEED_MAX)
        {
            // 최고 속도 제한 이하로 유지한다.
            velocity.x *= this.current_speed / Mathf.Abs(velocity.x);
            if (velocity.x < SPEED_MIN)
                velocity.x = SPEED_MIN;
        }

        this.GetComponent<Rigidbody>().velocity = velocity;
    }

    // 산불을 이동시키는 메소드
    private void SetPosition()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, -transform.up, Color.red, 0.1f);

        // 레이캐스트에 닿은 블록이 있다면 산불을 그 위치로 이동
        if(Physics.Raycast(transform.position, -transform.up, out hit))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                Vector3 movePos = new Vector3(hit.transform.position.x, hit.transform.position.y + 5f, hit.transform.position.z);
                FireEffect.transform.position = movePos;
                Fire.transform.position =  movePos;
            }
        }

        return;
    }

    // 체커를 이동시키는 메소드
    private void MovePosition()
    {
        // 플레이어와의 거리 계산
        float yPos = player.transform.position.y + 50f;
        float xPos;
        int distance = (int)Mathf.Abs(Vector3.Distance(this.transform.position, player.transform.position));

        if (distance > 55 ) // 플레이어가 너무 멀리 떨어져있으면 당겨오기
            xPos = player.transform.position.x - 25f;

        else
            xPos = this.transform.position.x + current_speed * Time.deltaTime;

        Vector3 move = new Vector3(xPos, yPos, 0f);
        this.transform.position = move;
    }
}