using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private GameObject player = null;
    private PlayerControl player_control;
    private Vector3 position_offset = Vector3.zero;

    void Start()
    {
        // 멤버 변수 player에 Player 오브젝트를 할당.
        this.player = GameObject.FindGameObjectWithTag("Player");
        // 카메라 위치(this.transform.position)와 플레이어 위치(this.player.transform.position)의 차이.
        this.position_offset = this.transform.position - this.player.transform.position + new Vector3(5, 1f, -15);

        // 멤버 변수 player_control에 PlayerControl 컴포넌트 할당;
        this.player_control = player.GetComponent<PlayerControl>();
    }

    void LateUpdate()
    { 
        // 모든 게임 오브젝트의 Update() 메서드 처리 후에 자동으로 호출.
        // 카메라 현재 위치를 new_position에 할당.
        Vector3 new_position = this.transform.position;
      
        // 카메라가 플레이어를 따라갈 수 있도록 함
        new_position.x = this.player.transform.position.x + this.position_offset.x;
        new_position.y = this.player.transform.position.y + this.position_offset.y;

        //// 플레어가 점프 상태이면 시야 확보를 위해 카메라를 멀리 떨어뜨림
        //if (player_control.Is_landed == false)
        //{
        //    Debug.Log("JUMP");
        //    new_position.z = Mathf.Lerp(transform.position.z, new_position.z - player_control.Distance * 2.0f, Time.deltaTime);
        //}
        //else
        //{
        //    Debug.Log("RUN");
        //    new_position.z = -10f;
        //}

        // 카메라 위치를 새로운 위치(new_position)로 갱신.
        this.transform.position = new_position;
    }
}
