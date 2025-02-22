using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f; // 투사체 속도
    public int damage = 10; // 투사체 데미지

    private Vector3 direction; // 투사체의 이동 방향

    public void Initialize(Vector3 direction, int damage)
    {
        this.direction = direction.normalized; // 방향을 정규화하여 저장
        this.damage = damage; // 데미지 설정
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 투사체 이동
        transform.position += direction * speed * Time.deltaTime; // 방향과 속도를 곱하여 위치 업데이트
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 적에게 데미지 적용
            Debug.Log($"Dealing {damage} damage to {other.name}.");
            // 적의 데미지 처리 로직 추가

            // 투사체 비활성화 또는 풀로 반환
            gameObject.SetActive(false); // 비활성화하여 풀로 반환
        }
    }
}
