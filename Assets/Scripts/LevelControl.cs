using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class LevelData
{
    public struct Range
    {
        public int min;
        public int max;
    };

    public float end_time;
    public float player_speed;

    public Range floor_count;
    public Range hole_count;
    public Range height_diff;

    public LevelData()
    {
        this.end_time = 15.0f;
        this.player_speed = 6.0f;
        this.floor_count.min = 10;
        this.floor_count.max = 10;
        this.hole_count.min = 2;
        this.hole_count.max = 6;
        this.height_diff.min = 0;
        this.height_diff.max = 0;
    }
}

public class LevelControl : MonoBehaviour {

    private List<LevelData> level_datas = new List<LevelData>();

    public int HEIGHT_MAX = 20;
    public int HEIGHT_MIN = -4;

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

    public LevelControl()
    {
        Debug.Log("Level Control Construct!");
    }

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

    private void update_level(ref CreatrionInfo current, CreatrionInfo previous, float passage_time)
    {
        /*
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
        */

        // 레벨 1~레벨 5를 반복한다.
        float local_time = Mathf.Repeat(passage_time, this.level_datas[this.level_datas.Count - 1].end_time);

        // 현재 레벨을 구한다.
        int i;
        for (i = 0; i < this.level_datas.Count - 1;i++){
            if(local_time <= this.level_datas[i].end_time){
                break;
            }
        }
        this.level = i;

        current.block_type = Block.TYPE.FLOOR;
        current.max_count = 1;





           
        if(this.block_count >= 10){
            // 현재 레벨용 레벨 데이터를 가져온다.
            LevelData level_data;
            level_data = this.level_datas[this.level];

            switch(previous.block_type){
                case Block.TYPE.FLOOR:                      // 이전 블록이 바닥인 경우
                    current.block_type = Block.TYPE.HOLE;   // 이번엔 구멍을 만든다
                    current.max_count = UnityEngine.Random.Range(level_data.hole_count.min, level_data.hole_count.max);
                    current.height = previous.height;
                    break;

                case Block.TYPE.HOLE:                       // 이전 블록이 구멍인 경우
                    current.block_type = Block.TYPE.FLOOR;  // 이번엔 바닥을 만든다.
                    current.max_count = UnityEngine.Random.Range(level_data.floor_count.min, level_data.floor_count.max);
                    int height_min = previous.height + level_data.height_diff.min;
                    int height_max = previous.height + level_data.height_diff.max;
                    height_min = Mathf.Clamp(height_min, HEIGHT_MIN, HEIGHT_MAX);
                    height_max = Mathf.Clamp(height_max, HEIGHT_MIN, HEIGHT_MAX);

                    current.height = UnityEngine.Random.Range(height_min, height_max);
                    break;
            }
        }
    }

    //public void update()
    public void update(float passage_time)
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
            this.update_level(ref this.next_block, this.current_block, passage_time);
        }
        this.block_count++;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void loadLevelData(TextAsset level_data_text)
    {
        string level_texts = level_data_text.text;

        string[] lines = level_texts.Split('\n');

        foreach(var line in lines){
            if (line == "")
                continue;

            Debug.Log(line);
            string[] words = line.Split();
            int n = 0;

            LevelData level_data = new LevelData();
            foreach(var word in words){
                if(word.StartsWith("#", StringComparison.CurrentCulture)){
                    break;
                }
                if (word == "")
                    continue;

                switch(n){
                    case 0: level_data.end_time = float.Parse(word);
                        break;
                    case 1:
                        level_data.player_speed = float.Parse(word);
                        break;
                    case 2:
                        level_data.floor_count.min = int.Parse(word);
                        break;
                    case 3:
                        level_data.floor_count.max = int.Parse(word);
                        break;
                    case 4:
                        level_data.hole_count.min = int.Parse(word);
                        break;
                    case 5:
                        level_data.hole_count.max = int.Parse(word);
                        break;
                    case 6:
                        level_data.height_diff.min = int.Parse(word);
                        break;
                    case 7:
                        level_data.height_diff.max = int.Parse(word);
                        break;
                }
                n++;
            }

            if(n >= 8){
                this.level_datas.Add(level_data);
            } else {
                if(n == 0){
                    
                } else {
                    Debug.LogError("[LevelData] Out of parameter.\n");
                }
            }
        }

        if(this.level_datas.Count == 0){
            Debug.LogError("[LevelData] Has no data. \n");
            this.level_datas.Add(new LevelData());
        }
    }

    public float getPlayerSpeed()
    {
        return (this.level_datas[this.level].player_speed);
    }
}
