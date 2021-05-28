using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    FireController fireController;
    private PlayerControl player;
    private float distance;

    private void Start()
    {
        fireController = GameObject.FindObjectOfType<FireController>();
        player = GameObject.FindObjectOfType<PlayerControl>().GetComponent<PlayerControl>();
    }

    private void Update()
    {
        CheckDistance();

        if(this.transform.position.x > player.transform.position.x)
        {
            fireController.velocity = Vector3.zero;
            fireController.level = FireController.LEVEL.ERROR;
        }
    }

    // 플레이어와의 거리 계산
    public FireController.LEVEL CheckDistance()
    {
        int distance = (int)Mathf.Abs(Vector3.Distance(this.transform.position, player.transform.position));

        if (distance > 20)
            return FireController.LEVEL.LEVEL1;
        else if (distance > 10)
            return FireController.LEVEL.LEVEL2;
        else
            return FireController.LEVEL.LEVEL3;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            fireController.velocity = Vector3.zero;
            fireController.level = FireController.LEVEL.ERROR;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            fireController.velocity = Vector3.zero;
            fireController.level = FireController.LEVEL.ERROR;
        }
    }
}
