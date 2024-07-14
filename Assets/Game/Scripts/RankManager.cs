using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

public class RankManager : MonoBehaviour
{
    public Text text;
    public RankData rankData = new();

    [DllImport("__Internal")]
    private static extern void GetJSON(string key, string objectName, string callback, string fallback);
    [DllImport("__Internal")]
    private static extern void SetJSON(string key, string value, string objectName, string callback, string fallback);

    public void Init()
    {
        DataToUpdate = null;
        UpdateRankData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            UpdateRankData();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            DataToUpdate = new();
            DataToUpdate.Name = "Test Player";
            DataToUpdate.Score = 10;
            UpdateRankData();
        }
    }

    // private void LoadRankData()
    // {
    //     var ui = GameCenter.Instance.uIManager;
    //     ui.ShowRankLoadingStatus();

    //     try
    //     {
    //         GetJSON(gameObject.name, "GetDataCallback", "OnRequestFailed");
    //     }
    //     catch (System.Exception e)
    //     {
    //         Debug.LogError(e);
    //         ui.ResetRankPage();
    //         ui.ShowRankFailStatus();
    //     }
    // }

    public void GetDataCallback(string data)
    {
        GameCenter.Instance.uIManager.ResetRankPage();
        RankData rank = JsonUtility.FromJson<RankData>(data);
        rankData = rank;
        GameCenter.Instance.uIManager.UpdateRankPage(rank.playerDatas);
    }

    public void OnRequestFailed(string error)
    {
        GameCenter.Instance.uIManager.ShowRankFailStatus();
    }

    public void SetDataCallback(string json)
    {
        var data = JsonUtility.FromJson<RankData>(json);

        if (data != null)
        {
            rankData = data;

            GameCenter.Instance.uIManager.UpdateRankPage(data.playerDatas);
        }
            
    }

    //----------- Update Data ---------------
    public PlayerData DataToUpdate;

    public void SetDataToUpdate(int score)
    {
        DataToUpdate = new();
        DataToUpdate.Name = GameCenter.Instance.PlayerName;
        DataToUpdate.Score = score;

        UpdateRankData();
    }

    public void UpdateRankData()
    {
        if(GameCenter.Instance.TestModeOn)
            return;

        try
        {
            var ui = GameCenter.Instance.uIManager;
            ui.ShowRankLoadingStatus();
            string key = GameCenter.Instance.IsInfiniteMode ? "Infinite" : "Normal";
            GetJSON(key, gameObject.name, "UpdateDataGet", "OnRequestFailed");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void UpdateDataGet(string data)
    {
        string key = GameCenter.Instance.IsInfiniteMode ? "Infinite" : "Normal";
        GameCenter.Instance.uIManager.ResetRankPage();
        RankData rank = JsonUtility.FromJson<RankData>(data);
        rank.OrderData();
        rankData = rank;

        if (DataToUpdate != null)
        {
            if (rankData.playerDatas.Count < 10)
            {
                rankData.playerDatas.Add(DataToUpdate);
                rankData.OrderData();
                string json = JsonUtility.ToJson(rankData);
                SetJSON(key, json, gameObject.name, "UpdateDataSet", "OnRequestFailed");
            }
            else
            {
                bool isRank = CheckRank();
                if (isRank)
                {
                    AddData(DataToUpdate);
                    //更新
                    rankData.playerDatas.Add(DataToUpdate);
                    string json = JsonUtility.ToJson(rankData);
                    SetJSON(key, json, gameObject.name, "UpdateDataSet", "OnRequestFailed");
                }
                else
                {
                    if (rankData != null)
                    {
                        GameCenter.Instance.uIManager.UpdateRankPage(rankData.playerDatas);
                    }
                    else
                        GameCenter.Instance.uIManager.ResetRankPage();
                }
            }
        }
        else
        {
            if (rankData != null)
            {
                GameCenter.Instance.uIManager.UpdateRankPage(rankData.playerDatas);
            }
            else
                GameCenter.Instance.uIManager.ResetRankPage();
        }

    }

    public void UpdateDataSet(string json)
    {
        var data = JsonUtility.FromJson<RankData>(json);
        if (data != null)
        {
            rankData = data;
            DataToUpdate = null;
            GameCenter.Instance.uIManager.UpdateRankPage(rankData.playerDatas);
        }
        else
        {
            GameCenter.Instance.uIManager.ResetRankPage();
        }
    }

    public bool CheckRank()
    {
        int score = DataToUpdate.Score;
        if (rankData.playerDatas.Count > 0)
        {
            if (rankData.playerDatas.Count < 10)
                return true;

            if (score > rankData.playerDatas.Last().Score)
            {
                return true;
            }
            else
                return false;

        }
        else
            return true;

    }

    public void AddData(PlayerData data)
    {
        if (data == null) return;

        rankData.playerDatas.Add(data);
        rankData.playerDatas = rankData.playerDatas.OrderByDescending(x => x.Score).ToList();
        while (rankData.playerDatas.Count > 10)
        {
            int count = rankData.playerDatas.Count;
            rankData.playerDatas.RemoveAt(count - 1);
        }
    }



}




[System.Serializable]
public class RankData
{
    public List<PlayerData> playerDatas = new List<PlayerData>();

    public void OrderData()
    {
        playerDatas = playerDatas.OrderByDescending(x => x.Score).ToList();
    }

}
