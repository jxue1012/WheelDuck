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
    }

    public void StartGame()
    {
        ShowGameTitle();
        HidePlayerInputPage();
        ShowRankPage();
        HideScore();

        ShowReadyBtn();
        HideTimer();
        HideEndPage();
        ShowPlayerName();
    }

    public void ReadyToChallenge(bool isInfinite)
    {
        HideGameTitle();
        HidePlayerInputPage();
        HideRankPage();
        ShowTimer();
        ShowScore();

        HideReadyBtn();
        HidePlayerName();
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
    }

    #region ---------- Ready Btn ----------------

    [Header("Ready Btn")]
    public GameObject ReadyBtn;
    public TextMeshProUGUI tReadyBtn;

    public void ShowReadyBtn()
    {
        ReadyBtn.SetActive(true);
        tReadyBtn.alpha = 0.75f;
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

    public void UpdateScore()
    {
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
    public void ShowTimer()
    {
        tTimer.gameObject.SetActive(true);
        UpdateTimer();
    }

    public void UpdateTimer()
    {
        tTimer.text = GameCenter.Instance.Timer.ToString("F1") + "s";
    }

    public void HideTimer()
    {
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
        EndPage.transform.DOScale(0f, 1f).SetEase(Ease.InOutExpo).OnComplete(() => EndPageRoot.SetActive(false));
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
    public GameObject GameTitle;
    public Transform gameTitleHideSpot, gameTitleShowSpot;

    public void ShowGameTitle()
    {
        GameTitle.transform.position = gameTitleHideSpot.position;
        GameTitle.transform.DOMove(gameTitleShowSpot.position, 1f).SetEase(Ease.InOutExpo);
    }

    public void HideGameTitle()
    {
        GameTitle.transform.position = gameTitleShowSpot.position;
        GameTitle.transform.DOMove(gameTitleHideSpot.position, 1f).SetEase(Ease.InOutExpo);
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

}
