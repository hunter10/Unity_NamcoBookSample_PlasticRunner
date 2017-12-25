using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public static float ACCELERATION = 10.0f;           // 가속도
    public static float SPEED_MIN = 4.0f;               // 속도의 최솟값
    public static float SPEED_MAX = 8.0f;               // 속도의 최댓값
    public static float JUMP_HEIGHT_MAX = 3.0f;         // 점프 높이
    public static float JUMP_KEY_RELEASE_REDUCE = 0.5f; // 점프후의 감속도
    public static float NARAKU_HEIGHT = -5.0f;          // 

    public enum STEP    // 플레이어의 각종 상태를 나타내는 자료형
    {
        NONE = -1,      // 상태정보 없음
        RUN = 0,        // 달린다.
        JUMP,           // 점프
        MISS,           // 실패
        NUM,            // 상태가 몇 종류 있는지 보여준다.
    }

    public STEP step = STEP.NONE;
    public STEP next_step = STEP.NONE;

    public float step_timer = 0.0f;             // 경과시간
    private bool is_landed = false;             // 착지했는가?
    private bool is_collided = false;           // 뭔가와 충돌했는가?
    private bool is_key_released = false;       // 버튼이 떨어졌는가?

	// Use this for initialization
	void Start () {
        this.next_step = STEP.RUN;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 velocity = this.GetComponent<Rigidbody>().velocity;
        this.check_landed();                    // 착지 상태인지 체크.

        switch(this.step)
        {
            case STEP.RUN:
            case STEP.JUMP:
                if(this.transform.position.y < NARAKU_HEIGHT)
                {
                    this.next_step = STEP.MISS;
                }
                break;
        }

        this.step_timer += Time.deltaTime;      // 경과 시간을 진행한다.

        if(this.next_step == STEP.NONE)
        {
            switch (this.step)
            {
                case STEP.RUN:
                    if (!this.is_landed)
                    {
                        // 달리는 중이고 착지하지 않은 경우 아무것도 하지 않는다.
                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0)) 
                        {
                            // 달리는 중이고 착지했고 왼쪽 버튼이 눌렸다면
                            // 다음 상태를 점프로 변경
                            this.next_step = STEP.JUMP;
                        }
                    }
                    break;
                case STEP.JUMP:
                    {
                        if (this.is_landed)
                        {
                            // 점프 중이고 착지했다면 다음 상태를 주행 중으로 변경.
                            this.next_step = STEP.RUN;
                        }
                    }
                    break;
            }
        }

        // '다음 정보'가  '상태 정보 없음'이 아닌 동안(상태가 변할 때만)
        while(this.next_step != STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = STEP.NONE;

            switch (this.step)
            {
                case STEP.JUMP:
                    //점프할 높이로 점프 속도를 계산
                    velocity.y = Mathf.Sqrt(2.0f * 9.8f * PlayerControl.JUMP_HEIGHT_MAX);
                    this.is_key_released = false;
                    break;
            }

            this.step_timer = 0.0f; // 상태가 변했으므로 경과시간을 제로로 리셋
        }

        switch(this.step)
        {
            case STEP.RUN:
                // 속도를 높인다.
                velocity.x += PlayerControl.ACCELERATION * Time.deltaTime;
                if(Mathf.Abs(velocity.x) > PlayerControl.SPEED_MAX)
                {
                    velocity.x *= PlayerControl.SPEED_MAX / Mathf.Abs(this.GetComponent<Rigidbody>().velocity.x);
                }
                break;

            case STEP.JUMP:
                do
                {
                    if (!Input.GetMouseButtonUp(0))
                    {
                        break;
                    }

                    if (this.is_key_released)
                    {
                        break;
                    }

                    if(velocity.y <= 0.0f)
                    {
                        break;
                    }

                    velocity.y *= JUMP_KEY_RELEASE_REDUCE;
                    this.is_key_released = true;
                } while (false);
                break;

            case STEP.MISS:
                velocity.x -= PlayerControl.ACCELERATION * Time.deltaTime;
                if (velocity.x < 0.0f)
                    velocity.x = 0.0f;
                break;

        }

        this.GetComponent<Rigidbody>().velocity = velocity;

        this.transform.Translate(new Vector3(0.0f, 0.0f, 3.0f * Time.deltaTime));
	}

    private void check_landed()
    {
        this.is_landed = false;
        do
        {
            Vector3 s = this.transform.position;
            Vector3 e = s + Vector3.down * 1.0f;
            RaycastHit hit;
            if(!Physics.Linecast(s, e, out hit))
            {
                break;
            }

            if(this.step == STEP.JUMP)  //  현재, 점프 상태라면
            {
                // 경과 시간이 3.0f 미만이라면
                if(this.step_timer < Time.deltaTime * 3.0f)
                {
                    break;
                }
            }

            // s부터 e사이에 뭔가 있고 JUMP 직후가 아닐 때만 아래가 실행.
            this.is_landed = true;

        } while (false);
    }
}
