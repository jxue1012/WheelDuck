using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D rb;

    public void Init()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    Vector3 velocity, desiredVelocity;
    Vector2 moveDir;
    public float maxSpeed = 5f;
    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 2f;

    private Vector2 worldDir;
    public float rotationSpeed = 180f; // 旋转速度，角度每秒

    void Update()
    {

        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.UpArrow))
        {
            // 角色向面朝方向移动
            moveDir = transform.up.normalized;
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.DownArrow))
        {
            // 角色朝面朝的反方向后退
            moveDir = -transform.up.normalized;
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.UpArrow))
        {
            //逆时针旋转
            float rotationDirection = 1f;
            rb.rotation += rotationDirection * rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.DownArrow))
        {
            //顺时针旋转
            float rotationDirection = -1f;
            rb.rotation += rotationDirection * rotationSpeed * Time.deltaTime;
        }
        else
        {
            moveDir = Vector2.zero;
        }


        desiredVelocity =
            moveDir * maxSpeed + worldDir * GameCenter.Instance.floorControl.Speed;
    }

    void FixedUpdate()
    {
        velocity = rb.velocity;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x =
            Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.y =
            Mathf.MoveTowards(velocity.y, desiredVelocity.y, maxSpeedChange);
        rb.velocity = velocity;
    }


}
