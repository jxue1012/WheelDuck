using UnityEngine;

public class FloorInstance : MonoBehaviour
{
    public int Index;

    public void AddScore()
    {
        int Score = GameCenter.Instance.gameData.GetFloorScore(Index);
        GameCenter.Instance.AddScore(Score);
        GameCenter.Instance.floorControl.ShowLight(Index);

    }


}
