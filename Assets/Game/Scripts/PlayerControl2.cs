using UnityEngine;

public class PlayerControl2 : MonoBehaviour
{
    private Rigidbody2D rb;

    public void Init()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    Vector3 velocity, desiredVelocity;
    Vector2 moveDir;
    private float minSpeed = 1f;
    private float maxSpeed = 5f;
    private float playerMoveSpeed;
    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 2f;

    private Vector2 worldDir;
    private float rotationSpeed = 180f; // 旋转速度，角度每秒

    private float MaxBtnTime = 2f;
    public float BtnChangeSpeed = 5f;
    private int leftBtnIndex = 1;
    private float leftBtnTime;

    private int rightBtnIndex = 1;
    private float rightBtnTime;

    private float leftValue, rightValue;

    public bool CanMove;

    public void StartGame()
    {
        var data = GameCenter.Instance.gameData;
        minSpeed = data.MinPlayerSpeed;
        maxSpeed = data.MaxPlayerSpeed;
        maxAcceleration = data.MaxAcceleration;
        rotationSpeed = data.PlayerRotationSpeed;
        BtnChangeSpeed = data.BtnChangeSpeed;

        this.transform.position = Vector2.zero;
        this.gameObject.SetActive(true);
        CanMove = true;
        animator.CrossFade("Idle", 0);
    }

    public void ReadyToChallenge()
    {
        CanMove = true;
        this.transform.position = Vector2.zero;
    }

    public void GameOver()
    {
        CanMove = false;
        rb.velocity = Vector2.zero;
        animator.CrossFade("Idle", 0);
    }

    public void AddForce(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public void CheckDie()
    {
        float dist = Vector2.Distance(Vector2.zero, this.transform.position);
        if (dist > GameCenter.Instance.floorControl.radius)
        {
            CanMove = false;
            this.gameObject.SetActive(false);
            GameCenter.Instance.GameOver();
        }
    }

    void Update()
    {
        if (CanMove == false) return;

        leftValue = UpdateLeftBtn();
        rightValue = UpdateRightBtn();

        int btnStatus = leftBtnIndex + rightBtnIndex;

        if (btnStatus > 19)
        {
            // 角色向面朝方向移动
            moveDir = transform.up.normalized;
            PlayAnim(forwardAnim);
        }
        else if (btnStatus < -19)
        {
            // 角色朝面朝的反方向后退
            moveDir = -transform.up.normalized;
            PlayAnim(backwardAnim);
        }
        else if (btnStatus == 0)
        {
            if (leftBtnIndex < -9 && rightBtnIndex > 9)
            {
                //逆时针旋转
                float rotationDirection = 1f;
                rb.rotation += rotationDirection * rotationSpeed * Time.deltaTime;
            }
            else if (leftBtnIndex > 9 && rightBtnIndex < -9)
            {
                //顺时针旋转
                float rotationDirection = -1f;
                rb.rotation += rotationDirection * rotationSpeed * Time.deltaTime;
            }

            PlayAnim(idleAnim);
        }
        else
        {
            PlayAnim(idleAnim);
            moveDir = Vector2.zero;
        }

        float value = leftValue + rightValue;
        playerMoveSpeed = MapValue(value);

        desiredVelocity =
            moveDir * playerMoveSpeed + worldDir * GameCenter.Instance.floorControl.Speed;

        CheckDie();
    }

    void FixedUpdate()
    {
        if (CanMove == false) return;

        velocity = rb.velocity;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x =
            Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.y =
            Mathf.MoveTowards(velocity.y, desiredVelocity.y, maxSpeedChange);
        rb.velocity = velocity;
    }

    public void SetWorldDir(Vector2 dir)
    {
        worldDir = dir;
    }

    private float UpdateLeftBtn()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (leftBtnIndex != 10)
            {
                leftBtnTime = 0;
            }

            leftBtnIndex = 10;
        }

        if (Input.GetKey(KeyCode.W))
        {
            float time = leftBtnTime + Time.deltaTime * BtnChangeSpeed;
            time = Mathf.Clamp(time, 0, MaxBtnTime);
            leftBtnTime = time;
            leftBtnIndex = 10;
        }
        else
        {
            if (leftBtnIndex >= 1)
            {
                float time = leftBtnTime - Time.deltaTime * BtnChangeSpeed;
                time = Mathf.Clamp(time, 0, MaxBtnTime);
                leftBtnTime = time;
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (leftBtnIndex != -10)
            {
                leftBtnTime = 0;
            }

            leftBtnIndex = -10;
        }

        if (Input.GetKey(KeyCode.S))
        {
            float time = leftBtnTime + Time.deltaTime * BtnChangeSpeed;
            time = Mathf.Clamp(time, 0, MaxBtnTime);
            leftBtnTime = time;
            leftBtnIndex = -10;
        }
        else
        {
            if (leftBtnIndex <= 1)
            {
                float time = leftBtnTime - Time.deltaTime * BtnChangeSpeed;
                time = Mathf.Clamp(time, 0, MaxBtnTime);
                leftBtnTime = time;
            }
        }

        float amount = leftBtnTime / MaxBtnTime;
        GameCenter.Instance.uIManager.SetLeftInputFill(amount, leftBtnIndex);
        if (amount == 0)
            leftBtnIndex = 1;

        return amount;
    }

    private float UpdateRightBtn()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (rightBtnIndex != 10)
            {
                rightBtnTime = 0;
            }

            rightBtnIndex = 10;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            float time = rightBtnTime + Time.deltaTime * BtnChangeSpeed;
            time = Mathf.Clamp(time, 0, MaxBtnTime);
            rightBtnTime = time;
        }
        else
        {
            if (rightBtnIndex >= 1)
            {
                float time = rightBtnTime - Time.deltaTime * BtnChangeSpeed;
                time = Mathf.Clamp(time, 0, MaxBtnTime);
                rightBtnTime = time;
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (rightBtnIndex != -10)
            {
                rightBtnTime = 0;
            }

            rightBtnIndex = -10;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            float time = rightBtnTime + Time.deltaTime * BtnChangeSpeed;
            time = Mathf.Clamp(time, 0, MaxBtnTime);
            rightBtnTime = time;
        }
        else
        {
            if (rightBtnIndex <= 1)
            {
                float time = rightBtnTime - Time.deltaTime * BtnChangeSpeed;
                time = Mathf.Clamp(time, 0, MaxBtnTime);
                rightBtnTime = time;
            }
        }

        float amount = rightBtnTime / MaxBtnTime;
        GameCenter.Instance.uIManager.SetRightInputFill(amount, rightBtnIndex);
        if (amount == 0)
            rightBtnIndex = 1;

        return amount;
    }

    float MapValue(float x)
    {
        float xMin = 0f; // 输入值的最小值
        float xMax = 2f; // 输入值的最大值
        float yMin = minSpeed; // 输出值的最小值
        float yMax = maxSpeed; // 输出值的最大值

        // 使用线性映射公式计算映射后的值
        float y = (x - xMin) * (yMax - yMin) / (xMax - xMin) + yMin;

        return y;
    }


    #region ----------- Anim ------------------

    [Header("Anim")]
    public Animator animator;

    public string idleAnim = "Idle";
    public string forwardAnim = "Forward";
    public string backwardAnim = "Backward";

    public void PlayAnim(string anim)
    {
        animator.CrossFade(anim, 0);
    }

    #endregion

}
