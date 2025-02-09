using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public PlayerItemData inventory = new PlayerItemData(); // 인벤토리 리스트

    // ID와 이름 매핑
    private Dictionary<int, string> itemNames = new Dictionary<int, string>
    {
        { 0, "Stone" },
        { 1, "Tree" },
        { 2, "Skin" },
        { 3, "Steel" },
        { 4, "Gold" }
    };

    // ID에 따른 이름 반환
    public string GetItemNameById(int id)
    {
        if (itemNames.TryGetValue(id, out string name))
        {
            return name;
        }
        return "Unknown";
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 기존 인스턴스가 있으면 새로운 인스턴스 삭제
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
    }


    // 아이템 추가
    public void AddItem(int id, int quantity)
    {
        switch(id){
            case 0: //돌
            inventory.stone+=quantity;
            break;
            case 1: //나무 
            inventory.tree+=quantity;
            break;
            case 2: // 가죽 
            inventory.skin+=quantity;
            break;
            case 3: // 철
            inventory.steel+=quantity;
            break;
            case 4: //금
            inventory.gold+=quantity;
            break; 
            case 5: //돈
            inventory.money+=quantity;
            break;
            case 6: //배터리
            inventory.battery+=quantity;
            break;
            case 7: //새로운 아이템추가, 이경우 quaitiy가 아이템의 아이디
            inventory.items.Add(quantity);
            break;
        }
    
        Debug.Log($"Added {quantity} {name} to inventory.");
    }

// 아이템 제거
public void RemoveItem(int id, int quantity)
    {
        switch(id){
            case 0: //돌
            inventory.stone-=quantity;
            break;
            case 1: //나무 
            inventory.tree-=quantity;
            break;
            case 2: // 가죽 
            inventory.skin-=quantity;
            break;
            case 3: // 철
            inventory.steel-=quantity;
            break;
            case 4: //금
            inventory.gold-=quantity;
            break;
            case 5: //돈
            inventory.money-=quantity;
            break;
            case 6: //배터리
            inventory.battery-=quantity;
            break;
        }
    }
    public void SaveInventory(){
        SaveManager.Instance.SaveData(inventory);
    }
    public void loadInventory(){
        inventory = SaveManager.Instance.LoadData();
    }
}