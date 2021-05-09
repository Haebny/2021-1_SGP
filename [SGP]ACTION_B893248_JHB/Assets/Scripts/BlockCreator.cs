using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCreator : MonoBehaviour
{
    public GameObject[] blockPrefabs; // 블록을 저장할 배열.
    private int block_count = 0; // 생성한 블록의 개수.

    public void CreateBlock(Vector3 block_position, Block.TYPE next_block_type)
    {
        // 블록을 생성하고 go에 보관한다.
        GameObject go = GameObject.Instantiate(this.blockPrefabs[(int)next_block_type]) as GameObject;
        go.transform.position = block_position; // 블록의 위치를 이동.
        this.block_count++; // 블록의 개수를 증가.
    }
}
