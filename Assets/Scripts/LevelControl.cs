using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControl : MonoBehaviour {

    public struct CreatrionInfo
    {
        public Block.TYPE block_type;
        public int max_count;
        public int height;
        public int current_count;
    }

    public CreatrionInfo previous_block;
    public CreatrionInfo current_block;
    public CreatrionInfo next_block;

    public int block_count = 0;
    public int level = 0;

    private void clear_next_block(ref CreatrionInfo block)
    {
        block.block_type = Block.TYPE.FLOOR;
        block.max_count = 15;
        block.height = 0;
        block.current_count = 0;
    }

    public void initialize()
    {
        this.block_count = 0;   // 블록의 총 수를 초기화

        this.clear_next_block(ref this.previous_block);
        this.clear_next_block(ref this.current_block);
        this.clear_next_block(ref this.next_block);
    }

    private void update_level(ref CreatrionInfo current, CreatrionInfo previous)
    {
        switch (previous.block_type)
        {
            case Block.TYPE.FLOOR:                          // 이번 블록이 바닥일 경우
                current.block_type = Block.TYPE.HOLE;       // 다음 번은 구멍을 만든다.
                current.max_count = 5;                      // 구멍은 5개 만든다.
                current.height = previous.height;           // 높이를 이전과 같게 한다.
                break;

            case Block.TYPE.HOLE:                           // 이번 블록이 구멍일 경우
                current.block_type = Block.TYPE.FLOOR;      // 다음은 바닥 만든다.
                current.max_count = 10;                     // 바닥은 10개 만든다.
                break;
        }
    }

    public void update()
    {
        // 이번에 만든 블록 개수를 증가
        this.current_block.current_count++;

        // 이번에 만든 블록 개수가 max_count 이상이면
        if(this.current_block.current_count >= this.current_block.max_count)
        {
            this.previous_block = this.current_block;
            this.current_block = this.next_block;

            //다음에 만들 블록의 내용을 초기화
            this.clear_next_block(ref this.next_block);

            // 다음에 만들 블록을 설정
            this.update_level(ref this.next_block, this.current_block);
        }
        this.block_count++;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
