using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRoot : MonoBehaviour
{
    public GameObject BlockPrefab = null; // ���� ����� ������.
    public BlockControl[,] blocks; // �׸���.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //����� ����� ���� ���� 9ĭ, ���� 9ĭ�� ��ġ�Ѵ�.
    public void initialSetUp()
    {
        // �׸����� ũ�⸦ 9 x 9 �� �Ѵ�.
        this.blocks = new BlockControl[Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y];
        int color_index = 0;

        for (int y = 0; y < Block.BLOCK_NUM_Y; y++) {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++) {
                // BlockPrefab�� �ν��Ͻ��� ���� �����.
                GameObject game_object = Instantiate(this.BlockPrefab) as GameObject;
                // ������ ���� ����� BlockControl Ŭ������ �����´�.
                BlockControl block = game_object.GetComponent<BlockControl>();
                // ����� �׸��忡 �����Ѵ�
                this.blocks[x, y] = block;

                // ����� ��ġ ���� (�׸��� ��ǥ) �� �����Ѵ�.
                block.i_pos.x = x;
                block.i_pos.y = y;
                // �� BlockControl�� ������ GameRoot�� �ڽ��̶�� �����Ѵ�.
                block.block_root = this;

                // �׸��� ��ǥ�� ���� ��ġ(���� ��ǥ)�� ��ȯ�Ѵ�.
                Vector3 position = BlockRoot.calcBlockPosition(block.i_pos);
                // ���� ��� ��ġ�� �̵��Ѵ�.
                block.transform.position = position;
                // ����� ���� �����Ѵ�.
                block.setColor((Block.COLOR)color_index);
                // ����� �̸��� ����(�ļ�)�Ѵ�.
                block.name = "block(" + block.i_pos.x.ToString() + "," + block.i_pos.y.ToString() + ")";

                // ��ü �� �߿��� ���Ƿ� �ϳ��� ���� �����Ѵ�.

                color_index = Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
            }
        }        
    }

    public static Vector3 calcBlockPosition(Block.iPosition i_pos)
    {

        // ��ġ�� ���� �� ���� ��ġ�� �ʱ갪���� �����Ѵ�.
        Vector3 position = new Vector3(-(Block.BLOCK_NUM_X / 2.0f - 0.5f), -(Block.BLOCK_NUM_Y / 2.0f - 0.5f), 0.0f);

        // �ʱ갪 + �׸��� ��ǥ x ��� ũ��.

        position.x += (float)i_pos.x * Block.COLLISION_SIZE;
        position.y += (float)i_pos.y * Block.COLLISION_SIZE;

        return (position);

    }


}
