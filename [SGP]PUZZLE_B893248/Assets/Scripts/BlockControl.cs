using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public static float COLLISION_SIZE = 1.0f;      // ����� �浹 ũ��
    public static float VANISH_TIME = 3.0f;         // �� �ٰ� ����� �������� �ð�

    // �׸��忡���� ��ǥ�� ��Ÿ���� ����ü
    public struct Position
    {
        public int x;           // x��ǥ
        public int y;           // y��ǥ
    }

    // ����� ����
    public enum COLOR
    {
        None = -1,              // �� ���� ����
        PINK = 0,               // ��ȫ��
        BLUE,                   // �Ķ���
        YELLOW,                 // �����
        GREEN,                  // �ʷϻ�
        MAGENTA,                // ����Ÿ
        ORANGE,                 // ��Ȳ��
        GRAY,                   // ȸ��
        NUM,                    // COLOR�� ����
        FIRST = PINK,           // �ʱ� �÷�(PINK)
        LAST = ORANGE,          // ���� �÷�(ORNAGE)
        NORMAL_COLOR_NUM = GRAY // ���� �÷�(ȸ�� �̿��� ��)�� ��
    };

    // ����� ����
    public enum DIR4
    {
        NONE = -1,          // ���� ���� ����
        RIGHT,              // ��
        LEFT,               // ��
        UP,                 // ��
        DOWN,               // ��
        NUM,                // ������ ����
    };

    // ����� � �������� �˷��ִ� Ŭ����
    public enum STEP
    {
        NONE = -1,          // ���� ���� ����
        IDLE = 0,           // ���
        GRABBED,            // ����
        RELEASED,           // ������
        SLIDE,              // �����̵� ��
        VACANT,             // �Ҹ� ��
        RESPAWN,            // ����� ��
        FALL,               // ���� ��
        LONG_SLIDE,         // ũ�� �����̵� ��
        NUM,                // ���°� �� �������� ǥ��
    }

    public static int BLOCK_NUM_X = 9;      // ����� ��ġ�� �� �ִ� X���� �ִ� ��
    public static int BLOCK_NUM_Y = 9;      // ����� ��ġ�� �� �ִ� Y���� �ִ� ��
}

public class BlockControl : MonoBehaviour
{
    public Block.COLOR color = (Block.COLOR)0;          // ��� ��
    public BlockRoot block_root = null;                 // ����� ��
    public Block.Position pos;                          // ��� ��ǥ

    public Block.STEP step = Block.STEP.NONE;               // ���� ����
    public Block.STEP next_step = Block.STEP.NONE;          // ���� ����
    private Vector3 position_offset_initial = Vector3.zero; // ��ü �� ��ġ
    public Vector3 position_offset = Vector3.zero;          // ��ü �� ��ġ

    public float vanish_timer = -1.0f;              // ����� �Ҹ��� �� �ɸ��� �ð�
    public Block.DIR4 slide_dir = Block.DIR4.NONE;  // �����̵� ����
    public float step_timer = 0.0f;                 // ����� ��ü�� ���� �̵��ð�

    public Material opague_material;        // ������ ��Ƽ����
    public Material transparent_material;   // ������ ��Ƽ����

    private struct StepFall
    {
        public float velocity;      // ���� �ӵ�
    }

    private StepFall fall;
    
    // Start is called before the first frame update
    void Start()
    {
        this.SetColor(this.color);      // �� ������
        this.next_step = Block.STEP.IDLE;   // ���� ����� ���������
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse_position;         // ���콺 ��ġ
        this.block_root.UnprojectMousePosition(out mouse_position, Input.mousePosition);  // ���콺 ��ġ ȹ��

        // ȹ���� ���콺 ��ġ�� X�� Y������
        Vector2 mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y);

