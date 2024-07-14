using UnityEngine;


[CreateAssetMenu(fileName = "GameData", menuName = "MySO/GameData", order = 0)]
public class GameData : ScriptableObject
{
    [Header("分数")]
    public int DrumScore = 35;
    public int FoodScore = 100;
    public int MaxScore = 50;
    public int MiddleScore = 15;
    public int MinScore = 5;

    public int GetFloorScore(int index)
    {
        switch (index)
        {
            case 0:
                return MinScore;
            case 1:
                return MiddleScore;
            case 2:
                return MaxScore;
            default:
                return 0;
        }
    }


    [Header("地板速度")]
    public float MinFloorSpeed = 0.5f;
    public float MiddleFloorSpeed = 1f;
    public float MaxFloorSpeed = 1.5f;
    public float FloorSpeedInc = 0.25f;
    public float FloorInfiniteSpeedInc = 0.1f;
    public float DrumForce = 5f;
    

    [Header("角色速度")]
    public float MinPlayerSpeed = 1f;
    public float MaxPlayerSpeed = 3f;

    [Header("角色加速度")]
    public float MaxAcceleration = 2f;

    [Header("角色旋转速度")]
    public float PlayerRotationSpeed = 180f;

    [Header("按钮最大时间")]
    public float BtnChangeSpeed = 2f;

}

