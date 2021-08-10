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

    public float vanish_timer = -1.0f; // 블록이 사라질 때 까지의 시간.
    public Block.DIR4 slide_dir = Block.DIR4.NONE; // 슬라이드 된 방향.
    public float step_timer = 0.0f; // 블록이 교체된 때의 이동시간 등.

    public Material opaque_material; // 불투명 머티리얼
    public Material transparent_material; // 반투명 머티리얼

    private struct StepFall {
        public float velocity;
    }
    private StepFall fall;


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

        if (this.vanish_timer >= 0.0f) { // 타이머가 0 이상이면,
            this.vanish_timer -= Time.deltaTime; // 타이머의 값을 줄인다.
            if (this.vanish_timer < 0.0f) { // 타이머가 0 미만이면,
                if (this.step != Block.STEP.SLIDE)
                { // 슬라이드 중이 아니라면,
                    this.vanish_timer = -1.0f;
                    this.next_step = Block.STEP.VACANT; // 상태를 '소멸중'으로 전환한다.
                }
                else {
                    this.vanish_timer = 0.0f;
                }
            }
        }

        this.step_timer += Time.deltaTime;
        float slide_time = 0.2f;


        // '상태 정보 없음'의 경우.
        if (this.next_step == Block.STEP.NONE) {
            switch (this.step) {
                case Block.STEP.SLIDE:
                    if (this.step_timer >= slide_time) {
                        //슬라이드 중인 블록이 소멸되면 VACANT(사라진) 상태로 이행.
                        if (this.vanish_timer == 0.0f)
                        {
                            this.next_step = Block.STEP.VACANT;
                        }
                        else {
                            this.next_step = Block.STEP.IDLE;
                        }
                    }
                    break;

                case Block.STEP.IDLE:
                    this.GetComponent<Renderer>().enabled = true;
                    break;

                case Block.STEP.FALL:
                    if (this.position_offset.y <= 0.0f) {
                        this.next_step = Block.STEP.IDLE;
                        this.position_offset.y = 0.0f;
                    }
                    break;
            }
        }

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
                case Block.STEP.VACANT:
                    this.position_offset = Vector3.zero;
                    this.setVisible(false); // 블록을 표시하지 않게 한다.
                    break;
                case Block.STEP.RESPAWN:
                    // 색을 랜덤하게 선택하여 블록을 그 색으로 설정.
                    int color_index = Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
                    this.setColor((Block.COLOR)color_index);
                    this.next_step = Block.STEP.IDLE;
                    break;
                case Block.STEP.FALL:
                    this.setVisible(true);
                    this.fall.velocity = 0.0f;
                    break;
            }
            this.step_timer = 0.0f;

        }

        switch (this.step) {
            case Block.STEP.GRABBED:
                this.slide_dir = this.calcSlideDir(mouse_position_xy);
                break;
            case Block.STEP.SLIDE:
                float rate = this.step_timer / slide_time;
                rate = Mathf.Min(rate, 1.0f);
                rate = Mathf.Sin(rate * Mathf.PI / 2.0f);
                this.position_offset = Vector3.Lerp(this.position_offset_initial, Vector3.zero, rate);
                break;
            case Block.STEP.FALL:
                // 속도에 중력의 영향을 부여한다.
                this.fall.velocity += Physics.gravity.y * Time.deltaTime * 0.3f;
                // 세로 방향 위치를 계산.
                this.position_offset.y += this.fall.velocity * Time.deltaTime;
                if (this.position_offset.y < 0.0f) { // 다 내려왔다면,
                    this.position_offset.y = 0.0f; // 그 자리에 머무른다.
                }
                break;
        }


        //그리드 좌표를 실제 좌표 (씬의 좌표)로 변환하고, position_offset을 추가한다.
        Vector3 position = BlockRoot.calcBlockPosition(this.i_pos) + this.position_offset;

        //실제 위치를 새로운 위치로 변경한다.
        this.transform.position = position;

        this.setColor(this.color);

        if (this.vanish_timer >= 0.0f) {
            Color color0 = Color.Lerp(this.GetComponent<Renderer>().material.color, Color.white, 0.5f); // 현재 색과 흰 색의 중간색
            Color color1 = Color.Lerp(this.GetComponent<Renderer>().material.color, Color.black, 0.5f); // 현재 색과 검은 색의 중간색

            // 불붙는 연출시간이 절반을 지났다면,
            if (this.vanish_timer < Block.VANISH_TIME / 2.0f) {
                // 투명도(a)를 설정.
                color0.a = this.vanish_timer / (Block.VANISH_TIME / 2.0f);
                color1.a = color0.a;

                // 반투명 머티리얼을 적용.
                this.GetComponent<Renderer>().material = this.transparent_material;
            }
            // vanish_timer가 줄어들수록 1에 가까워진다.
            float rate = 1.0f - this.vanish_timer / Block.VANISH_TIME;
            // 서서히 색을 바꾼다.
            this.GetComponent<Renderer>().material.color = Color.Lerp(color0, color1, rate);
        }

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


    // 인수로 저장한 마우스 위치를 바탕으로, 어느 쪽으로 슬라이드되었는지 판단.
    // 반환값: 슬라이드 된 방향 (Block.DIR4 형태로 반환)
    // 블록을 교체할 것인지 판정하기 위해 BlockRoot클래스의 Update()에서 호출되는 메서드.
    public Block.DIR4 calcSlideDir(Vector2 mouse_position)
    {
        Block.DIR4 dir = Block.DIR4.NONE;

        Vector2 v = mouse_position - new Vector2(this.transform.position.x, this.transform.position.y);
        // 지정된 mouse_position과 현재 위치의 차를 나타내는 벡터

        // 슬라이드 하면 = 벡터의 크기가 0.1보다 크면
        // (0.1보다 작으면 슬라이드 하지 않은 걸로 간주한다 -> 문턱 값.)
        if (v.magnitude > 0.1f) {
            if (v.y > v.x)
            {
                if (v.y > -v.x)
                {
                    dir = Block.DIR4.UP;
                }
                else
                {
                    dir = Block.DIR4.LEFT;
                }
            }
            else {
                if (v.y > -v.x)
                {
                    dir = Block.DIR4.RIGHT;
                }
                else {
                    dir = Block.DIR4.DOWN;
                }
            }
        }
        return (dir);
    }

    // 인수로 지정된 위치와 방향을 근거로, 현재 위치와 슬라이드 할 곳의 거리가 어느 정도인지 반환 (반환 자료형: 실수).
    // calcSlideDir()와 마찬가지로 BlockRoot클래스의 Update 메서드로부터 호출됨.
    public float calcDirOffset(Vector2 position, Block.DIR4 dir) {
        float offset = 0.0f;

        Vector2 v = position - new Vector2(this.transform.position.x, this.transform.position.y);
        // 지정된 위치와 블록의 현재 위치의 차를 나타내는 벡터.

        switch (dir) {
            case Block.DIR4.RIGHT: 
                offset = v.x;
                break;
            case Block.DIR4.LEFT:
                offset = -v.x;
                break;
            case Block.DIR4.UP:
                offset = v.y;
                break;
            case Block.DIR4.DOWN:
                offset = -v.x;
                break;
        }

        return (offset);
    }

    // 블록의 상태를 'SLIDE' 로 변경.
    // 블록의 상태가 SLIDE가 되면 이동이 시작되므로, 이동이 시작됨을 알리는 메서드에 해당한다.
    public void beginSlide(Vector3 offset) {
        this.position_offset_initial = offset;
        this.position_offset = this.position_offset_initial;

        this.next_step = Block.STEP.SLIDE;
        //상태를 SLIDE로 변경.
    }

    public void toVanishing() {
        // '사라질 때 까지 걸리는 시간'을 규정값으로 리셋.
        this.vanish_timer = Block.VANISH_TIME;
    }

    public bool isVanishing() {
        // vanish_timer가 0보다 크면 true.
        bool is_vanishing = (this.vanish_timer > 0.0f);
        return (is_vanishing);
    }

    public void rewindVanishTimer() {
        // '사라질 때 까지 걸리는 시간'을 규정값으로 리셋.
        this.vanish_timer = Block.VANISH_TIME;
    }

    public bool isVisible()
    {
        // 그리기 가능(renderer.enabled가 true)상태라면,
        // 표시되고 있다.
        bool is_visible = this.GetComponent<Renderer>().enabled;
        return (is_visible);
    }

    public void setVisible(bool is_visible) {

        // '그리기 가능' 설정에 인수를 대입.
        this.GetComponent<Renderer>().enabled = is_visible;
    }

    public bool isIdle() { 
        bool is_idle = false;
        // 현재 블록 상태가 '대기 중'이고,
        // 다음 블록 상태가 '없음' 이면,
        if (this.step == Block.STEP.IDLE && this.next_step == Block.STEP.NONE)
        {
            is_idle = true;
        }
        return (is_idle);
    }

    public void beginFall(BlockControl start) {
        this.next_step = Block.STEP.FALL;
        // 지정된 블록에서 좌표를 계산해 낸다.
        this.position_offset.y = (float)(start.i_pos.y - this.i_pos.y) * Block.COLLISION_SIZE;
    }

    public void beginRespawn(int start_ipos_y) {
        // 지정위치까지 y좌표를 이동한다.
        this.position_offset.y = (float)(start_ipos_y - this.i_pos.y) * Block.COLLISION_SIZE;
        this.next_step = Block.STEP.FALL;
        int color_index = Random.Range((int)Block.COLOR.FIRST, (int)Block.COLOR.LAST +1);
        this.setColor((Block.COLOR)color_index);
    }

    public bool isVacant() {
        bool is_vacant = false;
        if (this.step == Block.STEP.VACANT && this.next_step == Block.STEP.NONE) {
            is_vacant = true;
        }
        return (is_vacant);
    }

    public bool isSliding() {
        bool is_sliding = (this.position_offset.x != 0.0f);
        return (is_sliding);
    }

}
