using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRoot : MonoBehaviour
{
    public GameObject BlockPrefab = null; // 만들어낼 블록의 프리팹.
    public BlockControl[,] blocks; // 그리드.

    private GameObject main_camera = null; // 메인 카메라.
    private BlockControl grabbed_block = null; // 잡은 블록.

    // Start is called before the first frame update
    void Start()
    {
        this.main_camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse_position; // 마우스 위치
        this.unprojectMousePosition(out mouse_position, Input.mousePosition); // 마우스 위치를 가져온다.

        Vector2 mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y); // 가져온 마우스 위치를 하나의 Vector2로 모은다.
        if (this.grabbed_block == null) { // 잡은 블록이 비었으면,
            // if (!this.is_has_falling_block()){
            if (Input.GetMouseButtonDown(0)) {  // 마우스 버튼이 눌렸으면,
                // blocks 배열의 모든 요소를 차례로 처리한다.
                foreach (BlockControl block in this.blocks) {
                    if (!block.isGrabbable()) { // 블록을 잡을 수 없다면,
                        continue; // 루프의 처음으로 점프한다.
                    }
                    // 마우스 위치가 블록 영역 안이 아니면.
                    if (!block.isContainedPosition(mouse_position_xy)) {
                        continue; // 루프의 처음으로 점프한다. 
                    }
                    // 처리 중인 블록을 grabbed_block에 등록한다.
                    this.grabbed_block = block;
                    // 잡았을 때의 처리를 실행한다.
                    this.grabbed_block.beginGrab();
                    break;
                }
            }
            //}
        } else { // 블록을 잡았을 때
            if (!Input.GetMouseButton(0)) { // 마우스 버튼이 눌려져 있지 않으면,
                this.grabbed_block.endGrab(); //블록을 놨을 때의 처리를 실행.
                this.grabbed_block = null; // grabbed_block을 비우게 설정.
            }
        }
    }

    //블록을 만들어 내고 가로 9칸, 세로 9칸에 배치한다.
    public void initialSetUp()
    {
        // 그리드의 크기를 9 x 9 로 한다.
        this.blocks = new BlockControl[Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y];
        int color_index = 0;

        for (int y = 0; y < Block.BLOCK_NUM_Y; y++) {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++) {
                // BlockPrefab의 인스턴스를 씬에 만든다.
                GameObject game_object = Instantiate(this.BlockPrefab) as GameObject;
                // 위에서 만든 블록의 BlockControl 클래스를 가져온다.
                BlockControl block = game_object.GetComponent<BlockControl>();
                // 블록을 그리드에 저장한다
                this.blocks[x, y] = block;

                // 블록의 위치 정보 (그리드 좌표) 를 설정한다.
                block.i_pos.x = x;
                block.i_pos.y = y;
                // 각 BlockControl이 연계할 GameRoot는 자신이라고 설정한다.
                block.block_root = this;

                // 그리드 좌표를 실제 위치(씬의 좌표)로 변환한다.
                Vector3 position = BlockRoot.calcBlockPosition(block.i_pos);
                // 씬의 블록 위치를 이동한다.
                block.transform.position = position;
                // 블록의 색을 변경한다.
                block.setColor((Block.COLOR)color_index);
                // 블록의 이름을 설정(후술)한다.
                block.name = "block(" + block.i_pos.x.ToString() + "," + block.i_pos.y.ToString() + ")";

                // 전체 색 중에서 임의로 하나의 색을 선택한다.

                color_index = Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
            }
        }        
    }

    public static Vector3 calcBlockPosition(Block.iPosition i_pos)
    {

        // 배치할 왼쪽 위 구석 위치를 초깃값으로 설정한다.
        Vector3 position = new Vector3(-(Block.BLOCK_NUM_X / 2.0f - 0.5f), -(Block.BLOCK_NUM_Y / 2.0f - 0.5f), 0.0f);

        // 초깃값 + 그리드 좌표 x 블록 크기.

        position.x += (float)i_pos.x * Block.COLLISION_SIZE;
        position.y += (float)i_pos.y * Block.COLLISION_SIZE;

        return (position);

    }

    public bool unprojectMousePosition(out Vector3 world_position, Vector3 mouse_position) {
        bool ret;

        // 판을 작성한다. 이 판은 카메라에 대해서 뒤로 향해서 (Vector3.back).
        // 블록의 절반 크기만큼 앞에 둔다.
        Plane plane = new Plane(Vector3.back, new Vector3(0.0f, 0.0f, -Block.COLLISION_SIZE / 2.0f));

        // 카메라와 마우스를 통과하는 빛을 만든다.
        Ray ray = this.main_camera.GetComponent<Camera>().ScreenPointToRay(mouse_position);

        float depth;

        // 광선(ray)이 판(plane)에 닿았다면,
        if (plane.Raycast(ray, out depth)) { // 광선이 닿았다면 depth에 정보가 기록된다.
            // 인수 world_position을 마우스 위치로 덮어쓴다.
            world_position = ray.origin + ray.direction * depth;
            ret = true;
        } else {
            // 인수 world_position을 0인 벡터로 덮어쓴다.
            world_position = Vector3.zero;
            ret = false;
        }
        return (ret);
    }


}
