using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public static float COLLISION_SIZE = 1.0f; // ����� �浹 ũ��.
    public static float VANISH_TIME = 3.0f; // �� �ٰ� ����� �� ������ �ð�.

    public struct iPosition
    { // �׸��忡���� ��ǥ�� ��Ÿ���� ����ü.
        public int x; // X ��ǥ.
        public int y; // Y ��ǥ.
    }

    public enum COLOR
    { // ��� ����.
        NONE = -1, // �� ���� ����.
        PINK = 0, // ��ȫ�� (���� �� ������ ���).
        BLUE,
        YELLOW,
        GREEN,
        MAGENTA,
        ORANGE,
        GRAY,
        NUM, // �÷��� �� �������� ��Ÿ����.
        FIRST = PINK,
        LAST = ORANGE,
        NORMAL_COLOR_NUM = GRAY,
    };

    public enum DIR4
    { // �����¿� �� ����.
        NONE = -1, // �������� ����. 
        RIGHT,
        LEFT,
        UP,
        DOWN,
        NUM, // ������ �� ���� �ִ��� ��Ÿ���� (=4).
    }

    public static int BLOCK_NUM_X = 9; // ����� ��ġ�� �� �ִ� X���� �ִ��.
    public static int BLOCK_NUM_Y = 9; // ����� ��ġ�� �� �ִ� Y���� �ִ��.

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
        // �� ���� ������Ʈ�� ��Ƽ���� ������ �����Ѵ�.
        this.GetComponent<Renderer>().material.color = color_value;
    }
}
