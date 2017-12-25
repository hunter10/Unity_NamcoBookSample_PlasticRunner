using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour {
    public static float BLOCK_WIDTH = 1.0f;             // 블록의 폭.
    public static float BLOCK_HEIGHT = 0.2f;            // 블록의 높이.
    public static int BLOCK_NUM_IN_SCREEN = 24;         // 화면 내에 들어가는 블록의 개수.

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
	}
	
	// Update is called once per frame
	void Update () {
		
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

        this.block_creator.createBlock(block_position);

        this.last_block.position = block_position;
        this.last_block.is_created = true;
    }
}
