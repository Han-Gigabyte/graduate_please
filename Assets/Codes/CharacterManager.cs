using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterManager : MonoBehaviour
{
    public CharacterData[] characters;        // 캐릭터 데이터 배열
    public Text characterNameText;            // 캐릭터 이름 텍스트
    public Text descriptionText;              // 캐릭터 설명 텍스트
    public Text levelText;                    // 캐릭터 레벨 텍스트

    public GameObject characterInfoPanel;     // 캐릭터 정보 패널
    public GameObject upgradePanel;           // 업그레이드 패널

    // 업그레이드 패널 UI 요소
    public Text upgradeNameText;
    public Text upgradeLevelText;
    public Text vitalityText;
    public Text powerText;
    public Text agilityText;
    public Text luckText;

    public Button characterButton1;
    public Button characterButton2;
    public Button characterButton3;
    public Button characterButton4;
    public Button characterButton5;
    public Button characterButton6;
    public Button characterButton7;
    public Button backButton;
    public Button upgradeButton;
    public Button increaseVitalityButton;     // 생명력 + 버튼
    public Button increasePowerButton;        // 파워 + 버튼
    public Button increaseAgilityButton;      // 민첩 + 버튼
    public Button increaseLuckButton;         // 행운 + 버튼
    public Button closeUpgradeButton;         // 업그레이드 창 닫기 버튼

    private int currentCharacterIndex = 0;

    private void Start()
    {
        characterInfoPanel.SetActive(false);
        upgradePanel.SetActive(false);

        characterButton1.onClick.AddListener(() => ShowCharacterInfo(0));
        characterButton2.onClick.AddListener(() => ShowCharacterInfo(1));
        characterButton3.onClick.AddListener(() => ShowCharacterInfo(2));
        characterButton4.onClick.AddListener(() => ShowCharacterInfo(3));
        characterButton5.onClick.AddListener(() => ShowCharacterInfo(4));
        characterButton6.onClick.AddListener(() => ShowCharacterInfo(5));
        characterButton7.onClick.AddListener(() => ShowCharacterInfo(6));

        backButton.onClick.AddListener(HideCharacterInfo);
        upgradeButton.onClick.AddListener(ShowUpgradePanel);

        // + 버튼에 각각의 증가 함수 연결
        increaseVitalityButton.onClick.AddListener(IncreaseVitality);
        increasePowerButton.onClick.AddListener(IncreasePower);
        increaseAgilityButton.onClick.AddListener(IncreaseAgility);
        increaseLuckButton.onClick.AddListener(IncreaseLuck);

        closeUpgradeButton.onClick.AddListener(CloseUpgradePanel); // 업그레이드 창 닫기 버튼에 이벤트 추가
    }

    // Select 버튼 클릭
    public void OnSelectButtonClick()
    {
        SceneManager.LoadScene("GameScene");  // "GameScene"는 인게임 씬의 이름
    }

    public void ShowCharacterInfo(int index)
    {
        characterInfoPanel.SetActive(true);
        characterButton1.interactable = false;
        characterButton2.interactable = false;
        characterButton3.interactable = false;
        characterButton4.interactable = false;
        characterButton5.interactable = false;
        characterButton6.interactable = false;
        characterButton7.interactable = false;

        LoadCharacter(index);
    }

    public void HideCharacterInfo()
    {
        characterInfoPanel.SetActive(false);
        characterButton1.interactable = true;
        characterButton2.interactable = true;
        characterButton3.interactable = true;
        characterButton4.interactable = true;
        characterButton5.interactable = true;
        characterButton6.interactable = true;
        characterButton7.interactable = true;
    }

    public void LoadCharacter(int index)
    {
        CharacterData character = characters[index];
        characterNameText.text = character.characterName;
        descriptionText.text = character.description;
        levelText.text = "Level: " + character.level;

        // 업그레이드 창에 동일하게 표시
        upgradeNameText.text = character.characterName;
        upgradeLevelText.text = "Level: " + character.level;
        currentCharacterIndex = index;
    }

    public void ShowUpgradePanel()
    {
        characterInfoPanel.SetActive(false); // 캐릭터 정보 창 숨기기
        upgradePanel.SetActive(true);
        LoadUpgradePanel();
    }

    public void LoadUpgradePanel()
    {
        CharacterData character = characters[currentCharacterIndex];
        upgradeNameText.text = character.characterName;
        upgradeLevelText.text = "Level: " + character.level;
        vitalityText.text = "VIT: " + character.vitality;
        powerText.text = "POW: " + character.power;
        agilityText.text = "AGI: " + character.agility;
        luckText.text = "LUK: " + character.luck;
    }

    public void IncreaseVitality()
    {
        CharacterData character = characters[currentCharacterIndex];
        if (character.vitality < 5)
        {
            character.vitality++;
            vitalityText.text = "VIT: " + character.vitality;
        }
    }

    public void IncreasePower()
    {
        CharacterData character = characters[currentCharacterIndex];
        if (character.power < 5)
        {
            character.power++;
            powerText.text = "POW: " + character.power;
        }
    }

    public void IncreaseAgility()
    {
        CharacterData character = characters[currentCharacterIndex];
        if (character.agility < 5)
        {
            character.agility++;
            agilityText.text = "AGI: " + character.agility;
        }
    }

    public void IncreaseLuck()
    {
        CharacterData character = characters[currentCharacterIndex];
        if (character.luck < 5)
        {
            character.luck++;
            luckText.text = "LUK: " + character.luck;
        }
    }

    public void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);       // 업그레이드 창 숨기기
        characterInfoPanel.SetActive(true);  // 캐릭터 정보 창 다시 표시
        LoadCharacter(currentCharacterIndex); // 현재 선택된 캐릭터 정보 다시 로드
    }
}
