using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularAttackEffect : MonoBehaviour
{
    public float radius = 3f; // 공격 범위
    public Color attackColor = Color.red; // 공격 범위 색상
    public float duration = 0.5f; // 이펙트 지속 시간

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // // SpriteRenderer 컴포넌트 추가
        // spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        // //spriteRenderer.sprite = Resources.Load<Sprite>("CircleSprite"); // 원형 스프라이트 로드
        // spriteRenderer.color = attackColor;
        // spriteRenderer.drawMode = SpriteDrawMode.Sliced;
        // spriteRenderer.size = new Vector2(radius * 2, radius * 2); // 크기 설정

        // 이펙트 지속 시간 후 제거
        Destroy(gameObject, duration);
    }
}
