using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    public GameObject[] poolingObjectPrefab; // 오브젝트 풀에 넣을 프리팹

    Queue<GameObject> poolingObjectQueue = new Queue<GameObject>(); // 풀링할 오브젝트를 저장하는 큐

    private void Awake()
    {
        Instance = this;
        Initialize(11);
    }

    private void Initialize(int size)
    {
        for(int i=0; i<size; i++)
        {
            poolingObjectQueue.Enqueue(CreateNewObject());
        }
    }

    private GameObject CreateNewObject()
    {
        var newObj = Instantiate(poolingObjectPrefab[Random.Range(0, poolingObjectPrefab.Length)]).GetComponent<GameObject>();
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }

    public static GameObject GetObject()
    {
        // 오브젝트 풀에 오브젝트가 있는 경우
        if (Instance.poolingObjectQueue.Count > 0) 
        {
            var obj = Instance.poolingObjectQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObject = Instance.CreateNewObject();
            newObject.gameObject.SetActive(true);
            newObject.transform.SetParent(null);
            return newObject;
        }
    }

    public static void ReturnObject(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.poolingObjectQueue.Enqueue(obj);
    }
}
