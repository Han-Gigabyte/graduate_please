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
        }
    }

    public void IncreasePower()
    {
        CharacterData character = characters[currentCharacterIndex];
        if (character.power < 5)
        {
            character.power++;
            powerText.text = $"POW: {character.power}";
        }
    }

    public void IncreaseAgility()
    {
        CharacterData character = characters[currentCharacterIndex];
        if (character.agility < 5)
        {
            character.agility++;
            agilityText.text = $"AGI: {character.agility}";
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
}
