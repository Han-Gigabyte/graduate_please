using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro를 사용할 경우

public class PlayerUI : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Position Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, -85f, 0);

    private PlayerController player;
    private Camera mainCamera;
    private RectTransform rectTransform;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        
        offset = new Vector3(0, -85f, 0);
        
        if (player != null)
        {
            healthSlider.maxValue = GameManager.Instance.playerMaxHealth;
            healthSlider.value = player.CurrentHealth;
            
            if (healthText != null)
            {
                UpdateHealthText();
            }
        }
    }

    private void Update()
    {
        if (player != null && mainCamera != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(player.transform.position);
            rectTransform.position = screenPos + offset;
            
            healthSlider.value = player.CurrentHealth;
            
            if (healthText != null)
            {
                UpdateHealthText();
            }
        }
    }

    private void UpdateHealthText()
    {
        healthText.text = $"{player.CurrentHealth} / {GameManager.Instance.playerMaxHealth}";
    }
}