        // Ÿ�̸Ӱ� 0 �̻��̸�
        if (this.vanish_timer >= 0.0f)
        {
            // Ÿ�̸� ���� ����
            this.vanish_timer -= Time.deltaTime;

            // Ÿ�̸Ӱ� 0 �̸��̸�
            if (this.vanish_timer < 0.0f)
            {
                if(this.step != Block.STEP.SLIDE)
                {
                    this.vanish_timer = -1.0f;
                    this.next_step = Block.STEP.VACANT; // �Ҹ� ��
                }
                else
                {
                    this.vanish_timer = 0.0f;
                }
            }
        }

        this.step_timer += Time.deltaTime;
        float slide_time = 0.2f;

        if(this.next_step == Block.STEP.NONE)
        {
            switch (this.step)
            {
                case Block.STEP.SLIDE:
                    if(this.step_timer>=slide_time)
                    {
                        // �����̵� ���� ��� �Ҹ� �� VACANT ���·� ����
                        if(this.vanish_timer == 0.0f)
                        {
                            this.next_step = Block.STEP.VACANT;
                        }
                        else
                        {
                            this.next_step = Block.STEP.IDLE;
                        }
                    }
                    break;
                case Block.STEP.IDLE:
                    this.GetComponent<Renderer>().enabled = true;
                    break;
                case Block.STEP.FALL:
                    if(this.position_offset.y <= 0.0f)
                    {
                        this.next_step = Block.STEP.IDLE;
                        this.position_offset.y = 0.0f;
                    }
                    break;
            }
        }

        // '���� ���'���°� '���� ����' �̿��� ���� = '���� ���' ���°� ����� ���
        while(this.next_step != Block.STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = Block.STEP.NONE;

            switch (this.step)
            {
                case Block.STEP.IDLE:           // ���
                    this.position_offset = Vector3.zero;
                    // ��� ǥ�� ũ�⸦ ���� ũ���
                    this.transform.localScale = Vector3.one * 1.0f;
                    break;
                case Block.STEP.GRABBED:        // ����
                    // ��� ǥ�� ũ�⸦ ũ��
                    this.transform.localScale = Vector3.one * 1.2f;
                    break;
                case Block.STEP.RELEASED:       // ������
                    this.position_offset = Vector3.zero;
                    // ��� ǥ�� ũ�⸦ ���� ũ���
                    this.transform.localScale = Vector3.one * 1.0f;
                    break;
                case Block.STEP.VACANT:
                    this.position_offset = Vector3.zero;
                    this.SetVisible(false);     // ����� ǥ������ ����
                    break;
                case Block.STEP.RESPAWN:
                    //���� ���������Ͽ� ����� ������ ����
                    int color_index = Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
                    this.SetColor((Block.COLOR)color_index);
                    this.next_step = Block.STEP.IDLE;
                    break;
                case Block.STEP.FALL:
                    this.SetVisible(true);      // ��� ǥ��
                    this.fall.velocity = 0.0f;  // ���� �ӵ� ����
                    break;
            }
            this.step_timer = 0.0f;
        }

        switch (this.step)
        {
            case Block.STEP.GRABBED:
                // �׻� �����̵� ���� Ȯ��
                this.slide_dir = this.CalcSlideDir(mouse_position_xy);
                break;
            case Block.STEP.SLIDE:
                // ��� �̵� ó��
                float rate = this.step_timer / slide_time;
                rate = Mathf.Min(rate, 1.0f);
                rate = Mathf.Sin(rate * Mathf.PI / 2.0f);
                this.position_offset = Vector3.Lerp(this.position_offset_initial, Vector3.zero, rate);
                break;
            case Block.STEP.FALL:
                // �ӵ��� �߷� �ο�
                this.fall.velocity += Physics.gravity.y * Time.deltaTime * 0.3f;
                // ���� ���� ��ġ ���
                this.position_offset.y += this.fall.velocity * Time.deltaTime;
                // �� �����Դٸ�
                if (this.position_offset.y < 0.0f)
                {
                    this.position_offset.y = 0.0f;  // �� �ڸ��� ����
                }
                break;
        }

        // �׸��� ��ǥ�� ���� ��ǥ(�� ��ǥ)�� ���ϰ� position_offset �߰�
        Vector3 position = BlockRoot.CalcBlockPosition(this.pos) + this.position_offset;
        // ���� ��ġ�� ���ο� ��ġ�� ����
        this.transform.position = position;

