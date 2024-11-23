using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionData : MonoBehaviour
{
    public static CharacterSelectionData Instance { get; private set; }

    public Sprite selectedCharacterSprite; // 선택된 캐릭터의 스프라이트 저장

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 오브젝트 유지

            // 초기값 설정: 첫 번째 캐릭터의 스프라이트를 기본값으로 사용
            SetDefaultCharacterSprite();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetDefaultCharacterSprite()
    {
        // CharacterManager에서 데이터를 가져옴
        CharacterManager characterManager = FindObjectOfType<CharacterManager>();

        if (characterManager != null && characterManager.characters.Length > 0)
        {
            selectedCharacterSprite = characterManager.characters[0].characterSprite;
            Debug.Log("Default character sprite set to: " + selectedCharacterSprite.name);
        }
        else
        {
            Debug.LogWarning("CharacterManager not found or no characters available!");
        }
    }
}

