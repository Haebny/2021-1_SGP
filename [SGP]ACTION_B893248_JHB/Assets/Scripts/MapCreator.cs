using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Block 클래스 추가
public class Block
{
    // 블록의 종류를 나타내는 열거체.
    public enum TYPE
    {
        NONE = -1, // 없음.
        FLOOR = 0, // 평면
        FLOOR1 = 1, // 평면
        FLOOR2 = 2, // 평면
        FLOOR3 = 3, // 평면
        FLOOR4 = 4, // 평면
        OBSTACLE_R,  // 평지 장애물 바위
        KEY, // 열쇠
        BOX, // 박스
        SLOPE, // 급경사면
        OBSTACLE_T,  // 평지 장애물 비석
        OBSTACLE_F,  // 평지 장애물 펜스
        NUM // 블록이 몇 종류인지(＝5).
    };
};

public class MapCreator : MonoBehaviour
{
    public static float BLOCK_WIDTH = 1.0f; // 블록의 폭.
    public static float BLOCK_HEIGHT = -6f; // 블록의 높이.
    public static int BLOCK_NUM_IN_SCREEN = 50; // 화면 내에 들어가는 블록의 개수.
    private LevelControl level_control = null;
    //public TextAsset level_data_text = null;
    private GameRoot game_root = null;

    private struct FloorBlock
    {
        // 블록에 관한 정보를 모아서 관리하는 구조체 (여러 개의 정보를 하나로 묶을 때 사용).
        public bool is_created; // 블록이 만들어졌는가.
        public Vector3 position; // 블록의 위치.
    };

    private FloorBlock last_block; // 마지막에 생성한 블록.
    private PlayerControl player = null; // 씬상의 Player를 보관.

    void Start()
    {
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>(); 
        this.last_block.is_created = false;

        this.level_control = this.gameObject.AddComponent<LevelControl>();
        this.level_control.Initialize();

        //this.level_control.LoadLevelData(this.level_data_text);

        this.game_root = this.gameObject.GetComponent<GameRoot>();

        this.player.level_control = this.level_control;
    }

    private void CreateFloorBlock()
    {
        Vector3 block_position; // 이제부터 만들 블록의 위치.
       
        // 블록이 생성되지 않았을 경우 설정
        if(!this.last_block.is_created)
        {
            // 블록의 위치를 일단 Player와 같게 한다.
            block_position = this.player.transform.position;
            // 블록의 X 위치를 화면 절반만큼 왼쪽으로 이동.
            block_position.x -= BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN / 2.0f);
        }
        else
        {
            // last_block이 생성된 경우 이번에 만들 블록의 위치를 직전에 만든 블록과 같게.
            block_position = this.last_block.position;
        }

        block_position.x += BLOCK_WIDTH; // 블록을 1블럭만큼 오른쪽으로 이동.

        this.level_control.update(); // LevelControl을 갱신.

        // level_control에 저장된 current_block(지금 만들 블록 정보)의 height(높이)를 씬 상의 좌표로 변환.
        block_position.y = level_control.current_block.height * BLOCK_HEIGHT;

        // 지금 만들 블록에 관한 정보를 변수 current에 넣는다.
        LevelControl.CreationInfo current = this.level_control.current_block;

        // 지금 만들 블록이 바닥이면 (지금 만들 블록이 장애물이라면)
        if (current.block_type == Block.TYPE.FLOOR || current.block_type == Block.TYPE.FLOOR1
            || current.block_type == Block.TYPE.FLOOR2 || current.block_type == Block.TYPE.FLOOR3
            || current.block_type == Block.TYPE.FLOOR4)
        {
            level_control.SetFloorType(ref current);
        }

        // block_position의 위치에 블록을 실제로 생성.
        BlockCreator.GetObject(block_position, current.block_type);

        this.last_block.position = block_position; // last_block의 위치를 이번 위치로 갱신.
        this.last_block.is_created = true; // 블록이 생성되었으므로 last_block의 is_created를 true로 설정.
    }

    void Update()
    {
        // 플레이어의 X위치를 가져온다.
        float block_generate_x = this.player.transform.position.x;
        
        // 그리고 대략 반 화면만큼 오른쪽으로 이동.
        // 이 위치가 블록을 생성하는 문턱 값이 된다.
        block_generate_x += BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN + 1) / 2.0f;

        // 마지막에 만든 블록의 위치가 문턱 값보다 작으면.
        while (this.last_block.position.x < block_generate_x)
        {
            // 블록을 만든다.
            this.CreateFloorBlock();
        }
    }

    public bool IsOnCam(GameObject block_object)
    {
        bool ret = false; // 반환값.
                          
        // Player로부터 반 화면만큼 왼쪽에 위치, 이 위치가 사라지느냐 마느냐를 결정하는 문턱 값이 됨.
        float left_limit = this.player.transform.position.x - BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN * 2 / 2.0f);
        // 블록의 위치가 문턱 값보다 작으면(왼쪽),
        if (block_object.transform.position.x < left_limit)
        {
            ret = true; // 반환값을 true(사라져도 좋다)로
        }
        return (ret); // 판정 결과를 돌려줌.
    }
}
