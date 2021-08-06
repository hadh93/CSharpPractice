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

    public static int BLOCK_NUM_X = 9; // 블록을 배치할 수 있는 X방향 최대수.
    public static int BLOCK_NUM_Y = 9; // 블록을 배치할 수 있는 Y방향 최대수.

}

public class BlockControl : MonoBehaviour
{

    

    public Block.COLOR color = (Block.COLOR)0;
    public BlockRoot block_root = null;
    public Block.iPosition i_pos;


    // Start is called before the first frame update
    void Start()
    {
        this.setColor(this.color);
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
