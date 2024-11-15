using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    private float detectionRange;
    
    [Header("Ground Check")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    [Header("Collision")]
    public CompositeCollider2D compositeCollider;

    [Header("Attack")]
    private int attackDamage;
    private float knockbackForce;
    private float damageCooldown;
    private float nextDamageTime;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Transform playerTransform;
    private bool isGrounded;
    private bool isFacingRight = true;

    void Start()
    {
        // GameManager에서 값 가져오기
        moveSpeed = GameManager.Instance.meleeEnemyMoveSpeed;
        detectionRange = GameManager.Instance.meleeEnemyDetectionRange;
        attackDamage = GameManager.Instance.meleeEnemyDamage;
        knockbackForce = GameManager.Instance.meleeEnemyKnockbackForce;
        damageCooldown = GameManager.Instance.meleeEnemyDamageCooldown;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 2.5f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;  // 연속 충돌 감지 모드로 변경
        
        // 기존 Collider는 물리적 충돌용으로 사용
        GetComponent<Collider2D>().isTrigger = false;
        
        // 새로운 Trigger Collider 추가
        BoxCollider2D triggerCollider = gameObject.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
        // 기존 Collider와 같은 크기로 설정
        triggerCollider.size = GetComponent<BoxCollider2D>().size;
        triggerCollider.offset = GetComponent<BoxCollider2D>().offset;
        
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        
        // 씬에 있는 모든 Enemy들과의 충돌을 무시
        RangedEnemy[] rangedEnemies = FindObjectsOfType<RangedEnemy>();
        MeleeEnemy[] meleeEnemies = FindObjectsOfType<MeleeEnemy>();
        
        foreach (var enemy in meleeEnemies)
        {
            if (enemy != this)  // 자기 자신은 제외
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), enemy.GetComponent<Collider2D>(), true);
            }
        }
        
        foreach (var enemy in rangedEnemies)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), enemy.GetComponent<Collider2D>(), true);
        }
    }

    void Update()
    {
        // 플레이어가 죽었거나 없으면 더 이상 진행하지 않음
        if (PlayerController.IsDead || playerTransform == null)
        {
            // 정지
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        // 플레이어와의 거리 체크
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        
        if (distanceToPlayer <= detectionRange)
        {
            // 플레이어 방향으로 이동
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            
            // x축 방향으로만 이동
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            
            // 스프라이트 방향 전환
            if (direction.x > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (direction.x < 0 && isFacingRight)
            {
                Flip();
            }
        }
        else
        {
            // 플레이어가 감지 범위를 벗어나면 정지
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        spriteRenderer.flipX = !isFacingRight;
    }

    // 디버그용 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    // 새로 스폰되는 Enemy들과도 충돌을 무시하기 위한 트리거 체크
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other, true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= nextDamageTime)
            {
                IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                    knockbackDir.y = 0.5f;
                    
                    damageable.TakeDamage(attackDamage, knockbackDir, knockbackForce);
                    nextDamageTime = Time.time + damageCooldown;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= nextDamageTime)
            {
                IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                    knockbackDir.y = 0.5f;
                    
                    damageable.TakeDamage(attackDamage, knockbackDir, knockbackForce);
                    nextDamageTime = Time.time + damageCooldown;
                }
            }
        }
    }
}
