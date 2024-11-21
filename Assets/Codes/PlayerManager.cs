using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public SpriteRenderer playerSpriteRenderer;

    private void Start()
    {
        if (CharacterSelectionData.Instance == null || CharacterSelectionData.Instance.selectedCharacterSprite == null)
        {
            Debug.LogError("CharacterSelectionData or selectedCharacterSprite is null.");
            return;
        }

        // 선택된 스프라이트를 플레이어에게 적용
        playerSpriteRenderer.sprite = CharacterSelectionData.Instance.selectedCharacterSprite;
    }
}
