using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    public static float ACCELERATION = 10.0f; // 가속도.
    public float current_speed = 5.0f; // 현재 속도.
    public PlayerControl player;

    public enum STATE
    {
        ERROR = -1,
        LEVEL1 = 1,
        LEVEL2,
        LEVEL3
    };

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerControl>();
    }

    void Update()
    {
        Vector3 velocity = this.GetComponent<Rigidbody>().velocity; // 속도를 설정.
        this.current_speed = 5.0f;

        velocity.x += FireController.ACCELERATION * Time.deltaTime;

        // 계산으로 구한 속도가 설정해야 할 속도를 넘으면.
        if (Mathf.Abs(velocity.x) > this.current_speed)
        {
            // 넘지 않게 조정한다.
            velocity.x *= this.current_speed / Mathf.Abs(velocity.x);
        }

        CheckDistance();
    }

    // 플레이어와의 거리 계산
    public STATE CheckDistance()
    {
        int distance = (int)Mathf.Abs(Vector3.Distance(this.transform.position, player.transform.position));

        if (distance > 20)
            return STATE.LEVEL1;
        else if (distance > 10)
            return STATE.LEVEL2;
        else
            return STATE.LEVEL3;
    }
}
