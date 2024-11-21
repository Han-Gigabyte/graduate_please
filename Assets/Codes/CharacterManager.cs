using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; } // 싱글톤

    [Header("Character Data")]
    public CharacterData[] characters; // 캐릭터 데이터 배열
    public int currentCharacterIndex = 0; // 현재 선택된 캐릭터 인덱스

    [Header("UI Elements")]
    public GameObject characterInfoPanel; // 캐릭터 정보 패널
    public GameObject upgradePanel; // 업그레이드 패널
    public Image characterImage; // 캐릭터 이미지
    public Text characterNameText; // 캐릭터 이름 텍스트
    public Text descriptionText; // 캐릭터 설명 텍스트

    [Header("Upgrade UI Elements")]
    public Image upgradeCharacterImage; // 캐릭터 이미지
    public Text upgradeCharacterNameText; // 캐릭터 이름 텍스트
    public Text vitalityText; // 생명력 텍스트
    public Text powerText; // 파워 텍스트
    public Text agilityText; // 민첩 텍스트
    public Text luckText; // 행운 텍스트
    public Button increaseVitalityButton;
    public Button increasePowerButton;
    public Button increaseAgilityButton;
    public Button increaseLuckButton;

    [Header("Buttons")]
    public Button upgradeButton;
    public Button selectButton;
    public Button closeInfoButton;
    public Button closeUpgradeButton;

    [Header("Character Buttons")]
    public Button[] characterButtons; // 캐릭터 버튼 배열

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 이동 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 캐릭터 버튼 클릭 이벤트 연결
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i; // 로컬 변수로 복사
            characterButtons[i].onClick.AddListener(() => ShowCharacterInfo(index));
        }
        Debug.Log("Characters array length: " + characters.Length);

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

        // 버튼 동작 연결
        upgradeButton.onClick.AddListener(ShowUpgradePanel);
        selectButton.onClick.AddListener(OnSelectButtonClick);
        closeUpgradeButton.onClick.AddListener(CloseUpgradePanel);

        // 업그레이드 버튼 동작
        increaseVitalityButton.onClick.AddListener(IncreaseVitality);
        increasePowerButton.onClick.AddListener(IncreasePower);
        increaseAgilityButton.onClick.AddListener(IncreaseAgility);
        increaseLuckButton.onClick.AddListener(IncreaseLuck);

        // 초기 상태
        characterInfoPanel.SetActive(false);
        upgradePanel.SetActive(false);

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

    // 캐릭터 정보 패널 열기
    public void ShowCharacterInfo(int index)
    {
        currentCharacterIndex = index;
        CharacterData character = characters[index];

        characterImage.sprite = character.characterSprite;
        characterNameText.text = character.characterName;
        descriptionText.text = character.description;

        characterInfoPanel.SetActive(true);
    }

    // 캐릭터 정보 패널 닫기
    public void CloseInfoPanel()
    {
        characterInfoPanel.SetActive(false);
    }

    // 업그레이드 패널 열기
    public void ShowUpgradePanel()
    {
        CharacterData character = characters[currentCharacterIndex];

        upgradeCharacterImage.sprite = character.characterSprite;
        upgradeCharacterNameText.text = character.characterName;

        // 현재 상태 표시
        vitalityText.text = $"VIT: {character.vitality}";
        powerText.text = $"POW: {character.power}";
        agilityText.text = $"AGI: {character.agility}";
        luckText.text = $"LUK: {character.luck}";

        characterInfoPanel.SetActive(false);
        upgradePanel.SetActive(true);
        
        // 업그레이드 창에 동일하게 표시
        upgradeNameText.text = character.characterName;
        upgradeLevelText.text = "Level: " + character.level;
        vitalityText.text = "VIT: " + character.vitality;
        powerText.text = "POW: " + character.power;
        agilityText.text = "AGI: " + character.agility;
        luckText.text = "LUK: " + character.luck;
        currentCharacterIndex = index;
    }

    // 업그레이드 패널 닫기
    public void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);
        characterInfoPanel.SetActive(true);
    }

    // 캐릭터 선택 -> 게임 씬으로 이동
    public void OnSelectButtonClick()
    {
        CharacterData character = characters[currentCharacterIndex];
        CharacterSelectionData.Instance.selectedCharacterSprite = character.characterSprite;

        SceneManager.LoadScene("GameScene");
    }


    // 스탯 증가 기능
    public void IncreaseVitality()
    {
        CharacterData character = characters[currentCharacterIndex];
        if (character.vitality < 5)
        {
            character.vitality++;
            vitalityText.text = $"VIT: {character.vitality}";
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
            powerText.text = $"POW: {character.power}";
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
            agilityText.text = $"AGI: {character.agility}";
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
            luckText.text = $"LUK: {character.luck}";
        }
    }
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
