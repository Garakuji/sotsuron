using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossSkeletonArcher : Enemy
{
    [Header("Spread-Shot Only")]
    [Tooltip("��ä�� ������ ����� Arrow prefab")]
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
        // Enemy.base.maxHealth, base.health ����ȭ
        base.maxHealth = bossMaxHealth;
        base.health = bossMaxHealth;
        StartCoroutine(PatternLoop());
    }

    private IEnumerator PatternLoop()
    {
        while (currentHealth > 0)
        {
            // 1) Spread Shot (�ε������� ���� �ٷ� �߻�)
            int count = phase2 ? spreadCountPhase2 : spreadCountPhase1;
            yield return SpreadShot(count);

            // (�߰� ���� �ڸ���)

            // ������ ��ȯ
            if (!phase2 && currentHealth <= bossMaxHealth * 0.5f)
            {
                phase2 = true;
                // 2������ �ʱ�ȭ ���� ����
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// ���� �߽�(transform.position)���� ��ä�÷� ȭ���� �߻��մϴ�.
    /// </summary>
    private IEnumerator SpreadShot(int count)
    {
        // 1) �÷��̾� ���� ���� angle ���
        Vector2 toPlayer = (GameManager.Instance.player.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        // 2) ��ä�� ����
        float step = (count > 1) ? spreadAngle / (count - 1) : 0f;
        float startOffset = -spreadAngle / 2f;

        // 3) ���� ȭ�� �߻�
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
                Debug.LogError("Spread-Shot prefab�� EnemyProjectile�� �����ϴ�!");

            yield return new WaitForSeconds(spreadInterval);
        }
    }

    // Enemy.TakeDamage�� virtual�̶�� override, �ƴ϶�� new�� �����ϼ���
    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);
        currentHealth -= dmg;
    }
}
