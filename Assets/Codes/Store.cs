using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Store : MonoBehaviour
{
    public float interactionRange = 2f;
    List<int> selectedItem;
    private int[] prePrice;
    private int[] nowPrice;
    [Header("Store Object")]
    public GameObject StoreWindow;
    ItemListData itemList;
    PlayerItemData inventory;
    [Header("Item Object")]
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
    [Header("Ingredient Object")]
    public Text Stone;
    public Text Tree;
    public Text Skin;
    public Text Steel;
    public Text Gold;

    [Header("Ingredient Price Data")]
    public List<Text> prePriceData; // 돌, 나무, 가죽, 철, 금 순서
    public List<Text> nowPriceData;
    public List<Text> changeRate;


    
    private void Awake() {
        itemList = new ItemListData();
        selectedItem = new List<int>();
        prePrice = new int[5];
        nowPrice = new int[5];
        nowMoney.text = "보유머니 : "+InventoryManager.Instance.inventory.money.ToString();
        
        int randomIndex;
        for (int i = 0; i < 3; i++)
        {
            randomIndex = UnityEngine.Random.Range(0, ItemListData.items.Count);
            if(!InventoryManager.Instance.inventory.items.Exists(x => x == randomIndex)&&!selectedItem.Exists(x => x == randomIndex)){
                selectedItem.Add(randomIndex);
                continue;
            }
            i--;
        }
        item1Name.text = ItemListData.items[selectedItem[0]].name;
        item1Cost.text = ItemListData.items[selectedItem[1]].name;
        item2Cost.text = ItemListData.items[selectedItem[1]].price.ToString()+ " Gold";
        item3Name.text = ItemListData.items[selectedItem[2]].name;
        item3Cost.text = ItemListData.items[selectedItem[2]].price.ToString()+ " Gold";
        stockUpdate();
        Stone.text = InventoryManager.Instance.inventory.stone+"개 소유";
        Tree.text = InventoryManager.Instance.inventory.tree+"개 소유";
        Skin.text = InventoryManager.Instance.inventory.skin+"개 소유";
        Steel.text = InventoryManager.Instance.inventory.steel+"개 소유";
        Gold.text = InventoryManager.Instance.inventory.gold+"개 소유";
        Debug.Log("첫번째: "+selectedItem[0]+"두번째 : "+selectedItem[1]+"세번째 : " + selectedItem[2]);



    }
    private void stockUpdate(){
        prePrice[0] = InventoryManager.Instance.inventory.stonePrice;
        prePrice[1] = InventoryManager.Instance.inventory.treePrice;
        prePrice[2] = InventoryManager.Instance.inventory.skinPrice;
        prePrice[3] = InventoryManager.Instance.inventory.steelPrice;
        prePrice[4] = InventoryManager.Instance.inventory.goldPrice;
        for(int i=0;i<5;i++){
            int randomInt = UnityEngine.Random.Range(1, 6); //1~5로 등락률을 정함. 1은 하한가, 5는 상한가, 3는 그대로
            nowPrice[i] = prePrice[i]+InventoryManager.Instance.inventory.stockUpdateData[i][randomInt-1]; //가격 업데이트
            if(nowPrice[i]<InventoryManager.Instance.inventory.itemPriceRange[i,0]){
                nowPrice[i] = InventoryManager.Instance.inventory.itemPriceRange[i,0];
            }
            else if(nowPrice[i]>InventoryManager.Instance.inventory.itemPriceRange[i,1]){
                nowPrice[i] = InventoryManager.Instance.inventory.itemPriceRange[i,1];
            }
            changeRate[i].text = "(등락수치 : "+(nowPrice[i]-prePrice[i]).ToString()+")";
            if(nowPrice[i]-prePrice[i]>0){
                changeRate[i].color = Color.red;
            }
            else if(nowPrice[i]-prePrice[i]<0){
                changeRate[i].color = Color.blue;
            }
            else{
                changeRate[i].color = Color.gray;
            }
            prePriceData[i].text = "저번 라운드 가격 : "+prePrice[i].ToString();
            nowPriceData[i].text = "이번 라운드 가격 : "+nowPrice[i].ToString();
        }
        //가격 갱신
        InventoryManager.Instance.inventory.stonePrice=nowPrice[0];
        InventoryManager.Instance.inventory.treePrice= nowPrice[1];
        InventoryManager.Instance.inventory.skinPrice= nowPrice[2];
        InventoryManager.Instance.inventory.steelPrice= nowPrice[3];
        InventoryManager.Instance.inventory.goldPrice= nowPrice[4];
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
        itemEx.text = "";

    }
    public void sellItem(int buttonId){
        switch(buttonId){
            case 0:
            InventoryManager.Instance.inventory.money += InventoryManager.Instance.inventory.stone*nowPrice[buttonId];
            InventoryManager.Instance.inventory.stone=0;
            Stone.text = "0개 소유";
            break;
            case 1:
            InventoryManager.Instance.inventory.money += InventoryManager.Instance.inventory.tree*nowPrice[buttonId];
            InventoryManager.Instance.inventory.tree=0;
            Tree.text = "0개 소유";
            break;
            case 2:
            InventoryManager.Instance.inventory.money += InventoryManager.Instance.inventory.skin*nowPrice[buttonId];
            InventoryManager.Instance.inventory.skin=0;
            Skin.text = "0개 소유";
            break;
            case 3:
            InventoryManager.Instance.inventory.money += InventoryManager.Instance.inventory.steel*nowPrice[buttonId];
            InventoryManager.Instance.inventory.steel=0;
            Steel.text = "0개 소유";
            break;
            case 4:
            InventoryManager.Instance.inventory.money += InventoryManager.Instance.inventory.gold*nowPrice[buttonId];
            InventoryManager.Instance.inventory.gold=0;
            Gold.text = "0개 소유";
            break;
        }
        nowMoney.text = "보유 머니 : "+ InventoryManager.Instance.inventory.money.ToString();
    }
    public void CloseStore(){
        Time.timeScale=1;
        StoreWindow.SetActive(false);
    }
    public void showItemEx(int buttonId){
        itemEx.text = ItemListData.items[selectedItem[buttonId]].explaination;
    }
    public void buyItem(int buyButtonId){
        if(InventoryManager.Instance.inventory.money< ItemListData.items[selectedItem[buyButtonId]].price){
            Debug.Log(InventoryManager.Instance.inventory.money);
            Debug.Log("돈없음");
        }
        else{
            InventoryManager.Instance.RemoveItem(5, ItemListData.items[selectedItem[buyButtonId]].price);
            buttonList[buyButtonId].interactable =false;
            InventoryManager.Instance.AddItem(7, ItemListData.items[selectedItem[buyButtonId]].id);
            nowMoney.text = "보유머니 : "+InventoryManager.Instance.inventory.money.ToString();
        }
    }
}
