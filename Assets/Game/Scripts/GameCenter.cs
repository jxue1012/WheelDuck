
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCenter : MonoBehaviour
{
    private static GameCenter instance;
    public AudioClip StartSound;
    public AudioClip ReadySound;
    public AudioClip EndSound;
    public AudioClip ScoreSound;

    public bool GameStartReady;

    public static GameCenter Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
        testUI.gameObject.SetActive(false);
        GameStartReady = false;
    }

    private void Update()
    {
        if (TimerOn)
        {
            if (IsInfiniteMode)
            {
                Timer += Time.deltaTime;
                uIManager.UpdateTimer();
            }
            else
            {
                Timer -= Time.deltaTime;
                uIManager.UpdateTimer();

                if (Timer <= 0)
                {
                    TimerOn = false;
                    GameOver();
                }
            }
        }

        // if (Input.GetKeyDown(KeyCode.T) && Input.GetKey(KeyCode.LeftControl))
        // {
        //     testUI.Switch();
        // }

        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //    //ChangeGameMode();
        //    AddScore(15);
        // }

        if (GameStartReady && Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if (GameStatus == 0)
            {
                ReadyToChallenge();
            }
            if (GameStatus == 2)
            {
                Restart();
            }
        }
    }

    public TestUI testUI;
    public FloorControl floorControl;
    public PlayerControl2 player;

    public UIManager uIManager;

    public RankManager rankManager;


    public LayerMask FloorLM;
    public int GameStatus;
    public GameData gameData;

    public bool IsInfiniteMode { get; private set; }
    public bool TestModeOn;

    public GameObject windParticleSystem;
    public GameObject stoneParticleSystem;

    private void Start()
    {
        InitPlayerName();
        testUI.Init();
        floorControl.Init();
        player.Init();
        uIManager.Init();

        rankManager.Init();

        StartGame();
    }

    public void SetWorldDir(Vector2 dir)
    {
        player.SetWorldDir(dir);
        floorControl.SetArrowDir(dir);
    }


    public void Restart()
    {
        GameStartReady = false;
        StartGame();
    }

    public void StartGame()
    {
        Score = 0;
        GameStatus = 0;
        CamStart.SetActive(true);
        CamChallenge.SetActive(false);
        player.StartGame();
        floorControl.StartGame();
        ResetTimer();
        ResetScore();
        uIManager.StartGame();

        PlaySound(StartSound);
    }

    public void ReadyToChallenge(bool isInfinite = false)
    {
        CamStart.SetActive(false);
        CamChallenge.SetActive(true);
        GameStatus = 1;
        player.ReadyToChallenge();
        floorControl.ReadyToChallenge(isInfinite);

        TimerOn = true;
        ResetTimer();

        uIManager.ReadyToChallenge(isInfinite);

        PlaySound(ReadySound);
        windParticleSystem.SetActive(true);
        stoneParticleSystem.SetActive(true);
    }

    public void GameOver()
    {
        TimerOn = false;
        CamStart.SetActive(true);
        CamChallenge.SetActive(false);
        if (GameStatus == 0)
        {
            Restart();
        }
        else
        {
            GameStatus = 2;
            floorControl.GameOver();
            player.GameOver();
            uIManager.GameOver();

            rankManager.SetDataToUpdate(Score);

            PlaySound(EndSound);

            windParticleSystem.SetActive(false);
            stoneParticleSystem.SetActive(false);

            floorControl.ResetSpeed();
        }
    }

    public void ChangeGameMode()
    {
        IsInfiniteMode = !IsInfiniteMode;
        uIManager.UpdateGameMode();
        ResetTimer();
        rankManager.DataToUpdate = null;
        rankManager.UpdateRankData(false);
    }

    //Score
    public int Score;
    public GameObject AddScoreEffectPrefab;

    public void ResetScore()
    {
        Score = 0;
        uIManager.UpdateScore(0);
    }

    public void AddScore(int value)
    {
        int oldValue = Score;
        var effect = GameObject.Instantiate(AddScoreEffectPrefab).GetComponent<EffectAddScore>();
        effect.Show(value);

        Score += value;
        uIManager.UpdateScore(oldValue);

        PlaySound(ScoreSound);
    }

    //Timer
    public float MaxTime = 30f;
    public float Timer;
    private bool TimerOn;

    public void ResetTimer()
    {
        if (IsInfiniteMode)
            Timer = 0;
        else
            Timer = MaxTime;
    }

    //Cam
    public GameObject CamStart;
    public GameObject CamChallenge;

    //Data
    public PlayerDatabase playerDatabase;

    //Player Name
    public string PlayerName;

    public void InitPlayerName()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            PlayerName = PlayerPrefs.GetString("PlayerName");
        }
        else
        {
            PlayerName = "Fresh Duck";
        }
    }

    public void SetPlayerName(string name)
    {
        if (name != string.Empty && name.Length > 0)
        {

            PlayerName = LimitStringLength(name, 15);
            PlayerPrefs.SetString("PlayerName", name);
        }

        uIManager.UpdatePlayerName();

    }

    string LimitStringLength(string str, int maxLength)
    {
        if (str.Length > maxLength)
        {
            return str.Substring(0, maxLength);
        }
        else
        {
            return str;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public string Name;
    public int Score;
}