using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public PlayerItemData inventory = new PlayerItemData(); // 인벤토리 리스트


    // 아이템 추가
    public void AddItem(int id, int quantity)
    {
        switch(id){
            case 0: // Stone
                inventory.stone += quantity;
                Debug.Log($"Added {quantity} Stone(s). Total: {inventory.stone}");
                break;
            case 1: // Tree
                inventory.tree += quantity;
                Debug.Log($"Added {quantity} Tree(s). Total: {inventory.tree}");
                break;
            case 2: // Skin
                inventory.skin += quantity;
                Debug.Log($"Added {quantity} Skin(s). Total: {inventory.skin}");
                break;
            case 3: // Steel
                inventory.steel += quantity;
                Debug.Log($"Added {quantity} Steel(s). Total: {inventory.steel}");
                break;
            case 4: // Gold
                inventory.gold += quantity;
                Debug.Log($"Added {quantity} Gold(s). Total: {inventory.gold}");
                break;
        }

        // 리스트에 아이템 추가
        for (int i = 0; i < quantity; i++)
        {
            inventory.items.Add(id);
        }

        Debug.Log($"Item ID {id} added to list. Current items: {string.Join(", ", inventory.items)}");
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
        }

        // 리스트에서 아이템 제거
        for (int i = 0; i < quantity; i++)
        {
            if (inventory.items.Contains(id))
            {
                inventory.items.Remove(id);
            }
        }

        Debug.Log($"Item ID {id} removed from list. Current items: {string.Join(", ", inventory.items)}");
    }

    // 인벤토리 상태 출력 (디버그용)
    public void PrintInventory()
    {
        Debug.Log($"Stone: {inventory.stone}, Tree: {inventory.tree}, Skin: {inventory.skin}, Steel: {inventory.steel}, Gold: {inventory.gold}, Battery: {inventory.battery}");
    }

    public void SaveInventory(){
        SaveManager.Instance.SaveData(inventory);
    }
    public void LoadInventory(){
        inventory = SaveManager.Instance.LoadData();
    }
}