using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private int damage;
    private float speed;
    private Vector2 moveDirection;

    public void SetFireball(int _damage, float _speed, Vector2 _targetPosition)
    {
        damage = _damage;
        speed = _speed;

        // 타겟 방향 설정 (보스 → 플레이어)
        moveDirection = (_targetPosition - (Vector2)transform.position).normalized;
    }

    private void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().TakeDamage(damage, moveDirection, 10f);
            Debug.Log($"{collision.name}이(가) {damage} 데미지를 입음!");
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}