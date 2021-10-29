using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // UI클릭 시 터치 이벤트 발생 방지
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    #region Variables
    // 점프에 필요한 전역변수 선언
    public static float ACCELERATION = 10.0f; // 가속도.
    public static float SPEED_MIN = 7.0f; // 속도의 최솟값.
    public static float SPEED_MAX = 10.0f; // 속도의 최댓값.
    public static float JUMP_HEIGHT_MAX = 5.0f; // 점프 높이.
    public static float JUMP_KEY_RELEASE_REDUCE = 0.5f; // 점프 후의 감속도.

    // Player의 각종 상태를 나타내는 자료형 (*열거체)
    public enum STEP
    {
        NONE = -1, // 상태정보 없음.
        RUN = 0, // 달린다.
        JUMP, // 점프.
        FALL,   // 넘어짐
        MISS, // 게임오버.
        NUM, // 상태가 몇 종류 있는지 보여준다(=3).
    };

    public STEP step = STEP.NONE; // Player의 현재 상태.
    public STEP next_step = STEP.NONE; // Player의 다음 상태.
    public float step_timer = 0.0f; // 경과 시간.

    // 플레이어 캐릭터 타입
    public enum ANIMAL_TYPE
    {
        DOG = 0,
        CAT,
        CHICKEN
    }
    private ANIMAL_TYPE animalType;
    public ANIMAL_TYPE AnimalType { get { return animalType; } }

    // 돌진이 발동한 경로를 나타내는 자료형 
    public enum DASH_TYPE
    {
        HOPPING = 1,
        SKILL,
        RIDING
    };

    public enum LEVEL
    {
        LV1 = 1,
        LV2,
        LV3
    }
    public LEVEL Level = LEVEL.LV1;

    [SerializeField]private bool is_landed = false; // 착지했는가.
    public bool Is_landed    { get { return is_landed; } }

    [SerializeField] private bool is_grounded= false; // 착지했는가2.

    public bool is_collided;

    private bool is_fried = false; // 산불과 닿았는가.
    public bool Is_fried { get { return is_fried; } }

    [SerializeField] private bool is_key_released = false; // 버튼이 떨어졌는가.

    private bool gameOver = false;
    public bool GameOver { get { return gameOver; } }

    public bool Is_key_released { get { return is_key_released; } }

    [SerializeField]private bool is_dashing = false; // 돌진을 사용하는 중인가.
    public bool Is_dashing    { get { return is_dashing; } }

    private bool is_crushing = false; // 오브젝트를 부쉈는가
    public bool Is_crushing
    {
        get { return is_crushing; }
        set { is_crushing = value; }
    }

    public bool IsMounting;  // 탑승물을 탑승한 상태인가

    private bool levelUp = false;
    public bool LevelUp {
        get { return levelUp; }
        set { levelUp = value; }
    }

    private bool is_hopping = false; // 장애물을 밟았는가
    public bool Is_hopping { get { return is_hopping; } }

    private bool is_perfect = false; // 공중제비를 모두 돌았는가
    public bool Is_perfect
    {
        get { return is_perfect; }
        set { is_perfect = value; }
    }
    private bool is_tumbling = false;// 공중제비를 도는 중인가
    private bool setState = true;

    public float current_speed = 0.0f; // 현재 속도.
    public LevelControl level_control = null; // LevelControl이 저장됨.

    private int click_count = 0;
    public float click_timer = 1.0f; // 버튼이 눌린 후의 시간
    private float CLICK_GRACE_TIME = 0.5f; // 점프하고 싶은 의사를 받아들일 시간
    private int key = 0;    // 획득한 열쇠 아이템의 수
    public int Key {
        get { return key; }
        set { key = value; }
    }
    private int skill = 0;  // 쓸 수 있는 돌진 횟수
    public int Skill {
        get { return skill; }
        set { skill = value; }
    }

    private int score = 0;
    public int Score {
        get { return score; }
        set { score = value; }
    }
    private float meter = 0;
    public float Meter { get { return meter; } }
    private Vector3 startPos;
    private Vector3 velocity;
    private Rigidbody rb;

    public new AudioSource audio;
    public AudioClip[] audioClips;

    public Animator anim;
