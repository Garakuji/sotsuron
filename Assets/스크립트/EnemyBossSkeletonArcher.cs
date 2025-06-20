using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossSkeletonArcher : Enemy
{
    [Header("Spread-Shot Only")]
    [Tooltip("부채꼴 샷에만 사용할 Arrow prefab")]
    [SerializeField] private GameObject spreadProjectilePrefab;

    [Header("Spread Shot Settings")]
    public int spreadCountPhase1 = 3;
    public int spreadCountPhase2 = 5;
    public float spreadAngle = 120f;
    public float spreadInterval = 0.2f;
    public float spreadProjectileSpeed = 5f;
    [Header("Boss Stats")]
    public float bossMaxHealth = 200f;

    private bool phase2 = false;
    private float currentHealth;

    void Start()
    {
        currentHealth = bossMaxHealth;
        // Enemy.base.maxHealth, base.health 동기화
        base.maxHealth = bossMaxHealth;
        base.health = bossMaxHealth;
        StartCoroutine(PatternLoop());
    }

    private IEnumerator PatternLoop()
    {
        while (currentHealth > 0)
        {
            // 1) Spread Shot (인디케이터 없이 바로 발사)
            int count = phase2 ? spreadCountPhase2 : spreadCountPhase1;
            yield return SpreadShot(count);

            // (추가 패턴 자리…)

            // 페이즈 전환
            if (!phase2 && currentHealth <= bossMaxHealth * 0.5f)
            {
                phase2 = true;
                // 2페이즈 초기화 로직 삽입
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// 보스 중심(transform.position)에서 부채꼴로 화살을 발사합니다.
    /// </summary>
    private IEnumerator SpreadShot(int count)
    {
        // 1) 플레이어 방향 기준 angle 계산
        Vector2 toPlayer = (GameManager.Instance.player.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        // 2) 부채꼴 분할
        float step = (count > 1) ? spreadAngle / (count - 1) : 0f;
        float startOffset = -spreadAngle / 2f;

        // 3) 실제 화살 발사
        for (int i = 0; i < count; i++)
        {
            float angle = baseAngle + startOffset + step * i;
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            var go = Instantiate(
                spreadProjectilePrefab,
                transform.position,
                Quaternion.Euler(0f, 0f, angle)
            );

            var ep = go.GetComponent<EnemyProjectile>();
            if (ep != null)
                ep.Init(dir, damage, spreadProjectileSpeed, projectileTravel);
            else
                Debug.LogError("Spread-Shot prefab에 EnemyProjectile이 없습니다!");

            yield return new WaitForSeconds(spreadInterval);
        }
    }

    // Enemy.TakeDamage가 virtual이라면 override, 아니라면 new로 선언하세요
    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);
        currentHealth -= dmg;
    }
}
