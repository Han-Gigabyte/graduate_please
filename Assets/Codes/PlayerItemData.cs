using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerItemData
{
    public int stone;            // 아이템: Stone 개수
    public int tree;             // 아이템: 나무 개수
    public int skin;             // 아이템: Skin 개수
    public int steel;            // 아이템: Steel 개수
    public int gold;             // 아이템: Gold 개수
    public int battery;          // 아이템: Battery 개수
    public List<int> items;   // 플레이어가 소유한 아이템 목록

    public string GetItemNameById(int id)
    {
        switch (id)
        {
            case 0: return "Stone";
            case 1: return "Tree";
            case 2: return "Skin";
            case 3: return "Steel";
            case 4: return "Gold";
            case 5: return "Battery";
            default: return "Unknown";
        }
    }
}