#endregion

    private void Awake()
    {
        audioClips = new AudioClip[6];

        // 동물 타입 지정
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (name == "Dog(Clone)")
                animalType = ANIMAL_TYPE.DOG;
            else if (name == "Cat(Clone)")
                animalType = ANIMAL_TYPE.CAT;
            else if (name == "Chicken(Clone)")
                animalType = ANIMAL_TYPE.CHICKEN;
            else
                animalType = ANIMAL_TYPE.DOG;
        }

        // 탑승물을 지정 여부
        // 탑승물을 얻지 못하였거나 선택하지 않았을 경우
        if (PlayerPrefs.GetInt("Bed") == 0 || PlayerPrefs.GetString("Mount") == "None")
        {
            IsMounting = false;
            return;
        }

        //탑승물을 선택한 경우
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            IsMounting = true;
            this.transform.localPosition = new Vector3(0f, 1.5f, 0f);
        }
    }

    void Start()
    {
        // 타이틀 화면에서 지정한 동물 확인
        if (PlayerPrefs.GetString("Animal") == null)
            PlayerPrefs.SetString("Animal", "Dog");
        string name = PlayerPrefs.GetString("Animal") + "(Clone)";

        if (this.name != name)
            this.gameObject.SetActive(false);

        Time.timeScale = 1.0f;

        gameOver = false;
        audio = GetComponent<AudioSource>();
        startPos = transform.position;
        this.next_step = STEP.RUN;
        levelUp = false;
        this.current_speed = 7f;

        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("Distance", 0);
        PlayerPrefs.SetInt("TotalScore", 0);

        rb = this.GetComponent<Rigidbody>();
        //if (IsMounting)
        //    rb.useGravity = false;
        anim = this.GetComponentInChildren<Animator>();

        // 애니메이션 세팅(메인이어야 달리는 애니메이션)
        if(SceneManager.GetActiveScene().buildIndex == 0)
           anim.SetBool("IsPlaying", false);
        else
            anim.SetBool("IsPlaying", true);
        
        // 스킬 없으면 이펙트 끄기
        if (skill < 1)
            GameObject.Find("UI").transform.Find("Dash_PS").gameObject.SetActive(false);
    }

    void Update()
    {
        // 탑승물을 타고 있다면
        if (IsMounting)
        {
            this.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            if (this.transform.name == "Chicken(Clone)")
            {
                this.transform.localPosition = new Vector3(1.5f, 3.1f, 0f);
                this.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            }
            anim.SetBool("IsPlaying", false);   // IDLE 상태로 유지
            return;   // Mount에서 제어
        }
        else
        {
            if (this.transform.name == "Chicken(Clone)")
            {
                this.transform.Find("Model").localPosition = new Vector3(2f, 2f, 0f);
                //this.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            }
            anim.SetBool("IsPlaying", true);   // IDLE 상태로 유지
        }

        velocity = this.GetComponent<Rigidbody>().velocity; // 속도를 설정.
        this.CheckLanded(); // 착지 상태인지 체크.
        this.CheckDistacne(); // 플레이어의 이동 거리를 계산한다.

        // 산불과 충돌했다면
        if (is_fried)
        {
            this.next_step = STEP.MISS; // '실패' 상태로 한다.

            // 결과화면 띄우기
            PlayerPrefs.SetInt("Score", score);
            PlayerPrefs.SetInt("Distance", (int)meter);
            int totalScore = score + (int)meter;
            PlayerPrefs.SetInt("TotalScore", totalScore);

            SceneManager.LoadScene("ResultScene");
        }

        // 스킬 없으면 이펙트 끄기
        if (skill < 1)
            GameObject.Find("UI").transform.Find("Dash_PS").gameObject.SetActive(false);

        if ((int)meter == 500 && LevelControl.GetInstance().isChanged == false && levelUp == false)
        {
            levelUp = true;
            Level = LEVEL.LV2;
            FindObjectOfType<FireController>().level = FireController.LEVEL.LEVEL2;
        }
        else if((int)meter == 1200 && LevelControl.GetInstance().isChanged == false && levelUp == false)
        {
            levelUp = true;
            Level = LEVEL.LV3;
            FindObjectOfType<FireController>().level = FireController.LEVEL.LEVEL3;
        }
        else
        {
            levelUp = false;
        }

        // 마우스를 떼면 텀블링을 카운트하지 않음
        if (Input.GetMouseButtonUp(0))
        {
            StopCoroutine("TumblingTimer");
            is_perfect = false;
        }

        // 텀블링
        if (Input.GetMouseButton(0) && !is_collided)
        {
            if (is_grounded==false)
            {
                anim.SetBool("IsJumping", true);
                transform.Rotate(new Vector3(0, 0, Time.deltaTime*100f));
            }
        }
        // 앞으로 쏠림
        else if(is_grounded == false)
        {
            transform.Rotate(new Vector3(0, 0, -Time.deltaTime * 20f));
        }

        // 정상적으로 서있음
        if (is_landed && !is_collided)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
            anim.SetBool("IsJumping", false);
        }

        // 넘어짐
        if (is_collided && !setState)
        {
            // 넘어지면 cry 효과음
            audio.clip = audioClips[2];
            audio.volume = 0.8f;
            audio.Play();

            // 넘어진 것을 보이기 위해 회전
            Debug.Log("FALL DOWN !!! ");
            this.transform.rotation = Quaternion.Euler(0, 0, -90);
            setState = true;
        }

        // 넘어진 상태면 클릭해서 일어나야 함
        if (is_collided && Input.GetMouseButtonDown(0))
        {
            // 일으키기 = 일어나아아아아악!!!
            click_count++;

            if (click_count == 10)
            {
                // 넘어짐 상태 해제
                is_collided = false;
                is_tumbling = false;
                setState = true;
                click_count = 0;

                // 넘어진 오브젝트 일으키기
                transform.Rotate(0, 0, 0, Space.Self);

                // 일어나면 돌진
                UsingDash(DASH_TYPE.HOPPING);
            }

            return;
        }

        this.step_timer += Time.deltaTime; // 경과 시간을 진행한다.

        // 타이머 세팅
        if (Input.GetMouseButtonDown(0) && is_landed == true)
        {
            // 텀블링 체크
            if (is_tumbling == false)
            {
                is_tumbling = true;
                StartCoroutine("TumblingTimer");
            }

            // UI를 제외한 화면 터치.
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                // 점프하면 jump 효과음
                if (!audio.isPlaying)
                {
                    audio.clip = audioClips[0];
                    audio.volume = 0.7f;
                    audio.Play();
                }
                this.click_timer = 0.0f; // 타이머를 리셋.
            }
        }
        else
        {
            if (this.click_timer >= 0.0f)
            {
                // 그렇지 않으면.
                this.click_timer += Time.deltaTime; // 경과 시간을 더한다.
            }
        }

        // 다음 상태가 정해져 있지 않으면 상태의 변화를 조사한다.
        if (this.next_step == STEP.NONE)
        {
            switch (this.step)
            {
                // Player의 현재 상태로 분기.
                case STEP.RUN: // 달리는 중일 때.
                    if (0.0f <= this.click_timer && this.click_timer <= CLICK_GRACE_TIME)
                    {
                        if (this.is_landed)
                        {
                            // 착지했다면.
                            anim.SetBool("IsJumping", false);
                            this.click_timer = -1.0f; // 버튼이 눌려있지 않음을 나타내는 -1.0f로.
                            this.next_step = STEP.JUMP; // 점프 상태로 한다.
                        }
                    }
                    break;
                case STEP.JUMP: // 점프 중일 때.
                    if (this.is_landed)
                    {
                        // 점프 중이고 착지했다면 다음 상태를 주행 중으로 변경.
                        anim.SetBool("IsJumping", false);
                        this.next_step = STEP.RUN;
                    }
                    break;
            }
        }

        // '다음 정보'가 '상태 정보 없음'이 아닌 동안(상태가 변할 때만).
        while (this.next_step != STEP.NONE)
        {
            this.step = this.next_step; // '현재 상태'를 '다음 상태'로 갱신.
            this.next_step = STEP.NONE; // '다음 상태'를 '상태 없음'으로 변경.
            switch (this.step)
            { 
                // 갱신된 '현재 상태'가.
                case STEP.JUMP: // '점프'일 때.
                    // 최고 도달점 높이(JUMP_HEIGHT_MAX)까지 점프할 수 있는 속도를 계산.
                    velocity.y = Mathf.Sqrt(2.0f * 9.8f * PlayerControl.JUMP_HEIGHT_MAX);
                    velocity.x = rb.velocity.x;
                    // '버튼이 떨어졌음을 나타내는 플래그'를 클리어한다.
                    this.is_key_released = false;
                    break;
            }

            // 상태가 변했으므로 경과 시간을 제로로 리셋.
            this.step_timer = 0.0f;
        }
        
        // 상태별로 매 프레임 갱신 처리.
        switch (this.step)
        {
            case STEP.RUN: // 달리는 중일 때.
                if (is_collided) // 넘어진 상태면 클릭해서 일어나야 함
                {
                    return;
                }
                // 속도를 높인다.
                velocity.x += PlayerControl.ACCELERATION * Time.deltaTime;

                // 계산으로 구한 속도가 설정해야 할 속도를 넘으면.
                if (Mathf.Abs(velocity.x) > SPEED_MAX && !is_dashing)
                {
                    // 넘지 않게 조정한다.
                    velocity.x *= this.current_speed / Mathf.Abs(velocity.x);
                }
                break;
            case STEP.JUMP: // 점프 중일 때.
                do
                {
                    // '버튼이 떨어진 순간'이 아니면.
                    if (!Input.GetMouseButtonUp(0))
                    {
                        break; // 아무것도 하지 않고 루프를 빠져나간다.
                    }
                    // 이미 감속된 상태면(두 번이상 감속하지 않도록).
                    if (this.is_key_released)
                    {
                        break; // 아무것도 하지 않고 루프를 빠져나간다.
                    }
                    // 상하방향 속도가 0 이하면(하강 중이라면).
                    if (velocity.y <= 0.0f)
                    {
                        break; // 아무것도 하지 않고 루프를 빠져나간다.
                    }
                    // 버튼이 떨어져 있고 상승 중이라면 감속 시작.
                    // 점프의 상승은 여기서 끝.
                    //velocity.y *= JUMP_KEY_RELEASE_REDUCE;
                    this.is_key_released = true;
                } while (false);
                velocity.x = rb.velocity.x;
                break;

            case STEP.MISS:
                // 가속도(ACCELERATION)를 빼서 Player의 속도를 느리게 해 간다.
                velocity.x -= PlayerControl.ACCELERATION * Time.deltaTime;
                if (velocity.x < 0.0f)
                { // Player의 속도가 마이너스면.
                    velocity.x = 0.0f; // 0으로 한다.
                }
                break;
        }
        // Rigidbody의 속도를 위에서 구한 속도로 갱신.
        // (이 행은 상태에 관계없이 매번 실행된다).
        if (velocity.x < SPEED_MIN)
        {
            velocity.x += PlayerControl.ACCELERATION * SPEED_MIN * Time.deltaTime;
        }
        this.GetComponent<Rigidbody>().velocity = velocity;
    }

    private void CheckLanded() // 착지했는지 조사
    {
        this.is_landed = false; // 일단 false로 설정.
        velocity.x = rb.velocity.x;
        do
        {
            Vector3 s = this.transform.position; // Player의 현재 위치.
            Vector3 e = s + Vector3.down * 1.0f; // s부터 아래로 1.0f로 이동한 위치.

            RaycastHit hit;
            // s부터 e 사이에 아무것도 없을 때. *out: method 내에서 생선된 값을 반환때 사용.
            if(!Physics.Linecast(s, e, out hit)) {
                break; // 아무것도 하지 않고 do~while 루프를 빠져나감(탈출구로).
            }

            // s부터 e 사이에 뭔가 있을 때 아래의 처리가 실행.
            if(this.step == STEP.JUMP) { // 현재, 점프 상태라면.
                if(this.step_timer < Time.deltaTime * 3.0f) { // 경과 시간이 3.0f 미만이라면.
                    break; // 아무것도 하지 않고 do~while 루프를 빠져나감(탈출구로).
                }
            }
            // s부터 e 사이에 뭔가 있고 JUMP 직후가 아닐 때만 아래가 실행.
            this.is_landed = true;
            anim.SetBool("IsJumping", false);

        } while (false);
        // 루프의 탈출구.
    }

    public bool IsPlayEnd() // 게임이 끝났는지 판정.
    {
        bool ret = false;
        switch (this.step)
        {
            case STEP.MISS: // MISS 상태라면.
                ret = true; // '죽었어요'(true)라고 알려줌.
                break;
        }
        return (ret);
    }

    // 돌진 스킬 사용(가속도 유지 시간)
    public void UsingDash(DASH_TYPE type)
    {
        switch (type)
        {
            case DASH_TYPE.HOPPING:
                break;
            case DASH_TYPE.SKILL:
                // 남은 돌진이 없으면 반환
                if (skill < 1)
                    return;
                skill--;
                break;
            case DASH_TYPE.RIDING:
                break;
            default:
                break;
        }

        StartCoroutine(Dash((int)type));
    }

    // 돌진 스킬 처리
    IEnumerator Dash(int sec)
    {
        // 돌진 시 run 효과음
        if (!audio.isPlaying)
        {
            audio.clip = audioClips[1];
            audio.volume = 1.0f;
            audio.Play();
        }

        // 대시 파티클
        ParticleSystem particle = this.transform.GetChild(2).GetComponent<ParticleSystem>();
        is_dashing = true;

        // 스킬 사용
        if (particle.isPlaying)
            particle.Stop();
        particle.Play();
        rb.AddForce(Vector3.right * Time.deltaTime * 700f * sec);
        yield return new WaitForSeconds(sec + 0.5f);
        particle.Stop();

        is_dashing = false;
        is_hopping = false;
    }

    // 트리거 감지 처리
    private void OnTriggerEnter(Collider other)
    {
        // 장애물을 스쳤으면 점수를 얻고 돌진효과
        if (other.CompareTag("Obstacle"))
        {
            if(is_dashing)
            {
                is_crushing = true;
                is_hopping = false;
                other.gameObject.SetActive(false);
                this.GetComponent<Rigidbody>().velocity = velocity;
                Debug.Log("CRUSH !!! (+200)");
                score += (int)GameRoot.SCORE_TYPE.DESTROY;  // 점수 획득
                return;
            }
            Debug.Log("HOPPING !!! (+400)");
            is_hopping = true;
            score += (int)GameRoot.SCORE_TYPE.HOP;
            UsingDash(DASH_TYPE.HOPPING);
        }

        if (other.CompareTag("Key"))
        {
            Debug.Log("GET KEY!!! (+50)");
            other.gameObject.SetActive(false);
            score += 50;

            // 획득 시 key 효과음
            audio.clip = audioClips[4];
            audio.volume = 1.0f;
            audio.Play();

            // 닭은 열쇠를 2개까지 소지 가능
            if (this.transform.GetChild(0).name == "Chicken" && key < 2)
            {
                key++;
                return;
            }
            // 그 외에는 열쇠는 1개만 소지 가능
            else if (key > 0)
                return;

            // 열쇠 획득
            key++;
        }

        if (other.gameObject.CompareTag("Box"))
        {
            // 상자를 열지 못하는 조건: 열쇠가 없거나 돌진이 꽉 찼을 때
            if (key == 0)
                return;
            else if (animalType == ANIMAL_TYPE.DOG && skill > 0)
                return;
            else if ((animalType == ANIMAL_TYPE.CAT
                || animalType == ANIMAL_TYPE.CHICKEN) && skill > 1)
                return;

            // 열쇠를 사용하여 상자 속 Dash를 얻음
            key--;

            // 획득 시 box 효과음
            audio.clip = audioClips[5];
            audio.volume = 1.0f;
            audio.Play();

            Debug.Log("BOX OPEN !!! (+100)");
            //StopCoroutine("ScoreRecord");
            other.gameObject.SetActive(false);  //상자가 열리는 애니메이션으로 대체 시 트리거로 옮길 것
            score += 100;
            skill++;
            GameObject.Find("UI").transform.Find("Dash_PS").gameObject.SetActive(true);
        }
    }

    // 충돌 처리
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // 돌진 상태로 충돌 시 장애물은 파괴되고 점수를 획득
            if (is_dashing)
            {
                is_crushing = true;
                is_hopping = false;
                collision.gameObject.SetActive(false);
                this.GetComponent<Rigidbody>().velocity = velocity;
                Debug.Log("CRUSH !!! (+200)");
                score += (int)GameRoot.SCORE_TYPE.DESTROY;  // 점수 획득
                return;
            }

            if (is_hopping)
                return;

            // 넘어짐!!
            is_collided = true;  // 충돌
            setState = false;
            collision.gameObject.SetActive(false);

            // falldown 효과음
            audio.clip = audioClips[3];
            audio.volume = 0.7f;
            audio.Play();

            AudioSource crush = transform.GetChild(1).GetComponent<AudioSource>();
            crush.Play();
        }

        if(collision.gameObject.CompareTag("Fire"))
        {
            Debug.Log("GAME OVER...");
            is_fried = true;

            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        if(collision.gameObject.CompareTag("Ground"))
        {
            is_grounded = true;
        }

        else
            return;
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            is_grounded = true;

            // 공중제비를 성공하고 착지
            if (is_perfect)
            {
                Debug.Log("NICE TUMBLING !!! (+50)");
                score += 50; // 50점 획득
                UsingDash(DASH_TYPE.HOPPING);
            }
            is_tumbling = false;
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // 돌진 상태로 충돌 시 장애물은 파괴되고 점수를 획득
            if (is_dashing)
            {
                is_crushing = true;
                is_hopping = false;
                collision.gameObject.SetActive(false);
                this.GetComponent<Rigidbody>().velocity = velocity;
                Debug.Log("CRUSH !!! (+200)");
                score += (int)GameRoot.SCORE_TYPE.DESTROY;  // 점수 획득
                return;
            }

            if (is_hopping)
                return;

            // 넘어짐!!
            is_collided = true;  // 충돌
            setState = false;
            collision.gameObject.SetActive(false);

            // falldown 효과음
            audio.clip = audioClips[3];
            audio.volume = 0.7f;
            audio.Play();

            AudioSource crush = transform.GetChild(1).GetComponent<AudioSource>();
            crush.Play();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            is_grounded = false;
        }
    }

    private void CheckDistacne()
    {
        meter = Vector3.Distance(startPos, this.transform.position);
    }

    //IEnumerator ScoreRecord(string text)
    //{

    //    yield return new WaitForSeconds(2f);
    //}

    IEnumerator TumblingTimer()
    {
        float timer = 0;
        while (timer < 2.7)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            timer += 0.1f;
        }

        is_perfect = true;
        is_tumbling = false;
    }
}
