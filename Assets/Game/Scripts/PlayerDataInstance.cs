using TMPro;
using UnityEngine;

public class PlayerDataInstance : MonoBehaviour
{
    public TextMeshProUGUI tRank;
    public TextMeshProUGUI tName;
    public TextMeshProUGUI tScore;

    public void UpdateData(PlayerData data, int rank)
    {
        this.gameObject.SetActive(true);
        tRank.text = $"#{rank} - ";
        tName.text = $"{data.Name}";
        tScore.text = $"Score: {data.Score}";
    }

}
