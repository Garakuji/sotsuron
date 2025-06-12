using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterZone : MonoBehaviour
{
    private float tickDamage = 2f;
    private float duration = 3f;
    private float slowRatio = 0.3f;
    private float tickInterval = 1f;
    private float radius = 2f;

    [SerializeField] private GameObject waterVFX; // 이펙트 프리팹

    private void OnEnable()
    {
        StartCoroutine(ZoneRoutine());

        if (waterVFX != null)
        {
            GameObject effect = Instantiate(waterVFX, transform.position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * radius;
        }

        transform.localScale = Vector3.one * radius;
    }

    public void Init(float tickDamage, float duration, float slowRatio, float radius)
    {
        this.tickDamage = tickDamage;
        this.duration = duration;
        this.slowRatio = slowRatio;
        this.radius = radius;
    }

    private IEnumerator ZoneRoutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            ApplyEffects();
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        gameObject.SetActive(false);
    }

    private void ApplyEffects()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius / 2f, LayerMask.GetMask("Enemy"));

        foreach (Collider2D col in hits)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(tickDamage);
                enemy.ApplySlow(slowRatio);
                StartCoroutine(ResetAfterDelay(enemy, tickInterval)); // 슬로우 제거 예약
            }
        }
    }

    private IEnumerator ResetAfterDelay(Enemy enemy, float delay)
    {
        yield return new WaitForSeconds(delay);
        enemy.RemoveSlow();
    }
}
