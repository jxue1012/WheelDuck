using System.Collections;
using UnityEngine;
using DG.Tweening;

public class FloorControl : MonoBehaviour
{

    private float DefaultSpeed;
    private float MaxSpeed;
    private float speedInc;
    public float Speed { get; private set; }
    public Vector2 Dir;
    public float Duration = 3f;
    public float CheckScoreDuration = 2f;

    public float radius = 1;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, radius);
    }

    public void Init()
    {
        Speed = 0;
    }

    private void Update()
    {
 
        //if (foodUpdateOn)
        //{
        //    foodTimer += Time.deltaTime;
        //    if (foodTimer >= foodDuration)
        //    {
        //        float newSpeed = Speed + speedInc;
        //        SetFloorSpeed(newSpeed);
        //        ShowFood();
        //        foodTimer = 0;
        //    }
        //}

    }

    public void StartGame()
    {
        var data = GameCenter.Instance.gameData;
        DefaultSpeed = data.MinFloorSpeed;
        MaxSpeed = data.MaxFloorSpeed;
        speedInc = data.FloorSpeedInc;
        Speed = 0;
        StopDrum();
        HideArrow();
        HideLight();

        //foodUpdateOn = false;
        //foodTimer = 0;

    }

    public void ReadyToChallenge(bool isInfinite)
    {
        StartDrum();
        StartCoroutine(RandomDirCo());
        StartCoroutine(CheckScoreCo());
        ResetSpeed();
        ShowArrow();

        //foodUpdateOn = true;
        //foodTimer = 0;
        ShowFood();
    }

    public void GameOver()
    {
        StopAllCoroutines();
        HideArrow();
        HideLight();
        //foodUpdateOn = false;
        //foodTimer = 0;
    }

    public void ChangeSpeed(float value)
    {
        Speed = value;
    }

    public void ResetSpeed()
    {
        Speed = DefaultSpeed;
    }

    IEnumerator RandomDirCo()
    {
        while (true)
        {
            Dir = Random.insideUnitCircle.normalized;
            GameCenter.Instance.SetWorldDir(Dir);
            yield return new WaitForSeconds(Duration);
        }
    }

    IEnumerator CheckScoreCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(CheckScoreDuration);
            CheckScore();
        }
    }

    public void CheckScore()
    {
        var player = GameCenter.Instance.player;
        var playerPos = player.transform.position;

        Ray ray = new Ray(playerPos + new Vector3(0, 0, -0.5f), Vector3.forward);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100f, GameCenter.Instance.FloorLM);
        if (hit.collider != null)
        {
            var instance = hit.collider.gameObject.GetComponent<FloorInstance>();
            instance.AddScore();
        }
    }

    //Floor 
    [Header("Floor")]
    public GameObject LightIn;
    public GameObject LightMid, LightOut;

    public void ShowLight(int index)
    {
        LightIn.SetActive(index == 2);
        LightMid.SetActive(index == 1);
        LightOut.SetActive(index == 0);

        Invoke("HideLight", 0.5f);
    }

    private void HideLight()
    {
        LightIn.SetActive(false);
        LightMid.SetActive(false);
        LightOut.SetActive(false);
    }

    public void SetFloorSpeed(float value)
    {
        Speed = Mathf.Clamp(value, 0, GameCenter.Instance.gameData.MaxFloorSpeed);
    }
    public void IncreaseFloorSpeed(float increment)
    {
        Speed += increment;
        //Speed = Mathf.Clamp(Speed, 0, MaxSpeed);
    }

    //Drum 
    [Header("Drum")]
    public GameObject DrumContainer;
    public float DrumRotateDuration = 10f;
    public void StartDrum()
    {
        DrumContainer.transform.DORotate(new Vector3(0, 0, 360f), DrumRotateDuration, RotateMode.FastBeyond360)
                   .SetLoops(-1, LoopType.Restart);
    }

    public void StopDrum()
    {
        DrumContainer.transform.DOKill();
        DrumContainer.transform.rotation = Quaternion.identity;
    }

    //Arrow
    public GameObject Arrow;

    public void ShowArrow()
    {
        Arrow.SetActive(true);
    }

    public void HideArrow()
    {
        Arrow.SetActive(false);
    }

    // 在Start或者Update等适当的时机调用这个方法，传入目标方向向量
    public void SetArrowDir(Vector2 targetDirection)
    {
        // 计算当前朝向向量（默认朝上）
        Vector2 currentDirection = Vector2.up;

        // 计算旋转角度
        float angle = Vector2.SignedAngle(currentDirection, targetDirection);

        // 将箭头进行旋转，使其指向目标方向
        Arrow.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    #region -------------- Food -----------------

    public FoodControl FoodInstance;
    public float foodMinRadius, foodMaxRadius;
    //float foodTimer;
    //float foodDuration = 10f;
    //bool foodUpdateOn;

    public void ShowFood()
    {
        bool success = false;
        Vector2 rand = Vector2.zero;
        while (!success)
        {

            rand = RandomPointInRadius(foodMinRadius, foodMaxRadius);
            float dst = Vector2.Distance(rand, GameCenter.Instance.player.transform.position);
            if (dst > 1)
                success = true;
        }
        FoodInstance.Show(rand);
    }

    public void HideFood()
    {
        FoodInstance.Hide();
    }

    Vector2 RandomPointInRadius(float minRadius, float maxRadius)
    {
        // 随机生成半径
        float radius = Random.Range(minRadius, maxRadius);

        // 随机生成角度
        float angle = Random.Range(0f, Mathf.PI * 2f);

        // 构建 Vector2 点位
        Vector2 randomPoint = new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));

        return randomPoint;
    }


    #endregion


}
