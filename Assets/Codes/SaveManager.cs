using System;
using System.Collections;
using System.IO;
using UnityEngine;


public class SaveManager : MonoBehaviour
{
    private string savePath;
    public static SaveManager Instance { get; private set; }

    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "saveFile.json");
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        savePath = Path.Combine(Application.persistentDataPath, "playerInventory.json");
    }

    public void SaveData(PlayerItemData data)
    {
        string json = JsonUtility.ToJson(data, true); // JSON 변환
        File.WriteAllText(savePath, json); // 파일로 저장

        PlayerPrefs.SetInt("screw", data.screw);
        PlayerPrefs.SetInt("page", data.page);
        PlayerPrefs.SetInt("money", data.money);
        PlayerPrefs.Save();

        Debug.Log("Data Saved: " + savePath);
    }

    public PlayerItemData LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath); // 파일에서 읽기
            PlayerItemData data = JsonUtility.FromJson<PlayerItemData>(json); // JSON -> 객체

            data.money = PlayerPrefs.GetInt("money", 0); // 기본값 0
            data.screw = PlayerPrefs.GetInt("screw", 1); // 기본값 1
            data.page = PlayerPrefs.GetInt("page", 1);   // 기본값 1
        
            Debug.Log("Data Loaded");
            return data;
        }
        Debug.LogWarning("No Save File Found!");
        return new PlayerItemData(); // 데이터가 없으면 새로운 데이터 생성
    }
}