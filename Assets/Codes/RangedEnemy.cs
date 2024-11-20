using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    // Movement
    private float moveSpeed;
    private float attackRange;
    private float detectionRange;

    protected Transform PlayerTransform { get; private set; }
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isPlayerInRange = false;

    // Attack
    protected int AttackDamage { get; private set; }
    protected float ProjectileSpeed { get; private set; }
    private float attackCooldown;
    protected string projectileKey = "EnemyProjectile";
    protected Transform firePoint;

    private float nextAttackTime;

    void Start()
    {
        // GameManager에서 값 가져오기
        moveSpeed = GameManager.Instance.rangedEnemyMoveSpeed;
        attackRange = GameManager.Instance.rangedEnemyAttackRange;
        detectionRange = GameManager.Instance.rangedEnemyDetectionRange;
        AttackDamage = GameManager.Instance.rangedEnemyDamage;
        attackCooldown = GameManager.Instance.rangedEnemyAttackCooldown;
        ProjectileSpeed = GameManager.Instance.rangedEnemyProjectileSpeed;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        PlayerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        // Rigidbody2D 설정
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 2.5f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        // 기존 Collider는 물리적 충돌용으로 사용
        BoxCollider2D existingCollider = GetComponent<BoxCollider2D>();
        if (existingCollider != null)
        {
            existingCollider.isTrigger = false;
            
            // 새로운 Trigger Collider 추가
            BoxCollider2D triggerCollider = gameObject.AddComponent<BoxCollider2D>();
            triggerCollider.isTrigger = true;
            triggerCollider.size = existingCollider.size;
            triggerCollider.offset = existingCollider.offset;
        }
        
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        
        // 씬에 있는 모든 Enemy들과의 충돌을 무시
        RangedEnemy[] rangedEnemies = FindObjectsOfType<RangedEnemy>();
        MeleeEnemy[] meleeEnemies = FindObjectsOfType<MeleeEnemy>();
        
        foreach (var enemy in rangedEnemies)
        {
            if (enemy != this)  // 자기 자신은 제외
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), enemy.GetComponent<Collider2D>(), true);
            }
        }
        
        foreach (var enemy in meleeEnemies)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), enemy.GetComponent<Collider2D>(), true);
        }
        
        // firePoint 자동 생성
        GameObject firePointObj = new GameObject("FirePoint");
        firePoint = firePointObj.transform;
        firePoint.SetParent(transform);
        firePoint.localPosition = new Vector3(0f, 0f, 0f); // 발사 위치 고정
    }

    // 새로 스폰되는 Enemy들과도 충돌을 무시하기 위한 트리거 체크
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other, true);
        }
    }

    void Update()
    {
        // 플레이어가 죽었거나 없으면 더 이상 진행하지 않음
        if (PlayerController.IsDead || PlayerTransform == null)
        {
            StopMoving();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, PlayerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer > attackRange)
            {
                MoveTowardsPlayer();
                isPlayerInRange = false;
            }
            else
            {
                StopMoving();
                isPlayerInRange = true;
                
                // 공격 범위 안에 있고 쿨다운이 끝났으면 발사
                if (Time.time >= nextAttackTime)
                {
                    ShootProjectile();
                    nextAttackTime = Time.time + attackCooldown;
                }
            }

            UpdateFacingDirection();
        }
        else
        {
            StopMoving();
            isPlayerInRange = false;
        }
    }

    // virtual로 변경하여 오버라이드 가능하게 함
    protected virtual void ShootProjectile()
    {
        // 플레이어가 죽었다면 발사하지 않음
        if (PlayerController.IsDead) return;

        GameObject projectile = PoolManager.Instance.GetObject(projectileKey);
        if (projectile != null)
        {
            Vector3 spawnPosition = firePoint.position;
            projectile.transform.position = spawnPosition;

            EnemyProjectile projectileComponent = projectile.GetComponent<EnemyProjectile>();
            if (projectileComponent != null)
            {
                Vector2 direction = (PlayerTransform.position - spawnPosition).normalized;
                projectileComponent.Initialize(direction, ProjectileSpeed, AttackDamage);
            }
        }
    }

    void MoveTowardsPlayer()
    {
        // x축 방향으로만 이동하도록 수정
        float directionX = PlayerTransform.position.x > transform.position.x ? 1f : -1f;
        rb.velocity = new Vector2(directionX * moveSpeed, 0f);
    }

    void StopMoving()
    {
        rb.velocity = Vector2.zero;
    }

    void UpdateFacingDirection()
    {
        // 플레이어가 왼쪽에 있으면 true, 오른쪽에 있으면 false
        spriteRenderer.flipX = PlayerTransform.position.x < transform.position.x;
    }

    // 디버그용 시각화
    void OnDrawGizmosSelected()
    {
        // 공격 범위 표시 (빨간색)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 감지 범위 표시 (노란색)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    // 현재 플레이어가 공격 범위 안에 있는지 확인하는 프로퍼티
    public bool IsPlayerInRange => isPlayerInRange;
}
