using UnityEngine;
using UnityEngine.UI;
using ChocDino.PartyIO;

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
    private float rotationSpeed = 90f; // 旋转速度，角度每秒

    private float MaxBtnTime = 2f;
    public float BtnChangeSpeed = 5f;
    private int leftBtnIndex = 1;
    private float leftBtnTime;

    private int rightBtnIndex = 1;
    private float rightBtnTime;

    private float leftValue, rightValue;

    public bool CanMove;
    public GameObject dieAnimationPrefab;

    //Bar显示当前按键
    public Image UIImage1;
    public Image UIImage2;
    public Image UIImage3;
    public Image UIImage4;

    //player显示当前按键
    public Image l1;
    public Image l2;
    public Image l3;
    public Image l4;

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
            SpawnDieAnimation();
            GameCenter.Instance.GameOver();
        }
    }

    private void SpawnDieAnimation()
    {
        if (dieAnimationPrefab != null)
        {
            Instantiate(dieAnimationPrefab, this.transform.position, Quaternion.identity);
        }
    }

    void Update()
    {
        if (CanMove == false) return;

        leftValue = UpdateLeftBtn();
        rightValue = UpdateRightBtn();

        int btnStatus = leftBtnIndex + rightBtnIndex;

        if (!(leftBtnIndex == 10 && rightBtnIndex == 10) && !(leftBtnIndex == -10 && rightBtnIndex == -10))
        {
            if (leftBtnIndex == 10)
                rb.rotation += -1f * rotationSpeed * Time.deltaTime;
            else if (leftBtnIndex == -10)
                rb.rotation += 1f * rotationSpeed * Time.deltaTime;
            else if (rightBtnIndex == 10)
                rb.rotation += 1f * rotationSpeed * Time.deltaTime;
            else if (rightBtnIndex == -10)
                rb.rotation += -1f * rotationSpeed * Time.deltaTime;
        }

        var ui = GameCenter.Instance.uIManager;

        if (btnStatus > 19)
        {
            moveDir = transform.up.normalized;
            PlayAnim(forwardAnim);
            ui.SetTutorialCheckMark(0, true);
        }
        else if (btnStatus < -19)
        {
            moveDir = -transform.up.normalized;
            PlayAnim(backwardAnim);
            ui.SetTutorialCheckMark(1, true);
        }
        else if (btnStatus == 0)
        {
            if (leftBtnIndex < -9 && rightBtnIndex > 9)
            {
                rb.rotation += 1f * rotationSpeed * Time.deltaTime;
                PlayAnim(leftAnim);
                ui.SetTutorialCheckMark(2, true);
            }
            else if (leftBtnIndex > 9 && rightBtnIndex < -9)
            {
                rb.rotation += -1f * rotationSpeed * Time.deltaTime;
                PlayAnim(rightAnim);
                ui.SetTutorialCheckMark(3, true);
            }
        }
        else
        {
            PlayAnim(idleAnim);
            moveDir = Vector2.zero;
        }

        float value = leftValue + rightValue;
        playerMoveSpeed = MapValue(value);
        desiredVelocity = moveDir * playerMoveSpeed + worldDir * GameCenter.Instance.floorControl.Speed;

        CheckDie();
    }

    void FixedUpdate()
    {
        if (CanMove == false) return;

        velocity = rb.velocity;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.y = Mathf.MoveTowards(velocity.y, desiredVelocity.y, maxSpeedChange);
        rb.velocity = velocity;
    }

    public void SetWorldDir(Vector2 dir) => worldDir = dir;

    private float GetMouseYDirection(int mouseId)
    {
        var mouseList = MouseManager.Instance.All;
        if (mouseId >= mouseList.Count) return 0f;
        var mouse = mouseList[mouseId];
        if (mouse.ConnectionState != MouseConnectionState.Connected) return 0f;
        return mouse.PositionDelta.y;
    }

    private float UpdateLeftBtn()
    {
        float leftStickY = Input.GetAxis("LeftStickVertical");
        float mouse1Y = GetMouseYDirection(0);

        if (Input.GetKeyDown(KeyCode.W) || leftStickY < 0 || mouse1Y > 1f)
        {
            if (leftBtnIndex != 10) leftBtnTime = 0;
            leftBtnIndex = 10;
        }

        if (Input.GetKey(KeyCode.W) || leftStickY < 0 || mouse1Y > 1f)
        {
            ShowImage(UIImage1); ShowImage(l1);
            leftBtnTime = Mathf.Clamp(leftBtnTime + Time.deltaTime * BtnChangeSpeed, 0, MaxBtnTime);
            leftBtnIndex = 10;
        }
        else
        {
            HideImage(UIImage1); HideImage(l1);
            if (leftBtnIndex >= 1)
                leftBtnTime = Mathf.Clamp(leftBtnTime - Time.deltaTime * BtnChangeSpeed, 0, MaxBtnTime);
        }

        if (Input.GetKeyDown(KeyCode.S) || leftStickY > 0 || mouse1Y < -1f)
        {
            if (leftBtnIndex != -10) leftBtnTime = 0;
            leftBtnIndex = -10;
        }

        if (Input.GetKey(KeyCode.S) || leftStickY > 0 || mouse1Y < -1f)
        {
            ShowImage(UIImage2); ShowImage(l2);
            leftBtnTime = Mathf.Clamp(leftBtnTime + Time.deltaTime * BtnChangeSpeed, 0, MaxBtnTime);
            leftBtnIndex = -10;
        }
        else
        {
            HideImage(UIImage2); HideImage(l2);
            if (leftBtnIndex <= 1)
                leftBtnTime = Mathf.Clamp(leftBtnTime - Time.deltaTime * BtnChangeSpeed, 0, MaxBtnTime);
        }

        float amount = leftBtnTime / MaxBtnTime;
        GameCenter.Instance.uIManager.SetLeftInputFill(amount, leftBtnIndex);
        if (amount == 0) leftBtnIndex = 1;
        return amount;
    }

    private float UpdateRightBtn()
    {
        float rightStickY = Input.GetAxis("RightStickVertical");
        float mouse2Y = GetMouseYDirection(1);

        if (Input.GetKey(KeyCode.I) || rightStickY > 0 || mouse2Y > 1f)
        {
            if (rightBtnIndex != 10) rightBtnTime = 0;
            rightBtnIndex = 10;
        }

        if (Input.GetKey(KeyCode.I) || rightStickY > 0 || mouse2Y > 1f)
        {
            ShowImage(UIImage3); ShowImage(l3);
            rightBtnTime = Mathf.Clamp(rightBtnTime + Time.deltaTime * BtnChangeSpeed, 0, MaxBtnTime);
        }
        else
        {
            HideImage(UIImage3); HideImage(l3);
            if (rightBtnIndex >= 1)
                rightBtnTime = Mathf.Clamp(rightBtnTime - Time.deltaTime * BtnChangeSpeed, 0, MaxBtnTime);
        }

        if (Input.GetKey(KeyCode.K) || rightStickY < 0 || mouse2Y < -1f)
        {
            if (rightBtnIndex != -10) rightBtnTime = 0;
            rightBtnIndex = -10;
        }

        if (Input.GetKey(KeyCode.K) || rightStickY < 0 || mouse2Y < -1f)
        {
            ShowImage(UIImage4); ShowImage(l4);
            rightBtnTime = Mathf.Clamp(rightBtnTime + Time.deltaTime * BtnChangeSpeed, 0, MaxBtnTime);
        }
        else
        {
            HideImage(UIImage4); HideImage(l4);
            if (rightBtnIndex <= 1)
                rightBtnTime = Mathf.Clamp(rightBtnTime - Time.deltaTime * BtnChangeSpeed, 0, MaxBtnTime);
        }

        float amount = rightBtnTime / MaxBtnTime;
        GameCenter.Instance.uIManager.SetRightInputFill(amount, rightBtnIndex);
        if (amount == 0) rightBtnIndex = 1;
        return amount;
    }

    float MapValue(float x)
    {
        float xMin = 0f, xMax = 2f, yMin = minSpeed, yMax = maxSpeed;
        return (x - xMin) * (yMax - yMin) / (xMax - xMin) + yMin;
    }

    void ShowImage(Image image) => image?.gameObject.SetActive(true);
    void HideImage(Image image) => image?.gameObject.SetActive(false);
    #region ----------- Anim ------------------

    [Header("Anim")]
    public Animator animator;

    public string idleAnim = "Idle";
    public string forwardAnim = "Forward";
    public string backwardAnim = "Backward";
    public string leftAnim = "Left";
    public string rightAnim = "Right";
    public string die = "Die";
    public string lefthandAnim = "LeftHand";
    public string righthandAnim = "RightHand";

    public void PlayAnim(string anim)
    {
        animator.CrossFade(anim, 0);
    }

    #endregion

}