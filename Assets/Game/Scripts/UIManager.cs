using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public void Init()
    {
        HideScore();

        HidePlayerInputPage();

        InitPlayerName();

        UpdateGameMode();

        HideEndPage();
        InitHUD();
    }

    public void StartGame(bool skipTitle)
    {
        HideEndPage();
        if (skipTitle)
        {
            HideGameTitle();
            GameStartReady();
        }
        else
            ShowGameTitle();
    }

    private void GameStartReady()
    {
        ShowHUD();
        HidePlayerInputPage();
        ShowRankPage();
        HideScore();

        ShowTutorial();

        ShowReadyBtn();
        HideTimer();
        HideEndPage();
        ShowPlayerName();

        inputPlayerName.interactable = true;
        GameCenter.Instance.GameStartReady = true;
    }

    public void ReadyToChallenge(bool isInfinite)
    {

        //HideGameTitle();
        HidePlayerInputPage();
        HideRankPage();
        ShowTimer();
        ShowScore();

        HideTutorial();

        HideReadyBtn();
        HidePlayerName();

        inputPlayerName.interactable = false;
    }

    public void GameOver()
    {
        HideTimer();
        //HideScore();
        HideReadyBtn();
        // if (GameCenter.Instance.playerDatabase.CheckRank())
        //     ShowPlayerInputPage();
        // else
        ShowEndPage();
        HidePlayerName();

        inputPlayerName.interactable = false;
    }

    #region ---------- Ready Btn ----------------

    [Header("Ready Btn")]
    public GameObject ReadyBtn;
    public Image tReadyBtn;

    public void ShowReadyBtn()
    {
        ReadyBtn.SetActive(true);
        tReadyBtn.color = new Color(tReadyBtn.color.r, tReadyBtn.color.g, tReadyBtn.color.b, 0.75f);
        ReadyBtn.transform.DOScale(1.05f, 1f).SetLoops(-1, LoopType.Yoyo);
        tReadyBtn.DOFade(1f, 1f).SetLoops(-1, LoopType.Yoyo);
    }

    public void HideReadyBtn()
    {
        ReadyBtn.SetActive(false);
        ReadyBtn.transform.DOKill();
        tReadyBtn.DOKill();
    }

    #endregion

    // #region ------------ World Dir Sign ----------------

    // public RectTransform imageTransform; // UI Image 的 RectTransform 组件

    // public void SetWorldDirSign(Vector2 dir)
    // {

    //     // 计算随机方向向量的角度
    //     float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

    //     // 设置UI Image 的旋转角度
    //     imageTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    // }

    // public void SwitchWorldDirSign(bool show)
    // {
    //     imageTransform.gameObject.SetActive(show);
    // }

    // #endregion

    #region ---------------- Input Bar -------------------
    [Header("Input Bar")]
    public Image LeftInputFill;
    public Image RightInputFill;
    public Color UpColor, DownColor;

    public void SetLeftInputFill(float amount, int index)
    {
        LeftInputFill.fillAmount = amount;
        LeftInputFill.color = index > 0 ? UpColor : DownColor;
    }


    public void SetRightInputFill(float amount, int index)
    {
        RightInputFill.fillAmount = amount;
        RightInputFill.color = index > 0 ? UpColor : DownColor;
    }

    #endregion

    #region ---------------- Score -------------------

    [Header("Score")]
    public TextMeshProUGUI tScore;
    public int startValue;
    public int endValue;
    public float duration = 0.5f;


    public void UpdateScore(int oldValue)
    {
        startValue = oldValue;
        endValue = GameCenter.Instance.Score;

        DOTween.To(() => startValue, x =>
        {
            startValue = x;
            tScore.text = Mathf.Floor(startValue).ToString();
        }, endValue, duration);

        tScore.text = GameCenter.Instance.Score.ToString();
    }

    public void HideScore()
    {
        tScore.gameObject.SetActive(false);
    }

    public void ShowScore()
    {
        tScore.gameObject.SetActive(true);
    }

    #endregion

    #region -------------- Timer ----------------

    [Header("Timer")]
    public TextMeshProUGUI tTimer;
    public Transform TimerStartSpot, TimerActSpot;
    private bool timeLastCountOn = false;
    public Color HighlightColor;
    //Sequence timerSeq;

    public void ShowTimer()
    {
        tTimer.color = Color.white;
        tTimer.transform.position = TimerStartSpot.position;
        tTimer.gameObject.SetActive(true);
        tTimer.transform.localScale = Vector3.one * 2f;
        tTimer.alpha = 0.5f;

        tTimer.transform.DOMove(TimerActSpot.position, 1f).SetEase(Ease.InOutExpo);
        tTimer.transform.DOScale(1f, 1f).SetEase(Ease.InOutExpo);
        tTimer.DOFade(1f, 0.5f).SetEase(Ease.InOutExpo);

        timeLastCountOn = false;

        UpdateTimer();
    }

    public void UpdateTimer()
    {
        float time = GameCenter.Instance.Timer;
        if (time <= 5f && timeLastCountOn == false)
        {
            // timerSeq = DOTween.Sequence();
            // timerSeq.Append(tTimer.transform.DOScale(1.2f, 0.5f))
            // .Join(tTimer.DOColor(HighlightColor, 0.3f))
            // .Append(tTimer.transform.DOScale(1f, 0.5f))
            // .AppendInterval(0.2f)
            // .Append(tTimer.DOColor(Color.white, 0.3f))
            // .SetEase(Ease.InCubic)
            // .SetLoops(-1)
            // .Play();
            tTimer.transform.DOScale(1.3f, 0.5f).SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
            tTimer.DOColor(HighlightColor, 0.5f).SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
            timeLastCountOn = true;
        }
        tTimer.text = GameCenter.Instance.Timer.ToString("F1") + "s";
    }

    public void HideTimer()
    {
        // if (timerSeq != null)
        //     timerSeq.Kill();
        tTimer.DOKill();
        tTimer.transform.DOKill();
        tTimer.gameObject.SetActive(false);
    }

    #endregion

    #region ------------- End Page --------------

    [Header("End Page")]
    public GameObject EndPageRoot;
    public GameObject EndPage;
    public TextMeshProUGUI tEndScore;

    public void ShowEndPage()
    {
        EndPageRoot.SetActive(true);
        EndPage.transform.localScale = Vector3.zero;
        EndPage.transform.DOScale(1f, 1f).SetEase(Ease.InOutExpo);
        tEndScore.text = GameCenter.Instance.Score.ToString();
    }

    public void HideEndPage()
    {
        EndPage.transform.localScale = Vector3.one;
        EndPageRoot.SetActive(false);
        // EndPage.transform.DOScale(0f, 1f).SetEase(Ease.InOutExpo).OnComplete(() =>
        // {
        //     Debug.Log("End Page hidden.");
        // });
    }

    #endregion

    #region ------------ Player Name Input Page ---------------

    [Header("Player Name Input Page")]
    public GameObject PlayerInputPageRoot;
    public GameObject PlayerInputPage;
    public TMP_InputField NameInput;

    public void ShowPlayerInputPage()
    {
        PlayerInputPageRoot.SetActive(true);
        PlayerInputPage.transform.localScale = Vector3.zero;
        PlayerInputPage.transform.DOScale(1f, 1f).SetEase(Ease.InOutExpo);
    }

    public void HidePlayerInputPage()
    {
        PlayerInputPageRoot.SetActive(false);
        ClearNameInput();
    }

    public void ClearNameInput()
    {
        NameInput.text = "";
    }

    public void BtnInputNameClick()
    {
        string name = NameInput.text;
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        GameCenter.Instance.playerDatabase.AddData(name, GameCenter.Instance.Score);
        HidePlayerInputPage();
        GameCenter.Instance.Restart();
    }

    #endregion

    #region -------------- Rank Page ----------------

    [Header("Rank Page")]
    public GameObject RankPage;
    public List<PlayerDataInstance> playerDataInstances;
    public Transform RankShowSpot, RankHideSpot;

    public GameObject LoadingStatus, FailStatus;

    public void ShowRankPage()
    {
        RankPage.transform.position = RankHideSpot.position;
        RankPage.transform.DOMove(RankShowSpot.position, 1f).SetEase(Ease.InOutExpo);
    }

    public void ResetRankPage()
    {
        LoadingStatus.SetActive(false);
        FailStatus.SetActive(false);

        foreach (var instance in playerDataInstances)
        {
            instance.gameObject.SetActive(false);
        }
    }

    public void UpdateRankPage(List<PlayerData> dataList = null)
    {
        ResetRankPage();

        if (dataList == null)
        {
            return;
        }

        int dataLength = dataList.Count;
        for (int i = 0; i < playerDataInstances.Count; i++)
        {
            if (i < dataLength)
                playerDataInstances[i].UpdateData(dataList[i], i + 1);
            else
                playerDataInstances[i].gameObject.SetActive(false);
        }
    }

    public void HideRankPage()
    {
        RankPage.transform.position = RankShowSpot.position;
        RankPage.transform.DOMove(RankHideSpot.position, 1f).SetEase(ease: Ease.InOutExpo);
    }

    public void ShowRankLoadingStatus()
    {
        ResetRankPage();
        LoadingStatus.SetActive(true);
    }

    public void ShowRankFailStatus()
    {
        ResetRankPage();
        FailStatus.SetActive(true);
    }

    #endregion

    #region ---------- Game Title -------------

    [Header("Game Title")]
    public CanvasGroup GameTitle;
    public Transform gameTitleHideSpot, gameTitleShowSpot;

    public void ShowGameTitle()
    {
        GameTitle.gameObject.SetActive(true);
        GameTitle.transform.position = gameTitleShowSpot.position;
        GameTitle.alpha = 0;
        var seq = DOTween.Sequence();
        seq.Append(GameTitle.DOFade(1f, 0.5f))
        .AppendInterval(0.5f)
        .Append(GameTitle.transform.DOMove(gameTitleHideSpot.position, 0.5f).SetEase(Ease.InOutExpo))
        .InsertCallback(1f, () =>
        {
            GameStartReady();
        })
        .Play();
        //GameTitle.transform.DOMove(gameTitleShowSpot.position, 1f).SetEase(Ease.InOutExpo);
    }

    public void HideGameTitle()
    {
        GameTitle.DOKill();
        GameTitle.gameObject.SetActive(false);
    }

    #endregion

    #region ---------- HUD -------------

    [Header("HUD")]
    public CanvasGroup HUD;

    private void InitHUD()
    {
        HUD.alpha = 0;
    }

    public void ShowHUD()
    {
        HUD.alpha = 0;
        HUD.DOFade(1f, 1f);
    }

    public void HideHUD()
    {
        HUD.DOFade(0f, 1f);
    }

    #endregion

    #region -------- Player Name ------------

    [Header("Player Name")]
    public TMP_InputField inputPlayerName;

    private void InitPlayerName()
    {
        UpdatePlayerName();
        inputPlayerName.onValueChanged.AddListener(GameCenter.Instance.SetPlayerName);
    }

    public void UpdatePlayerName()
    {
        inputPlayerName.text = GameCenter.Instance.PlayerName;
    }

    public void ShowPlayerName()
    {
        inputPlayerName.gameObject.SetActive(true);
    }

    public void HidePlayerName()
    {
        inputPlayerName.gameObject.SetActive(false);
    }

    #endregion

    #region ----------- Game Mode -------------

    [Header("Game Mode")]
    public TextMeshProUGUI tGameMode;

    public void UpdateGameMode()
    {
        bool isInfinite = GameCenter.Instance.IsInfiniteMode;
        tGameMode.text = isInfinite ? "Infinite\n Mode" : "Normal\n Mode";
    }

    #endregion


    #region ----------- Tutorial -------------

    [Header("Tutorial")]
    public GameObject TutorialPage;
    public Transform TutorialShowSpot, TutorialHideSpot;

    public GameObject[] TutorialCheckMarks;
    public bool[] TutorialCheckFlags;
    public bool TutorialOn = false;
    public int LeafEatCount;

    public void ShowTutorial()
    {
        TutorialOn = true;
        ResetTutorial();
        //TutorialPage.SetActive(true);
        TutorialPage.transform.position = TutorialHideSpot.position;
        TutorialPage.transform.DOMove(TutorialShowSpot.position, 1f).SetEase(Ease.InOutExpo);
    }

    public void HideTutorial()
    {
        TutorialOn = false;
        //TutorialPage.SetActive(false);
        TutorialPage.transform.position = TutorialShowSpot.position;
        TutorialPage.transform.DOMove(TutorialHideSpot.position, 1f).SetEase(Ease.InOutExpo);
    }

    private void ResetTutorial()
    {
        LeafEatCount = 0;
        for (int i = 0; i < TutorialCheckMarks.Length; i++)
        {
            TutorialCheckMarks[i].SetActive(false);
            TutorialCheckFlags[i] = false;
        }
    }

    public void SetTutorialCheckMark(int index, bool isOn)
    {
        if (TutorialOn == false) return;

        TutorialCheckMarks[index].SetActive(isOn);
        TutorialCheckFlags[index] = isOn;
    }

    public void AddLeafEatCount()
    {
        if (TutorialOn == false) return;

        LeafEatCount++;
        if (LeafEatCount >= 3)
        {
            SetTutorialCheckMark(5, true);
        }
    }


    #endregion

}
