using System;
using System.Collections;
using System.IO;
using UnityEngine;


public class SaveManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerData
    {
        public int score;
        public string playerName;
        public int Stone;
        public int skin;
        public int steel;
        public int gold;
        public int battery;
        public ArrayList items;
    }

    private string savePath;

    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "saveFile.json");
    }

    public void SaveData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true); // JSON 변환
        File.WriteAllText(savePath, json); // 파일로 저장
        Debug.Log("Data Saved: " + savePath);
    }

    public PlayerData LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath); // 파일에서 읽기
            PlayerData data = JsonUtility.FromJson<PlayerData>(json); // JSON -> 객체
            Debug.Log("Data Loaded");
            return data;
        }
        Debug.LogWarning("No Save File Found!");
        return null;
    }
}