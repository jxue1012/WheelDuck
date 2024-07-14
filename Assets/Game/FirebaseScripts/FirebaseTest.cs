using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System.Collections.Generic;

public class FirebaseTest : MonoBehaviour
{

    public Text text;

    [DllImport("__Internal")]
    private static extern void GetJSON(string objectName, string callback, string fallback);
    [DllImport("__Internal")]
    private static extern void SetJSON(string key, string value, string objectName, string callback, string fallback);

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            GetJSON(gameObject.name, "TestFunc", "OnRequestFailed");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            var list = GameCenter.Instance.playerDatabase.playerDatas;

            PlayerData data = new();
            data.Name = "MySelf";
            data.Score = 100;

            PlayerData data2 = new();
            data.Name = "CGT";
            data.Score = 200;

            list.Add(data);
            list.Add(data2);

            RankData tempData = new();


            var json = JsonUtility.ToJson(tempData);

            SetJSON("Test", json, gameObject.name, "SetDataCallback", "OnRequestFailed");
            //SetJSON("Test", "Hello World", gameObject.name, "TestFunc", "OnRequestFailed");
        }
    }

    public void TestFunc(string data)
    {
        text.color = Color.green;
        text.text = data;
    }

    public void OnRequestFailed(string error)
    {
        text.color = Color.red;
        text.text = error;
    }

    public void SetDataCallback(string json)
    {
        var data = JsonUtility.FromJson<RankData>(json);
        if (data != null)
        {
            text.text = string.Empty;
 
        }
        else
            text.text = "Data is null";
    }
}
