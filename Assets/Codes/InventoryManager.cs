using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public PlayerItemData inventory; // 인벤토리 리스트
    public static InventoryManager Instance { get; private set; }
    void Awake(){
        inventory = new PlayerItemData();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
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