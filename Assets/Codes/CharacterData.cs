using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;  // 캐릭터 이름
    public Sprite characterSprite; // 캐릭터 이미지
    public int level = 1;         // 레벨 (초기값을 1로 설정)
    public string description;    // 설명
    public int vitality;          // 생명력
    public int power;             // 파워
    public int agility;           // 민첩
    public int luck;              // 행운
}

