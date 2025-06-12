using UnityEngine;

public class FireExplosion : MonoBehaviour
{
    public float damage = 10f;
    public float burnDuration = 3f;
    public float radius = 1.5f;
    public float lifetime = 0.5f;

    private void OnEnable()
    {
        // 폭발 범위 내 모든 적 감지
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("Enemy"));

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // 즉시 피해
                enemy.ApplyBurn(burnDuration); // 화상 효과
            }
        }

        // 일정 시간 후 비활성화
        Invoke(nameof(DisableSelf), lifetime);
    }

    private void DisableSelf()
    {
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
