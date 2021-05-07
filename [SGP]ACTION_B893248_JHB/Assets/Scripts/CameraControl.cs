using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private GameObject player = null;
    private Vector3 position_offset = Vector3.zero;

    void Start()
    {
        // 멤버 변수 player에 Player 오브젝트를 할당.
        this.player = GameObject.FindGameObjectWithTag("Player");
        // 카메라 위치(this.transform.position)와 플레이어 위치(this.player.transform.position)의 차이.
        this.position_offset = this.transform.position - this.player.transform.position;
    }

    void LateUpdate()
    { 
        // 모든 게임 오브젝트의 Update() 메서드 처리 후에 자동으로 호출.
        // 카메라 현재 위치를 new_position에 할당.
        Vector3 new_position = this.transform.position;
      
        // 카메라가 플레이어를 따라갈 수 있도록 함
        new_position.x = this.player.transform.position.x + this.position_offset.x;
        new_position.y = this.player.transform.position.y + this.position_offset.y;

        // 카메라 위치를 새로운 위치(new_position)로 갱신.
        this.transform.position = new_position;
    }
}
