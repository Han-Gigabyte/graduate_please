using System.Collections;
using UnityEngine;

public class MidBoss : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    private float attackRange;
    private float detectionRange;

    [Header("Damage")]
    public int baseDamage = 20; // 기본 공격력
    public DamageMultiplier damageMultiplier; // 공격력 비율을 위한 ScriptableObject
    public int attackDamage; // 공격력
    private float projectileSpeed; // 발사체 속도
    private float damageCooldown;
    private float attackCooldown;
    private float knockbackForce;
    protected string projectileKey = "EnemyProjectile";
    protected Transform firePoint;
    

    [Header("Health")]
    public float baseHealth = 80f; // 기본 체력
    public HealthMultiplier healthMultiplier; // 체력 비율을 위한 ScriptableObject
    public float calculatedHealth; // 계산된 체력

    [Header("Skill Fireball")]
    public GameObject fireballPrefab; // 파이어볼 프리팹
    public int fireballBaseDamage = 30;  // 기본 데미지
    public float fireballProjectileSpeed = 10f; // 발사 속도
    public float fireballCooldown = 10f; // 스킬 쿨타임
    
    private bool canUseFireball = false;   // 스킬 사용 가능 여부

    [Header("Skill Shockwave")]
    public GameObject shockwavePrefab;  // 충격파 프리팹
    public Transform shockwaveSpawnPoint;  // 충격파 생성 위치
    public float shockwaveCooldown = 5f;  // 스킬 쿨타임

    private bool canUseShockwave = false;

    [Header("Skill Throw")]
    public GameObject throwObjectPrefab;
    public Transform throwPoint;
    public float throwCooldown = 5f; // 스킬 사용 간격

    private bool canUseThrow = false;


    private float nextDamageTime;
    private float nextAttackTime;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Transform playerTransform;
    private bool isPlayerInRange = false;

    void Start()
    {
        // GameManager에서 값 가져오기
        moveSpeed = GameManager.Instance.rangedEnemyMoveSpeed;
        attackRange = GameManager.Instance.rangedEnemyAttackRange;
        detectionRange = GameManager.Instance.rangedEnemyDetectionRange;
        attackCooldown = GameManager.Instance.rangedEnemyAttackCooldown;
        projectileSpeed = GameManager.Instance.rangedEnemyProjectileSpeed;
        damageCooldown = GameManager.Instance.meleeEnemyDamageCooldown;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        // 체력과 공격력 초기화
        float healthMultiplierValue = healthMultiplier.GetHealthMultiplier(GameManager.Instance.Stage, GameManager.Instance.Chapter);
        calculatedHealth = baseHealth * healthMultiplierValue;

        attackDamage = Mathf.RoundToInt(baseDamage * damageMultiplier.GetDamageMultiplier(GameManager.Instance.Stage, GameManager.Instance.Chapter));

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

        StartCoroutine(EnableSkillAfterDelay(5f));  // 5초 후 처음 사용 가능
    }

    // 새로 스폰되는 Enemy들과도 충돌을 무시하기 위한 트리거 체크
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other, true);
        }
    }

    private IEnumerator EnableSkillAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canUseFireball = true;
        canUseShockwave = true;
        canUseThrow = true;
    }

    void Update()
    {
        // 플레이어가 죽었거나 없으면 더 이상 진행하지 않음
        if (PlayerController.IsDead || playerTransform == null)
        {
            StopMoving();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

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
        /*
        if (canUseFireball)
        {
            StartCoroutine(UseFireball());
            canUseFireball = false;
            Invoke(nameof(ResetSkillFireballCooldown), fireballCooldown);
        }
        */

        /*
        if (canUseShockwave)
        {
            StartCoroutine(UseShockwave());
            canUseShockwave = false;
            Invoke(nameof(ResetSkillShockwaveCooldown), shockwaveCooldown);
        }
        */

        if (canUseThrow)
        {
            StartCoroutine(UseThrow());
            canUseThrow = false;
            Invoke(nameof(ResetSkillThrowCooldown), throwCooldown);
        }

    }

    private IEnumerator UseFireball()
    {
        Debug.Log("MidBoss가 파이어볼 스킬을 준비 중...");

        // 애니메이션 실행 (선택 사항)
        //GetComponent<Animator>().SetTrigger("UseSkill");

        yield return new WaitForSeconds(0.5f); // 애니메이션 대기

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) yield break; // 플레이어가 없으면 스킬 취소

        Vector2 targetPosition = player.transform.position; // 플레이어 위치 가져오기

        // 파이어볼 생성
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        fireball.GetComponent<Fireball>().SetFireball(fireballBaseDamage, projectileSpeed, targetPosition);

        Debug.Log("MidBoss가 파이어볼 발사!");
    }

    private IEnumerator UseShockwave()
    {
        Debug.Log("MidBoss가 충격파를 준비 중...");
        // 보스가 충격파 준비 모션
        //GetComponent<Animator>().SetTrigger("UseSkill");

        yield return new WaitForSeconds(1f);

        // 충격파 생성
        Instantiate(shockwavePrefab, shockwaveSpawnPoint.position, Quaternion.identity);
        Debug.Log("MidBoss가 충격파 사용!");

        yield return new WaitForSeconds(shockwaveCooldown); // 쿨타임 적용
    }

    private IEnumerator UseThrow()
    {
        Debug.Log("MidBoss가 투척 스킬 준비 중...");

        // 플레이어 위치 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        { 
            // 플레이어 위치 계산 후 던지기
            Vector2 targetPosition = playerTransform.position + Vector3.down * 0.5f; // 목표 지점 (땅과 근접)
            GameObject throws = Instantiate(throwObjectPrefab, firePoint.position, Quaternion.identity);
            throws.GetComponent<Throw>().setThrow(targetPosition);
        }

        Debug.Log("MidBoss가 투척 스킬 사용!");
        yield return new WaitForSeconds(throwCooldown);
    }

    private void ResetSkillFireballCooldown()
    {
        canUseFireball = true;
    }

    private void ResetSkillShockwaveCooldown()
    {
        canUseShockwave = true;
    }

    private void ResetSkillThrowCooldown()
    {
        canUseThrow = true;
    }

    // virtual로 변경하여 오버라이드 가능하게 함
    protected virtual void ShootProjectile()
    {
        // 플레이어가 죽었다면 발사하지 않음
        if (PlayerController.IsDead) return;

        GameObject projectile = PoolManager.Instance.GetObject(projectileKey);

        // null 체크 추가
        if (projectile == null)
        {
            Debug.LogError("발사체를 가져오는 데 실패했습니다.");
            return;
        }

        Vector3 spawnPosition = firePoint.position;
        projectile.transform.position = spawnPosition;

        EnemyProjectile projectileComponent = projectile.GetComponent<EnemyProjectile>();
        if (projectileComponent != null)
        {
            Vector2 direction = (playerTransform.position - spawnPosition).normalized;
            projectileComponent.Initialize(direction, projectileSpeed, attackDamage);
            projectileComponent.SetPoolManager(PoolManager.Instance, projectileKey);
        }
    }

    void MoveTowardsPlayer()
    {
        // x축 방향으로만 이동하도록 수정
        float directionX = playerTransform.position.x > transform.position.x ? 1f : -1f;
        rb.velocity = new Vector2(directionX * moveSpeed, 0f);
    }

    void StopMoving()
    {
        rb.velocity = Vector2.zero;
    }

    void UpdateFacingDirection()
    {
        // 플레이어가 왼쪽에 있으면 true, 오른쪽에 있으면 false
        spriteRenderer.flipX = playerTransform.position.x < transform.position.x;
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

    // 현재 플레이어가 공격 범위 안에 있는지 확인하는 프로퍼티
    public bool IsPlayerInRange => isPlayerInRange;
}
