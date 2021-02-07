using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControl : MonoBehaviour
{
    private BlockRoot block_root = null;

    // Start is called before the first frame update
    void Start()
    {
        // BlockRoot ��ũ��Ʈ ��������
        this.block_root = this.gameObject.GetComponent<BlockRoot>();
        // BlockRoot ��ũ��Ʈ�� initialSetup() ȣ��
        this.block_root.InitialSetUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
