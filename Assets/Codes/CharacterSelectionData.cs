using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionData : MonoBehaviour
{
    public static CharacterSelectionData Instance { get; private set; }

    public Sprite selectedCharacterSprite; // 선택된 캐릭터의 스프라이트

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // CharacterManager.Instance가 null인지 확인
            if (CharacterManager.Instance != null && CharacterManager.Instance.characters.Length > 0)
            {
                selectedCharacterSprite = CharacterManager.Instance.characters[0].characterSprite;
            }
            else
            {
                Debug.LogWarning("CharacterManager.Instance is null or characters array is empty. Default sprite not set.");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}


