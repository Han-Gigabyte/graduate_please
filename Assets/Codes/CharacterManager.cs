using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterManager : MonoBehaviour
{
    [Header("Character Data Info")]
    public CharacterData[] characters;        // 캐릭터 데이터 배열
    public Image characterImage;              // 캐릭터 이미지
    public Text characterNameText;            // 캐릭터 이름 텍스트
    public Text descriptionText;              // 캐릭터 설명 텍스트
    public Text levelText;                    // 캐릭터 레벨 텍스트
    public Text unlockConditionText;          // 캐릭터 해금조건 텍스트


    [Header("Panel Info")]
    public GameObject characterInfoPanel;     // 캐릭터 정보 패널
    public GameObject upgradePanel;           // 업그레이드 패널

    [Header("Upgrade Info")]
    public Image upgradeCharacterImage;
    public Text upgradeNameText;
    public Text upgradeLevelText;
    public Text vitalityText;
    public Text powerText;
    public Text agilityText;
    public Text luckText;

    [Header("Upgrade Buttons")]
    public Button vitalityUpgradeButton; // Vitality 업그레이드 버튼
    public Button powerUpgradeButton;     // Power 업그레이드 버튼
    public Button agilityUpgradeButton;   // Agility 업그레이드 버튼
    public Button luckUpgradeButton;      // Luck 업그레이드 버튼

    [Header("Buttons")]
    public Transform characterContainer;  // 캐릭터 버튼들이 들어갈 부모 컨테이너
    public Button backButton;
    public Button selectButton;
    public Button upgradeButton;
    public Button unlockButton;
    public Button closeUpgradeButton;         // 업그레이드 창 닫기 버튼

    private int currentCharacterIndex = 0;
    private List<Button> characterButtons = new List<Button>(); // 동적 버튼 리스트


    private void Start()
    {
        Debug.Log("Characters array length: " + characters.Length);

        characterInfoPanel.SetActive(false);
        upgradePanel.SetActive(false);

        // 캐릭터 데이터 초기화
        foreach (var character in characters)
        {
            int index = Array.IndexOf(characters, character);

            character.level = PlayerPrefs.GetInt("CharacterLevel_" + index, 1);
            character.vitality = PlayerPrefs.GetInt("CharacterVitality_" + index, 0);
            character.power = PlayerPrefs.GetInt("CharacterPower_" + index, 0);
            character.agility = PlayerPrefs.GetInt("CharacterAgility_" + index, 0);
            character.luck = PlayerPrefs.GetInt("CharacterLuck_" + index, 0);
        }

        unlockButton.onClick.AddListener(() => TryUnlockCharacter(currentCharacterIndex));
        backButton.onClick.AddListener(HideCharacterInfo);
        selectButton.onClick.AddListener(OnSelectButtonClick);
        upgradeButton.onClick.AddListener(ShowUpgradePanel);
        closeUpgradeButton.onClick.AddListener(CloseUpgradePanel);

        Button[] existingButtons = characterContainer.GetComponentsInChildren<Button>();

        if (existingButtons.Length != characters.Length)
        {
            Debug.LogError($"⚠️ 버튼 개수({existingButtons.Length})와 캐릭터 개수({characters.Length})가 맞지 않습니다!");
            return;
        }

        characterButtons = new List<Button>();

        for (int i = 0; i < existingButtons.Length; i++)
        {
            int index = i;
            Button button = existingButtons[i];

            Text buttonText = button.GetComponentInChildren<Text>(); // 버튼의 텍스트
            Image buttonImage = button.GetComponent<Image>(); // 버튼 자체의 이미지

            if (buttonText != null)
            {
                buttonText.text = characters[index].characterName;
            }

            // 캐릭터가 잠겨 있으면 버튼을 어둡게 처리
            if (!characters[index].isUnlocked)
            {
                buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); // 어두운 색상 적용
            }

            // 버튼 클릭 이벤트 추가
            button.onClick.AddListener(() => ShowCharacterInfo(index));

            // 리스트에 추가
            characterButtons.Add(button);
        }
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
        LoadCharacter(currentCharacterIndex); // 캐릭터 정보 다시 로드

        LoadUpgradePanel();
    }

    // Select 버튼 클릭
    public void OnSelectButtonClick()
    {
        if (!characters[currentCharacterIndex].isUnlocked)
        {
            Debug.Log("이 캐릭터는 잠겨 있습니다!");
            return;
        }

        // 선택된 캐릭터의 데이터를 CharacterSelectionData에 저장
        CharacterData selectedCharacter = characters[currentCharacterIndex];
        CharacterSelectionData.Instance.selectedCharacterSprite = selectedCharacter.characterSprite;

        // 게임 씬으로 이동
        SceneManager.LoadScene("GameScene");
    }

    public void HideCharacterInfo()
    {
        characterInfoPanel.SetActive(false);
        SetCharacterButtonsInteractable(true);
    }
    
    public void ShowCharacterInfo(int index)
    {
        CharacterData character = characters[currentCharacterIndex];
        characterInfoPanel.SetActive(true);
        SetCharacterButtonsInteractable(false);
        LoadCharacter(index);

        if (characters[index].isUnlocked)
        {
            selectButton.interactable = true;
            upgradeButton.interactable = true;
            unlockButton.gameObject.SetActive(false); // 해금 버튼 숨김
        }
        else
        {
            selectButton.interactable = false;
            upgradeButton.interactable = false;
            unlockButton.gameObject.SetActive(true); // 해금 버튼 표시
            unlockConditionText.text = $"필요한 기계조각 : {character.requiredScrews}\n필요한 페이지 : {character.requiredPages}";

        }
    }

    public void TryUnlockCharacter(int index)
    {
        CharacterData character = characters[index];

        if (InventoryManager.Instance.inventory.page >= character.requiredPages && InventoryManager.Instance.inventory.screw >= character.requiredScrews)
        {
            InventoryManager.Instance.inventory.page -= character.requiredPages;
            InventoryManager.Instance.inventory.screw -= character.requiredScrews;

            character.isUnlocked = true;
            Debug.Log($"{character.characterName}이(가) 해금되었습니다!");

            UnlockCharacter(index);
            ShowCharacterInfo(index); // UI 갱신
        }
        else
        {
            Debug.Log("재료가 부족합니다!");
        }
    }

    private void UnlockCharacter(int index)
    { 
        Button[] Buttons = characterContainer.GetComponentsInChildren<Button>();
        Button characterButton = Buttons[index];

        Image buttonImage = characterButton.GetComponent<Image>();
        buttonImage.color = Color.white;

        PlayerPrefs.SetInt("CharacterUnlocked_" + index, 1);
        PlayerPrefs.Save();
    }

    public void LoadCharacter(int index)
    {
        CharacterData character = characters[index];
        characterImage.sprite = character.characterSprite;
        characterImage.color = character.isUnlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);

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

        // 레벨 값의 -1과 비교
        int totalIncrease = character.vitality + character.power + character.agility + character.luck;

        if (totalIncrease == character.level - 1)
        {
            // 모든 버튼 비활성화
            vitalityUpgradeButton.interactable = false;
            powerUpgradeButton.interactable = false;
            agilityUpgradeButton.interactable = false;
            luckUpgradeButton.interactable = false;
        }
        else if (totalIncrease < character.level - 1)
        {
            // 각 속성의 증가량이 5인 경우 해당 버튼 비활성화
            vitalityUpgradeButton.interactable = character.vitality < 5;
            powerUpgradeButton.interactable = character.power < 5;
            agilityUpgradeButton.interactable = character.agility < 5;
            luckUpgradeButton.interactable = character.luck < 5;
        }

        Debug.Log($"Total Increase: {totalIncrease}, Level: {character.level}, Vitality: {character.vitality}, Power: {character.power}, Agility: {character.agility}, Luck: {character.luck}");
    }

    public void ShowUpgradePanel()
    {
        characterInfoPanel.SetActive(false); // 캐릭터 정보 창 숨기기
        upgradePanel.SetActive(true);
        SetCharacterButtonsInteractable(false);
        LoadUpgradePanel();
    }

    public void LoadUpgradePanel()
    {
        CharacterData character = characters[currentCharacterIndex];
        upgradeCharacterImage.sprite = character.characterSprite;
        upgradeNameText.text = character.characterName;
        upgradeLevelText.text = "Level: " + character.level;
        vitalityText.text = "VIT: " + character.vitality;
        powerText.text = "POW: " + character.power;
        agilityText.text = "AGI: " + character.agility;
        luckText.text = "LUK: " + character.luck;

        // 레벨 값의 -1과 비교
        int totalIncrease = character.vitality + character.power + character.agility + character.luck;

        if (totalIncrease == character.level - 1)
        {
            // 모든 버튼 비활성화
            vitalityUpgradeButton.interactable = false;
            powerUpgradeButton.interactable = false;
            agilityUpgradeButton.interactable = false;
            luckUpgradeButton.interactable = false;
        }
        else if (totalIncrease < character.level - 1)
        {
            // 각 속성의 증가량이 5인 경우 해당 버튼 비활성화
            vitalityUpgradeButton.interactable = character.vitality < 5;
            powerUpgradeButton.interactable = character.power < 5;
            agilityUpgradeButton.interactable = character.agility < 5;
            luckUpgradeButton.interactable = character.luck < 5;
        }

        Debug.Log($"Total Increase: {totalIncrease}, Level: {character.level}, Vitality: {character.vitality}, Power: {character.power}, Agility: {character.agility}, Luck: {character.luck}");
    }

    public void IncreaseVitality()
    {
        CharacterData character = characters[currentCharacterIndex];
        if (character.vitality < 5)
        {
            character.vitality++;
            vitalityText.text = "VIT: " + character.vitality;
            SaveCharacterStats(); // 캐릭터 속성 저장
        }

        LoadUpgradePanel();
    }

    public void IncreasePower()
    {
        CharacterData character = characters[currentCharacterIndex];
        if (character.power < 5)
        {
            character.power++;
            powerText.text = "POW: " + character.power;
            SaveCharacterStats(); // 캐릭터 속성 저장
        }

        LoadUpgradePanel();
    }

    public void IncreaseAgility()
    {
        CharacterData character = characters[currentCharacterIndex];
        if (character.agility < 5)
        {
            character.agility++;
            agilityText.text = "AGI: " + character.agility;
            SaveCharacterStats(); // 캐릭터 속성 저장
        }

        LoadUpgradePanel();
    }

    public void IncreaseLuck()
    {
        CharacterData character = characters[currentCharacterIndex];
        if (character.luck < 5)
        {
            character.luck++;
            luckText.text = "LUK: " + character.luck;
            SaveCharacterStats(); // 캐릭터 속성 저장
        }

        LoadUpgradePanel();
    }

    public void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);       // 업그레이드 창 숨기기
        characterInfoPanel.SetActive(true);  // 캐릭터 정보 창 다시 표시
        LoadCharacter(currentCharacterIndex); // 현재 선택된 캐릭터 정보 다시 로드
    }

    private void SetCharacterButtonsInteractable(bool state)
    {
        foreach (Button button in characterButtons)
        {
            button.interactable = state;
        }
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

    
}
