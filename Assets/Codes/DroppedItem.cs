using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public int itemId;        // 아이템 ID
    public string itemName;   // 아이템 이름
    public int quantity;      // 아이템 개수

    public void Initialize(int id, int quantity, PlayerItemData playerItemData)
    {
        this.itemId = id;
        this.quantity = quantity;
        this.itemName = playerItemData.GetItemNameById(id); // ID를 기반으로 이름 설정
        Debug.Log($"Dropped Item Initialized: ID = {itemId}, Name = {itemName}, Quantity = {quantity}");
    }
}

