using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; } // Singleton 인스턴스

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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }
    }

    // ID에 따른 이름 반환
    public string GetItemNameById(int id)
    {
        if (itemNames.TryGetValue(id, out string name))
        {
            return name;
        }
        return "Unknown";
    }

    // 아이템 추가
    public void AddItem(int id, int quantity)
    {
        switch (id)
        {
            case 0: // 돌
                inventory.stone += quantity;
                break;
            case 1: // 나무
                inventory.tree += quantity;
                break;
            case 2: // 가죽
                inventory.skin += quantity;
                break;
            case 3: // 철
                inventory.steel += quantity;
                break;
            case 4: // 금
                inventory.gold += quantity;
                break;
            case 5: // 돈
                inventory.money += quantity;
                break;
            case 6: // 배터리
                inventory.battery += quantity;
                break;
            case 7: // 새로운 아이템 추가, 이 경우 quantity가 아이템의 아이디
                inventory.items.Add(quantity);
                break;
        }

        Debug.Log($"Added {quantity} {GetItemNameById(id)} to inventory."); // 아이템 이름 출력
    }

    // 아이템 제거
    public void RemoveItem(int id, int quantity)
    {
        switch (id)
        {
            case 0: // 돌
                inventory.stone -= quantity;
                break;
            case 1: // 나무
                inventory.tree -= quantity;
                break;
            case 2: // 가죽
                inventory.skin -= quantity;
                break;
            case 3: // 철
                inventory.steel -= quantity;
                break;
            case 4: // 금
                inventory.gold -= quantity;
                break;
            case 5: // 돈
                inventory.money -= quantity;
                break;
            case 6: // 배터리
                inventory.battery -= quantity;
                break;
        }
    }

    public void SaveInventory()
    {
        SaveManager.Instance.SaveData(inventory);
    }

    public void LoadInventory()
    {
        inventory = SaveManager.Instance.LoadData();
    }
}