using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public static float COLLISION_SIZE = 1.0f; // 블록의 충돌 크기.
    public static float VANISH_TIME = 3.0f; // 불 붙고 사라질 때 까지의 시간.

    public struct iPosition
    { // 그리드에서의 좌표를 나타내는 구조체.
        public int x; // X 좌표.
        public int y; // Y 좌표.
    }

    public enum COLOR
    { // 블록 색상.
        NONE = -1, // 색 지정 없음.
        PINK = 0, // 분홍색 (이하 각 색상의 상수).
        BLUE,
        YELLOW,
        GREEN,
        MAGENTA,
        ORANGE,
        GRAY,
        NUM, // 컬러가 몇 종류인지 나타낸다.
        FIRST = PINK,
        LAST = ORANGE,
        NORMAL_COLOR_NUM = GRAY,
    };

    public enum DIR4
    { // 상하좌우 네 방향.
        NONE = -1, // 방향지정 없음. 
        RIGHT,
        LEFT,
        UP,
        DOWN,
        NUM, // 방향이 몇 종류 있는지 나타낸다 (=4).
    }

    public enum STEP { // 블록의 상태 표시.
        NONE = -1, // 상태 정보 없음.
        IDLE = 0, // 대기중.
        GRABBED, // 잡혀 있음.
        RELEASED, // 떨어진 순간.
        SLIDE, // 슬라이드 중.
        VACANT, // 소멸 중.
        RESPAWN, // 재생성 중.
        FALL, // 낙하 중.
        LONG_SLIDE, // 크게 슬라이드 중.
        NUM, // 상태가 몇 종류인지 표시.
    }

    public static int BLOCK_NUM_X = 9; // 블록을 배치할 수 있는 X방향 최대수.
    public static int BLOCK_NUM_Y = 9; // 블록을 배치할 수 있는 Y방향 최대수.

}

public class BlockControl : MonoBehaviour
{

    

    public Block.COLOR color = (Block.COLOR)0;
    public BlockRoot block_root = null;
    public Block.iPosition i_pos;

    public Block.STEP step = Block.STEP.NONE;
    public Block.STEP next_step = Block.STEP.NONE;
    private Vector3 position_offset_initial = Vector3.zero;
    public Vector3 position_offset = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        this.setColor(this.color);
        this.next_step = Block.STEP.IDLE; // 다음 블록을 대기중으로.
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse_position; // 마우스 위치.
        this.block_root.unprojectMousePosition(out mouse_position, Input.mousePosition); // 마우스 위치 획득.

        // 획득한 마우스 위치를 X와 Y만으로 한다.
        Vector2 mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y);

        // '다음 블록' 상태가 '정보 없음' 이외인 동안.
        // = '다음 블록' 상태가 변경된 경우.

        while (this.next_step != Block.STEP.NONE) {
            this.step = this.next_step;
            this.next_step = Block.STEP.NONE;

            switch (this.step) {
                case Block.STEP.IDLE: // '대기(IDLE)' 상태.
                    this.position_offset = Vector3.zero;
                    // 블록 표시 크기를 보통으로 한다.
                    this.transform.localScale = Vector3.one * 1.0f;
                    break;
                case Block.STEP.GRABBED: // '잡힌(GRABBED)' 상태.
                    // 블록 표시 크기를 크게 한다.
                    this.transform.localScale = Vector3.one * 1.2f;
                    break;
                case Block.STEP.RELEASED:
                    this.position_offset = Vector3.zero;
                    // 블록 표시 크기를 보통으로 한다.
                    this.transform.localScale = Vector3.one * 1.0f;
                    break;
            }

        }
        //그리드 좌표를 실제 좌표 (씬의 좌표)로 변환하고, position_offset을 추가한다.
        Vector3 position = BlockRoot.calcBlockPosition(this.i_pos) + this.position_offset;

        //실제 위치를 새로운 위치로 변경한다.
        this.transform.position = position;

    }
    public void setColor(Block.COLOR color)
    {
        this.color = color;
        Color color_value;

        switch (this.color) {
            default:
            case Block.COLOR.PINK:
                color_value = new Color(1.0f, 0.5f, 0.5f);
                break;
            case Block.COLOR.BLUE:
                color_value = Color.blue;
                break;
            case Block.COLOR.YELLOW:
                color_value = Color.yellow;
                break;
            case Block.COLOR.GREEN:
                color_value = Color.green;
                break;
            case Block.COLOR.MAGENTA:
                color_value = Color.magenta;
                break;
            case Block.COLOR.ORANGE:
                color_value = new Color(1.0f, 0.46f, 0.0f);
                break;
        }
        // 이 게임 오브젝트의 머티리얼 색상을 변경한다.
        this.GetComponent<Renderer>().material.color = color_value;
    }

    public void beginGrab()
    {
        this.next_step = Block.STEP.GRABBED;
    }

    public void endGrab()
    {
        this.next_step = Block.STEP.IDLE;
    }

    public bool isGrabbable()
    {
        bool is_grabbable = false;
        switch (this.step) {
            case Block.STEP.IDLE: // '대기'상태일때만.
                is_grabbable = true; // true(잡을 수 있다)를 반환한다.
                break;
        }
        return (is_grabbable);
    }

    public bool isContainedPosition(Vector2 position)
    {
        bool ret = false;
        Vector3 center = this.transform.position;
        float h = Block.COLLISION_SIZE / 2.0f;
        do
        {
            // X좌표가 자신과 겹치지 않으면 break로 루프를 빠져 나간다.
            if (position.x < center.x - h || center.x + h < position.x)
            {
                break;
            }

            // Y좌표가 자신과 겹치지 않으면 break로 루프를 빠져 나간다.
            if (position.y < center.y - h || center.y + h < position.y)
            {
                break;
            }
            // X좌표, Y좌표 모두 겹쳐있으면 true(겹쳐있다)를 반환한다.
            ret = true;

        } while (false);

        return ret;

    }

}
