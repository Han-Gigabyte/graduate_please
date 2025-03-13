using System.Collections;
using UnityEngine;

public class Throw : MonoBehaviour
{
    public float throwForce = 10f;      // 던지는 힘 (속도)
    public float arcHeight = 2.5f;      // 포물선 높이
    public int damage = 20;             // 독사과 데미지
    public LayerMask groundLayer;       // 땅 체크를 위한 레이어

    private Vector2 targetPosition;     // 목표 (플레이어 위치)
    private Vector2 startPosition;      // 시작 위치
    private float travelTime = 0.5f;    // 이동 시간
    private bool isThrown = false;

    public void setThrow(Vector2 target)
    {
        targetPosition = target;
        startPosition = transform.position;
        isThrown = true;
        StartCoroutine(ThrowCoroutine());
    }

    IEnumerator ThrowCoroutine()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / travelTime;

            // 포물선 궤적 계산
            float height = Mathf.Sin(t * Mathf.PI) * arcHeight;
            transform.position = Vector2.Lerp(startPosition, targetPosition, t) + Vector2.up * height;

            yield return null;
        }

        // 땅에 닿으면 사과 제거
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어가 맞았을 때
        {
            other.GetComponent<PlayerController>().TakeDamage(damage, Vector2.zero, 0f);
            Debug.Log("독사과 적중! 플레이어에게 " + damage + " 데미지를 입음");
            Destroy(gameObject);
        }

        // 땅에 닿으면 삭제
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}
