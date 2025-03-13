using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public float maxSize = 15.0f;      // 최대 크기 증가
    public float expandSpeed = 15.0f;  // 확장 속도 증가
    public int damage = 20;            // 충격파 데미지
    public float duration = 5f;        // 지속 시간 단축
    private float timer = 0f;
    private BoxCollider2D col;

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        col.enabled = false;

        // 보스 발밑에서 시작 (y 좌표 조정)
        transform.position = new Vector2(transform.position.x, transform.position.y - 1.3f);
        transform.localScale = new Vector3(1f, 0.5f, 1f);
    }

    void Update()
    {
        transform.localScale += new Vector3(expandSpeed * Time.deltaTime, 0, 0);

        // 충돌 판정 활성화
        if (transform.localScale.x >= maxSize * 0.5f)
        {
            col.enabled = true;
        }

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log($"지진파 명중! {damage} 피해를 입음");
                player.TakeDamage(damage, Vector2.zero, 0f);
            }
        }
    }
}
