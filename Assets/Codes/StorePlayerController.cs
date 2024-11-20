using UnityEngine;

public class StorePlayerController : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public int maxHealth = 100; // 최대 체력
    private int currentHealth;

    private void Start()
    {
        // GameManager에서 플레이어 상태 복원
        currentHealth = GameManager.Instance.CurrentPlayerHealth; // GameManager에서 현재 체력 가져오기
        Debug.Log($"Player health restored to: {currentHealth}");
    }

    public void RestoreHealth(int health)
    {
        currentHealth = Mathf.Min(maxHealth, health);
        Debug.Log($"Store player health restored to: {currentHealth}");
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

        if (knockbackDirection != Vector2.zero)
        {
            // 넉백 로직 추가 (예: Rigidbody2D를 사용하여 넉백 적용)
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // 사망 처리 로직 추가 (예: 게임 오버 화면 표시 등)
    }
}