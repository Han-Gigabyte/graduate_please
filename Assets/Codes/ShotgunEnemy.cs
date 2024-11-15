using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunEnemy : RangedEnemy
{
    private const float SPREAD_ANGLE = 10f;  // 탄퍼짐 각도
    private const int PROJECTILE_COUNT = 3;  // 발사할 투사체 수

    protected override void ShootProjectile()
    {
        // 플레이어가 죽었다면 발사하지 않음
        if (PlayerController.IsDead) return;

        Vector2 directionToPlayer = (PlayerTransform.position - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        // 여러 발의 투사체 발사
        for (int i = 0; i < PROJECTILE_COUNT; i++)
        {
            GameObject projectile = PoolManager.Instance.GetObject(projectileKey);
            if (projectile != null)
            {
                projectile.transform.position = firePoint.position;

                // 각도 계산 (-10, 0, 10도)
                float currentAngle = baseAngle + SPREAD_ANGLE * (i - 1);  // -10, 0, 10
                Vector2 direction = GetDirectionFromAngle(currentAngle);

                EnemyProjectile projectileComponent = projectile.GetComponent<EnemyProjectile>();
                if (projectileComponent != null)
                {
                    projectileComponent.Initialize(direction, ProjectileSpeed, AttackDamage);
                }
            }
        }
    }

    private Vector2 GetDirectionFromAngle(float angle)
    {
        // 각도를 라디안으로 변환
        float radian = angle * Mathf.Deg2Rad;
        // 방향 벡터 계산
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