        this.SetColor(this.color);
        if(this.vanish_timer >= 0.0f)
        {
            // ���� ������ ���ҽð����� ����
            float vanish_time = this.block_root.level_control.GetVanishTime();

            // ���� ���� ����� �߰� ��
            Color color0 = Color.Lerp(this.GetComponent<Renderer>().material.color, Color.white, 0.5f);
            // ���� ���� �������� �߰� ��
            Color color1 = Color.Lerp(this.GetComponent<Renderer>().material.color, Color.black, 0.5f);

            // �Һٴ� ���� �ð��� ������ ������
            if (this.vanish_timer < Block.VANISH_TIME / 2.0f)
            {
                // ���� ����
                color0.a = this.vanish_timer / (Block.VANISH_TIME / 2.0f);
                color1.a = color0.a;

                // ������ ��Ƽ������ ����
                this.GetComponent<Renderer>().material = this.transparent_material;
            }

            // vanish_timer�� �پ����� 1�� �������
            float rate = 1.0f - this.vanish_timer / Block.VANISH_TIME;

            // ������ ���� �ٲ�
            this.GetComponent<Renderer>().material.color = Color.Lerp(color0, color1, rate);
        }
    
    }

    public void SetColor(Block.COLOR color)
    {
        this.color = color;         // ������ ���� ��� ������ ����
        Color color_value;         // ��

        switch (this.color)         // ��ĥ�� ���� ���� �з�
        {
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

        this.GetComponent<Renderer>().material.color = color_value;
    }

    // ����� ������ �� ȣ��
    public void BeginGrab()
    {
        this.next_step = Block.STEP.GRABBED;
    }

    // ����� �������� �� ȣ��
    public void EndGrab()
    {
        this.next_step = Block.STEP.IDLE;
    }

    // ����� ���� �� �ִ��� ���� Ȯ�� �Լ�
    public bool IsGrabbable()
    {
        bool is_grabbable = false;
        switch (this.step)
        {
            case Block.STEP.IDLE:           // ��� ���¸� ���� �� ����
                is_grabbable = true;
                break;
        }

        return is_grabbable;
    }

    // ������ ���콺 ��ǥ�� �ڽŰ� ��ġ���� ��ȯ
    public bool IsContainedPosition(Vector2 position)
    {
        bool result = false;
        Vector3 center = this.transform.position;
        float h = Block.COLLISION_SIZE / 2.0f;

        do
        {
            // X��ǥ�� �ڽŰ� ��ġ�� ������ break�� ���� Ż��
            if (position.x < center.x - h || center.x + h < position.x)
            {
                break;
            }

            // Y��ǥ�� �ڽŰ� ��ġ�� ������ break�� ���� Ż��
            if (position.y < center.y - h || center.y + h < position.y)
            {
                break;
            }

            // X��ǥ, Y��ǥ ��� ���������� ��������
            result = true;
        }
        while (false);

        return result;
    }

    public Block.DIR4 CalcSlideDir(Vector2 mouse_position)
    {
        Block.DIR4 dir = Block.DIR4.NONE;

        // ������ mouse_position�� ���� ��ġ�� ���� ��Ÿ���� ����
        Vector2 vec = mouse_position - new Vector2(this.transform.position.x, this.transform.position.y);

        //������ ũ�Ⱑ 0.1���� ũ�� �����̵�� ����
        if (vec.magnitude > 0.1f)
        {
            if(vec.y > vec.x)
            {
                if(vec.y > - vec.x)
                {
                    dir = Block.DIR4.UP;
                }
                else
                {
                    dir = Block.DIR4.LEFT;
                }
            }
            else
            {
                if(vec.y > -vec.x)
                {
                    dir = Block.DIR4.RIGHT;
                }
                else
                {
                    dir = Block.DIR4.DOWN;
                }
            }
        }

        return dir;
    }

    public float CalcDirOffset(Vector2 position, Block.DIR4 dir)
    {
        float offset = 0.0f;

        // ������ ��ġ�� ����� ������ ��ġ�� ���� ��Ÿ���� ����
        Vector2 vec = position - new Vector2(this.transform.position.x, this.transform.position.y);

        // ���� ���⿡ ����
        switch (dir)
        {
            case Block.DIR4.RIGHT:
                offset = vec.x;
                break;
            case Block.DIR4.LEFT:
                offset = -vec.x;
                break;
            case Block.DIR4.UP:
                offset = vec.y;
                break;
            case Block.DIR4.DOWN:
                offset = -vec.y;
                break;
        }

        return offset;
    }

    public void BeginSlide(Vector3 offset)
    {
        this.position_offset_initial = offset;
        this.position_offset = this.position_offset_initial;

        // ���¸� SLIDE�� ����
        this.next_step = Block.STEP.SLIDE;
    }

    // '������µ� �ɸ��� �ð�'�� ���������� ����
    public void ToVanishing()
    {
        //this.vanish_timer = Block.VANISH_TIME;
        // ���� ������ ���ҽð����� ����
        float vanish_time = this.block_root.level_control.GetVanishTime();
        this.vanish_timer = vanish_time;
    }

    // vanish_timer�� 0���� ũ�� true
    public bool IsVanishing()
    {
        bool isVanishing = (this.vanish_timer > 0.0f);
        return isVanishing;
    }

    // '������µ� �ɸ��� �ð�'�� ���������� ����
    public void RewindVanishTimer()
    {
        //this.vanish_timer = Block.VANISH_TIME;

        // ���� ������ ���ҽð����� ����
        float vanish_time = this.block_root.level_control.GetVanishTime();
        this.vanish_timer = vanish_time;
    }

    // �׸��� ����(renderer.enabled==true)�̸� ǥ��
    public bool IsVisible()
    {
        bool isVisible = this.GetComponent<Renderer>().enabled;
        return isVisible;
    }

    // �׸��� ���� ����
    public void SetVisible(bool isVisible)
    {
        this.GetComponent<Renderer>().enabled = isVisible;
    }

    // ������ Ȯ�� �Լ�
    public bool IsIdle()
    {
        bool isIdle = false;

        // ���� ����� �����, ���� ����� ������
        if(this.step == Block.STEP.IDLE && this.next_step == Block.STEP.NONE)
        {
            isIdle = true;
        }

        return isIdle;
    }

    // ���� ���� ó�� �Լ�
    public void BeginFall(BlockControl start)
    {
        this.next_step = Block.STEP.FALL;
        // ������ ��Ͽ��� ��ǥ ���
        this.position_offset.y = (float)(start.pos.y - this.pos.y) * Block.COLLISION_SIZE;
    }

    // ���� �ٲ� ���ϻ��� ǥ�� �� ���� ��ġ ���ġ �Լ�
    public void BeginRespawn(int start_pos_y)
    {
        // ���� ��ġ���� y��ǥ �̵�
        this.position_offset.y = (float)(start_pos_y - this.pos.y) * Block.COLLISION_SIZE;
        this.next_step = Block.STEP.FALL;
        //int color_index = Random.Range((int)Block.COLOR.FIRST, (int)Block.COLOR.LAST + 1);
        //this.SetColor((Block.COLOR)color_index);

        // ���� ������ ���� Ȯ���� �������� ��� �� ����
        Block.COLOR color = this.block_root.SelectBlockColor();
        this.SetColor(color);
    }

    // ����� ��ǥ�÷� �Ǿ��ִ��� Ȯ���ϴ� �Լ�
    public bool IsVacant()
    {
        bool is_vacant = false;
        if (this.step == Block.STEP.VACANT && this.next_step == Block.STEP.NONE)
        {
            is_vacant = true;
        }

        return is_vacant;
    }

    // ��ü��(�����̵� ��)���� Ȯ���ϴ� �Լ�
    public bool IsSliding()
    {
        bool is_sliding = (this.position_offset.x != 0.0f);
        return is_sliding;
    }
}
