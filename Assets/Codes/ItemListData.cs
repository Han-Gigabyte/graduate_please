using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemList
{
    public int id;           // 아이템 고유 ID
    public string name;      // 아이템 이름
    public int price;        // 아이템 가격
    public string explaination;  // 아이템 설명

    // 생성자
    public ItemList(int id, string name, int price, string explaination)
    {
        this.id = id;
        this.name = name;
        this.price = price;
        this.explaination = explaination;
    }
}

public class ItemListData
{
    public List<ItemList> items = new List<ItemList>
    {
        new ItemList(0, "Steel_Cloak", 500, "대시 중 무적 효과 추가"),         // 강철 망토
        new ItemList(1, "Cloud_Boots", 400, "점프 후 1초정도 부유효과"),        // 구름 장화
        new ItemList(2, "Long_Sword", 700, "공격 데미지 15% 추가"),            // 롱소드
        new ItemList(3, "Knuckle", 600, "치명타 확률 5% 증가"),                // 너클
        new ItemList(4, "Plate_Armor", 800, "받는 데미지 15% 감소"),           // 플레이트 갑옷
        new ItemList(5, "Dumbbell", 550, "최대 체력 현재 체력의 15% 증가"),     // 덤벨
        new ItemList(6, "Dice", 300, "처치 시 재료 드랍 확률 5% 증가"),        // 주사위
        new ItemList(7, "Ice_Boots", 450, "이동 속도 15% 증가"),               // 얼음 장화
        new ItemList(8, "Premium_Oil", 200, "체력 25% 회복 (소모품)"),         // 고급 기름
        new ItemList(9, "Lubricant", 350, "공속 10% 증가"),                   // 윤활유
        new ItemList(10, "Shield", 750, "공격 1회 무효"),                     // 쉴드
        new ItemList(11, "Phoenix_HighGrade", 1000, "죽을 시 25% 체력으로 자동 부활 (1회)"), // 피닉스(고오급)
        new ItemList(12, "Thor_Hammer", 950, "치명타 적중 시 번개"),           // 토르의 망치
        new ItemList(13, "Thorn_Armor", 850, "피격 시 5% 공격반사"),           // 가시갑옷
        new ItemList(14, "Ice_Blade", 900, "일정 거리 내 적 둔화 (적 이동 속도 20% 감소)"), // 얼음 칼날
        new ItemList(15, "Spinning_Tornado", 1200, "35초마다 회오리 생성 (회오리로 적 쓸어버림)"), // 회전 회오리
        new ItemList(16, "Zeus_Lightning", 1500, "55초마다 해당 맵 절반만큼의 적에게 벼락 공격"), // 제우스의 번개
        new ItemList(17, "Berserker_Mask", 1100, "체력 비율이 30% 이하일 때 데미지 50% 증가")    // 광전사의 가면
    };
}