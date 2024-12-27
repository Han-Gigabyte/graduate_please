using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItemBehavior : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InventoryManager inventoryManager = collision.GetComponent<InventoryManager>();
            if (inventoryManager != null)
            {
                DroppedItem droppedItem = GetComponent<DroppedItem>();
                if (droppedItem != null)
                {
                    // 아이템 추가
                    inventoryManager.AddItem(droppedItem.itemId, droppedItem.quantity);

                    // 로그 출력
                    Debug.Log($"Player collected: {droppedItem.itemName} (ID: {droppedItem.itemId}, Quantity: {droppedItem.quantity})");

                    // 아이템 제거
                    Destroy(gameObject);
                }
            }
        }
    }

}

