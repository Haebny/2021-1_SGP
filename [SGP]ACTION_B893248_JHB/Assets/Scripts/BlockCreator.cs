using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCreator : MonoBehaviour
{
    public static BlockCreator Instance;  // 싱글톤으로 구현

    public GameObject[] poolingObjectPrefab;  // 오브젝트 풀에 넣을 프리팹
    Queue<GameObject> poolingObjectQueue = new Queue<GameObject>(); // 풀링할 오브젝트를 저장하는 큐

    private int block_count = 0; // 생성한 블록의 개수.
    private int count = 0;

    private void Awake()
    {
        Instance = this;
    }

    //private void Initialize(int size)
    //{
    //    LevelControl.CreationInfo block;

    //    for (int i = 0; i < size; i++)
    //    {
    //        switch (i)
    //        {
    //            case 0:
    //                block.block_type = Block.TYPE.FLOOR;
    //                break;
    //            case 1:
    //                block.block_type = Block.TYPE.FLOOR1;
    //                break;
    //            case 2:
    //                block.block_type = Block.TYPE.FLOOR2;
    //                break;
    //            case 3:
    //                block.block_type = Block.TYPE.FLOOR3;
    //                break;
    //            case 4:
    //                block.block_type = Block.TYPE.FLOOR4;
    //                break;
    //            case 5:
    //                block.block_type = Block.TYPE.OBSTACLE_R;
    //                break;
    //            case 6:
    //                block.block_type = Block.TYPE.KEY;
    //                break;
    //            case 7:
    //                block.block_type = Block.TYPE.BOX;
    //                break;
    //            case 8:
    //                block.block_type = Block.TYPE.SLOPE;
    //                break;
    //            case 9:
    //                block.block_type = Block.TYPE.OBSTACLE_T;
    //                break;
    //            case 10:
    //                block.block_type = Block.TYPE.OBSTACLE_F;
    //                break;
    //            default:
    //                block.block_type = Block.TYPE.FLOOR;
    //                break;
    //        }

    //        poolingObjectQueue.Enqueue(CreateNewObject(block.block_type));
    //    }
    //}

    private GameObject CreateNewObject(Block.TYPE type)
    {
        var newObj = GameObject.Instantiate(this.poolingObjectPrefab[(int)type]) as GameObject;
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }

    public static GameObject GetObject(Vector3 block_position, Block.TYPE next_block_type)
    {
        Debug.Log(next_block_type);
        if (next_block_type == Block.TYPE.SLOPE)
            BlockCreator.Instance.count++;

        block_position.y -= BlockCreator.Instance.count * 3;

        // 오브젝트 풀이 비어있지 않은 경우
        if (BlockCreator.Instance.poolingObjectQueue.Count > 0)
        {
            int i = 0;
            int size = BlockCreator.Instance.poolingObjectQueue.Count;

            while (i < size)
            {
                var obj = BlockCreator.Instance.poolingObjectQueue.Dequeue();
                switch (next_block_type)
                {
                    case Block.TYPE.FLOOR:
                        if (!obj.name.Contains("Flat"))
                            BlockCreator.Instance.poolingObjectQueue.Enqueue(obj);
                        break;
                    case Block.TYPE.FLOOR1:
                        if (!obj.name.Contains("Land A"))
                            BlockCreator.Instance.poolingObjectQueue.Enqueue(obj);
                        break;
                    case Block.TYPE.FLOOR2:
                        if (!obj.name.Contains("Land B"))
                            BlockCreator.Instance.poolingObjectQueue.Enqueue(obj);
                        break;
                    case Block.TYPE.FLOOR3:
                        if (!obj.name.Contains("Land C"))
                            BlockCreator.Instance.poolingObjectQueue.Enqueue(obj);
                        break;
                    case Block.TYPE.FLOOR4:
                        if (!obj.name.Contains("Land D"))
                            BlockCreator.Instance.poolingObjectQueue.Enqueue(obj);
                        break;
                    case Block.TYPE.OBSTACLE_R:
                        if (!obj.name.Contains("_R"))
                            BlockCreator.Instance.poolingObjectQueue.Enqueue(obj);
                        break;
                    case Block.TYPE.KEY:
                        if (!obj.name.Contains("_Key"))
                            BlockCreator.Instance.poolingObjectQueue.Enqueue(obj);
                        break;
                    case Block.TYPE.BOX:
                        if (!obj.name.Contains("_Box"))
                            BlockCreator.Instance.poolingObjectQueue.Enqueue(obj);
                        break;
                    case Block.TYPE.SLOPE:
                        if (!obj.name.Contains("Slope"))
                            BlockCreator.Instance.poolingObjectQueue.Enqueue(obj);
                        break;
                    case Block.TYPE.OBSTACLE_T:
                        if (!obj.name.Contains("_T"))
                            BlockCreator.Instance.poolingObjectQueue.Enqueue(obj);
                        break;
                    case Block.TYPE.OBSTACLE_F:
                        if (!obj.name.Contains("_F"))
                            BlockCreator.Instance.poolingObjectQueue.Enqueue(obj);
                        break;
                    default:
                        break;
                }

                // deque 되지 않았으면
                if (size == BlockCreator.Instance.poolingObjectQueue.Count)
                {
                    i++;
                    continue;
                }

                obj.transform.SetParent(null);
                obj.gameObject.SetActive(true);

                // 블록의 위치 세팅
                obj.transform.position = block_position; // 블록의 위치를 이동.
                BlockCreator.Instance.block_count++; // 블록의 개수를 증가.

                return obj;
            }
        }

        var newObject = Instance.CreateNewObject(next_block_type);
        newObject.gameObject.SetActive(true);
        newObject.transform.SetParent(null);

        // 블록의 위치 세팅
        newObject.transform.position = block_position; // 블록의 위치를 이동.
        Instance.block_count++; // 블록의 개수를 증가.

        return newObject;
    }

    public static void ReturnObject(GameObject obj)
    {
        if (obj.transform.GetChild(0).CompareTag("Obstacle") || obj.transform.GetChild(0).CompareTag("Key") || obj.transform.GetChild(0).CompareTag("Box"))
        {
            obj.transform.GetChild(0).gameObject.SetActive(true);
        }

        obj.gameObject.SetActive(false);
        obj.transform.SetParent(BlockCreator.Instance.transform);
        BlockCreator.Instance.poolingObjectQueue.Enqueue(obj);
    }

    //public void CreateBlock(Vector3 block_position, Block.TYPE next_block_type)
    //{
    //    if (next_block_type == Block.TYPE.SLOPE)
    //        count++;

    //    block_position.y -= count * 3;

    //    // 블록을 생성하고 go에 보관한다.
    //    GameObject go = GameObject.Instantiate(this.poolingObjectPrefab[(int)next_block_type]) as GameObject;
    //    go.transform.position = block_position; // 블록의 위치를 이동.
    //    this.block_count++; // 블록의 개수를 증가.
    //}

}