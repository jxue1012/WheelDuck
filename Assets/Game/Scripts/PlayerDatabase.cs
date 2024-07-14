using UnityEngine;
using System.Linq;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "PlayerDatabase", menuName = "MySO/PlayerDatabase", order = 0)]
public class PlayerDatabase : ScriptableObject
{
    public List<PlayerData> playerDatas = new List<PlayerData>();

    public void ClearData()
    {
        playerDatas.Clear();
    }

    public void AddData(string name, int score)
    {
        PlayerData data = new PlayerData();
        data.Name = name;
        data.Score = score;
        playerDatas.Add(data);
        playerDatas = playerDatas.OrderByDescending(x => x.Score).ToList();
        while (playerDatas.Count > 10)
        {
            int count = playerDatas.Count;
            playerDatas.RemoveAt(count - 1);
        }
    }

    public bool CheckRank()
    {
        int score = GameCenter.Instance.Score;
        if (playerDatas.Count > 0)
        {
            if (playerDatas.Count < 10)
                return true;

            if (score > playerDatas.Last().Score)
            {
                return true;
            }
            else
                return false;

        }
        else
            return true;

    }
}

