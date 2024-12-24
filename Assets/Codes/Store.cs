using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Store : MonoBehaviour
{
    public float interactionRange = 2f;
    public GameObject StoreWindow;
    ItemListData itemList;
    PlayerItemData inventory;
    List<int> selectedItem;
    public List<Button> buttonList;
    //public Image item1image;
    public Text item1Name;
    public Text item1Cost;

    //public Image item2image;
    public Text item2Name;
    public Text item2Cost;
    //public Image item3image;
    public Text item3Name;
    public Text item3Cost;
    public Text itemEx;
    public Text nowMoney;
    
    private void Awake() {
        itemList = new ItemListData();
        selectedItem = new List<int>();
        nowMoney.text = "보유머니 : "+InventoryManager.Instance.inventory.money.ToString();
        int randomIndex;
        for (int i = 0; i < 3; i++)
        {
            randomIndex = UnityEngine.Random.Range(0, itemList.items.Count);
            if(!InventoryManager.Instance.inventory.items.Exists(x => x == randomIndex)&&!selectedItem.Exists(x => x == randomIndex)){
                selectedItem.Add(randomIndex);
                continue;
            }
            i--;
        }
        Debug.Log("첫번째: "+selectedItem[0]+"두번째 : "+selectedItem[1]+"세번째 : " + selectedItem[2]);
    }

    private void Update()
    {
        // 플레이어가 상호작용 범위에 있을 때 Space 바를 눌렀는지 확인
        if (GameManager.Instance.IsPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            OpenStore();
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("포탈들어옴");
        Debug.Log(ItemListData.items[0].name);
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.IsPlayerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어가 포탈 범위에서 나갔을 때
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.IsPlayerInRange = false;
        }
    }
    private void OpenStore()
    {
        Debug.Log("상점 열림");
        Time.timeScale = 0;
        StoreWindow.SetActive(true);
        item1Name.text = itemList.items[selectedItem[0]].name;
        item1Cost.text = itemList.items[selectedItem[0]].price.ToString()+ " Gold";
        item2Name.text = itemList.items[selectedItem[1]].name;
        item2Cost.text = itemList.items[selectedItem[1]].price.ToString()+ " Gold";
        item3Name.text = itemList.items[selectedItem[2]].name;
        item3Cost.text = itemList.items[selectedItem[2]].price.ToString()+ " Gold";
        itemEx.text = "";

    }
    public void CloseStore(){
        Time.timeScale=1;
        StoreWindow.SetActive(false);
    }
    public void showItemEx(int buttonId){
        itemEx.text = itemList.items[selectedItem[buttonId]].explaination;
    }
    public void buyItem(int buyButtonId){
        if(InventoryManager.Instance.inventory.money<itemList.items[selectedItem[buyButtonId]].price){
            Debug.Log(InventoryManager.Instance.inventory.money);
            Debug.Log("돈없음");
        }
        else{
            InventoryManager.Instance.RemoveItem(5,itemList.items[selectedItem[buyButtonId]].price);
            buttonList[buyButtonId].interactable =false;
            InventoryManager.Instance.AddItem(7,itemList.items[selectedItem[buyButtonId]].id);
            nowMoney.text = "보유머니 : "+InventoryManager.Instance.inventory.money.ToString();
        }
    }
}
