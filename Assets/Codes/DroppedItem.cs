using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    private int itemId;
    private string itemName;
    private int quantity = 1; // 기본 수량

    public void Initialize(int id, string name)
    {
        itemId = id;
        itemName = name;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Collision detected with: {collision.name}");

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player collided with dropped item!");
            // 인벤토리에 아이템 추가
            InventoryManager inventoryManager = collision.GetComponent<InventoryManager>();
            if (inventoryManager != null)
            {
                inventoryManager.AddItem(itemId, itemName, quantity);
            }

            // 아이템 제거
            Destroy(gameObject);
        }
    }
}
