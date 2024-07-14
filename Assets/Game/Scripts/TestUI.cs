using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TestUI : MonoBehaviour
{

    public void Init()
    {
        InitFloorSpeedControl();
        InitPlayerSpeedControl();
        InitScoreControl();
    }

    public void Switch()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void StopWorldMove()
    {
        GameCenter.Instance.floorControl.ChangeSpeed(0);
        GameCenter.Instance.floorControl.HideArrow();
    }

    public void StartWorldMove()
    {
        GameCenter.Instance.floorControl.ResetSpeed();
        GameCenter.Instance.floorControl.ShowArrow();
    }

    public void ClearRank()
    {
        GameCenter.Instance.playerDatabase.ClearData();
        GameCenter.Instance.uIManager.UpdateRankPage();
    }


    //Floor Speed
    public TMP_InputField FloorSpeedMin, FloorSpeedMid, FloorSpeedMax;

    private void InitFloorSpeedControl()
    {
        var data = GameCenter.Instance.gameData;

        FloorSpeedMin.text = data.MinFloorSpeed.ToString();
        FloorSpeedMin.onValueChanged.AddListener((value) =>
        {
            GameCenter.Instance.gameData.MinFloorSpeed = float.Parse(value);
        });

        FloorSpeedMid.text = data.MiddleFloorSpeed.ToString();
        FloorSpeedMid.onValueChanged.AddListener((value) =>
        {
            GameCenter.Instance.gameData.MiddleFloorSpeed = float.Parse(value);
        });

        FloorSpeedMax.text = data.MaxFloorSpeed.ToString();
        FloorSpeedMax.onValueChanged.AddListener((value) =>
        {
            GameCenter.Instance.gameData.MaxFloorSpeed = float.Parse(value);
        });
    }

    //Player Speed
    public TMP_InputField PlayerSpeedMin, PlayerSpeedMax, PlayerAcceleration, PlayerRotationSpeed;

    private void InitPlayerSpeedControl()
    {
        var data = GameCenter.Instance.gameData;

        PlayerSpeedMin.text = data.MinPlayerSpeed.ToString();
        PlayerSpeedMin.onValueChanged.AddListener((value) =>
        {
            GameCenter.Instance.gameData.MinPlayerSpeed = float.Parse(value);
        });

        PlayerSpeedMax.text = data.MaxPlayerSpeed.ToString();
        PlayerSpeedMax.onValueChanged.AddListener((value) =>
        {
            GameCenter.Instance.gameData.MaxPlayerSpeed = float.Parse(value);
        });

        PlayerAcceleration.text = data.MaxAcceleration.ToString();
        PlayerAcceleration.onValueChanged.AddListener((value) =>
        {
            GameCenter.Instance.gameData.MaxAcceleration = float.Parse(value);
        });

        PlayerRotationSpeed.text = data.PlayerRotationSpeed.ToString();
        PlayerRotationSpeed.onValueChanged.AddListener((value) =>
        {
            GameCenter.Instance.gameData.PlayerRotationSpeed = float.Parse(value);
        });
    }


    //Score Control
    public TMP_InputField DrumScore, MaxScore, MiddleScore, MinScore;

    private void InitScoreControl()
    {
        var data = GameCenter.Instance.gameData;

        DrumScore.text = data.DrumScore.ToString();
        DrumScore.onValueChanged.AddListener((value) =>
        {
            GameCenter.Instance.gameData.DrumScore = int.Parse(value);
        });

        MaxScore.text = data.MaxScore.ToString();
        MaxScore.onValueChanged.AddListener((value) =>
        {
            GameCenter.Instance.gameData.MaxScore = int.Parse(value);
        });

        MiddleScore.text = data.MiddleScore.ToString();
        MiddleScore.onValueChanged.AddListener((value) =>
        {
            GameCenter.Instance.gameData.MiddleScore = int.Parse(value);
        });

        MinScore.text = data.MinScore.ToString();
        MinScore.onValueChanged.AddListener((value) =>
        {
            GameCenter.Instance.gameData.MinScore = int.Parse(value);
        });
    }

}
