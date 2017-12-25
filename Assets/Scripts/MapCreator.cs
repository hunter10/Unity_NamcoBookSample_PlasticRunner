using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public enum TYPE
    {
        NONE = -1,  // 없음
        FLOOR = 0,  // 마루
        HOLE,       // 구멍.
        NUM,        // 블록이 몇 종류인지 나타낸다.
    }
}

public class MapCreator : MonoBehaviour {
    public static float BLOCK_WIDTH = 1.0f;             // 블록의 폭.
    public static float BLOCK_HEIGHT = 0.2f;            // 블록의 높이.
    public static int BLOCK_NUM_IN_SCREEN = 24;         // 화면 내에 들어가는 블록의 개수.

    private LevelControl level_control = null;

    private struct FloorBlock
    {
        public bool is_created;                         // 블록이 만들어졌는가.
        public Vector3 position;                        // 블록의 위치.
    };

    private FloorBlock last_block;                      // 마지막에 생성한 블록.
    private PlayerControl player = null;                // 씬상의 Player를 보관 
    private BlockCreator block_creator;                 // BlockCreator를 보관 

	void Start () {
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        this.last_block.is_created = false;
        this.block_creator = this.gameObject.GetComponent<BlockCreator>();

        this.level_control = new LevelControl();
        this.level_control.initialize();
	}
	
	// Update is called once per frame
	void Update () {
        // 플레이어의 x위치를 가져온다.
        float block_generate_x = this.player.transform.position.x;

        // 그리고 대략 반 화면만큼 오른쪽으로 이동
        // 이 위치가 블록을 생성하는 문턱 값이 된다.
        block_generate_x += BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN + 1) / 2.0f;

        // 마지막에 만든 블록의 위치가 문턱 값보다 작으면
        while(this.last_block.position.x < block_generate_x)
        {
            this.create_floor_block();
        }
	}

    private void create_floor_block()
    {
        Vector3 block_position;                     // 이제부터 만들 블록의 위치.
        if(!this.last_block.is_created){            // last_block이 생성되지 않은 경우. 
            // 블록의 위치를 일단 Player와 같게 한다.
            block_position = this.player.transform.position;

            // 그러고 나서 블록의 X 위치를 화면 절반만큼 왼쪽으로 이동.
            block_position.x -= BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN / 2.0f);
            
            // 블록의 Y위치는 0으로 
            block_position.y = 0.0f;
        } 
        else // last_block이 생성된 경우 
        {
            block_position = this.last_block.position;
        }

        // 블록을 1블록만큼 오른
        block_position.x += BLOCK_WIDTH;

        //this.block_creator.createBlock(block_position);
        this.level_control.update();
        
        // level_control에 저장된 current_block(지금 만들 블록 정보)의
        // height(높이)를 씬 상의 좌표로 변환.        
        block_position.y = level_control.current_block.height * BLOCK_HEIGHT;

        // 지금 만들 블록에 관한 정보를 변수 current에 넣는다.
        LevelControl.CreatrionInfo current = this.level_control.current_block;

        // 지금 만들 블록이 바닥이면
        if(current.block_type == Block.TYPE.FLOOR)
        {
            // block_position의 위치에 블록을 실제로 생성.
            this.block_creator.createBlock(block_position);
        }
        
        this.last_block.position = block_position;
        this.last_block.is_created = true;
    }

    public bool isDelete(GameObject block_object)
    {
        bool ret = false;


        // Player로부터 반 화면만큼 왼쪽에 위치
        // 이 위치가 사라지느냐 마느냐를 결정하는 문턱 값이 됨
        float left_limit = this.player.transform.position.x - BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN / 2.0f);

        // 블록의 위치가 문턱 값보다 작으면 (왼쪽)
        if(block_object.transform.position.x < left_limit)
        {
            ret = true;
        }

        return (ret);
    }
}
