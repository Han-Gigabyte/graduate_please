using System;
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

        // 캐릭터 데이터 초기화
        foreach (var character in characters)
        {
            // PlayerPrefs에서 레벨 및 속성 불러오기
            int savedLevel = PlayerPrefs.GetInt("CharacterLevel_" + Array.IndexOf(characters, character), 1); // 기본값 1
            character.level = savedLevel; // 저장된 레벨로 초기화

            character.vitality = PlayerPrefs.GetInt("CharacterVitality_" + Array.IndexOf(characters, character), 0); // 기본값 0
            character.power = PlayerPrefs.GetInt("CharacterPower_" + Array.IndexOf(characters, character), 0); // 기본값 0
            character.agility = PlayerPrefs.GetInt("CharacterAgility_" + Array.IndexOf(characters, character), 0); // 기본값 0
            character.luck = PlayerPrefs.GetInt("CharacterLuck_" + Array.IndexOf(characters, character), 0); // 기본값 0
        }

        // 버튼 리스너 설정
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

        // 초기 버튼 상태 설정
        UpdateUpgradeButtonStates();
    }

    private void Update()
    {
        // 키보드의 `키`를 눌렀을 때 현재 선택된 캐릭터의 레벨을 증가
        if (Input.GetKeyDown(KeyCode.BackQuote)) // `키는 BackQuote로 표현
        {
            IncreaseCharacterLevel();
        }
    }

    private void IncreaseCharacterLevel()
    {
        CharacterData character = characters[currentCharacterIndex];
        character.level++; // 레벨 증가
        SaveCharacterStats(); // 변경된 레벨 저장
        UpdateUpgradeButtonStates(); // 버튼 상태 업데이트
        LoadCharacter(currentCharacterIndex); // 캐릭터 정보 다시 로드
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
        vitalityText.text = "VIT: " + character.vitality;
        powerText.text = "POW: " + character.power;
        agilityText.text = "AGI: " + character.agility;
        luckText.text = "LUK: " + character.luck;
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
            UpdateUpgradeButtonStates(); // 버튼 상태 업데이트
            SaveCharacterStats(); // 캐릭터 속성 저장
        }
    }

    public void IncreasePower()
    {
        CharacterData character = characters[currentCharacterIndex];
        if (character.power < 5)
        {
            character.power++;
            powerText.text = "POW: " + character.power;
            UpdateUpgradeButtonStates(); // 버튼 상태 업데이트
            SaveCharacterStats(); // 캐릭터 속성 저장
        }
    }

    public void IncreaseAgility()
    {
        CharacterData character = characters[currentCharacterIndex];
        if (character.agility < 5)
        {
            character.agility++;
            agilityText.text = "AGI: " + character.agility;
            UpdateUpgradeButtonStates(); // 버튼 상태 업데이트
            SaveCharacterStats(); // 캐릭터 속성 저장
        }
    }

    public void IncreaseLuck()
    {
        CharacterData character = characters[currentCharacterIndex];
        if (character.luck < 5)
        {
            character.luck++;
            luckText.text = "LUK: " + character.luck;
            UpdateUpgradeButtonStates(); // 버튼 상태 업데이트
            SaveCharacterStats(); // 캐릭터 속성 저장
        }
    }

    public void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);       // 업그레이드 창 숨기기
        characterInfoPanel.SetActive(true);  // 캐릭터 정보 창 다시 표시
        LoadCharacter(currentCharacterIndex); // 현재 선택된 캐릭터 정보 다시 로드
    }

    // 캐릭터 속성을 PlayerPrefs에 저장하는 메서드
    private void SaveCharacterStats()
    {
        CharacterData character = characters[currentCharacterIndex];
        PlayerPrefs.SetInt("CharacterLevel_" + currentCharacterIndex, character.level);
        PlayerPrefs.SetInt("CharacterVitality_" + currentCharacterIndex, character.vitality);
        PlayerPrefs.SetInt("CharacterPower_" + currentCharacterIndex, character.power);
        PlayerPrefs.SetInt("CharacterAgility_" + currentCharacterIndex, character.agility);
        PlayerPrefs.SetInt("CharacterLuck_" + currentCharacterIndex, character.luck);
        PlayerPrefs.Save(); // 변경 사항 저장
    }

    private void UpdateUpgradeButtonStates()
    {
        CharacterData character = characters[currentCharacterIndex];
        int totalStats = character.vitality + character.power + character.agility + character.luck;

        // 현재 캐릭터의 레벨 - 1과 비교
        bool canUpgrade = totalStats < character.level - 1;

        // 버튼 활성화 상태 설정
        increaseVitalityButton.interactable = canUpgrade;
        increasePowerButton.interactable = canUpgrade;
        increaseAgilityButton.interactable = canUpgrade;
        increaseLuckButton.interactable = canUpgrade;
    }
}
