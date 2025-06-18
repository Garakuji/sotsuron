using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(ParticleSystem))]
public class ShockwaveHammer : MonoBehaviour
{
    [HideInInspector] public float maxRadius;
    [HideInInspector] public float expandTime;       // 총 확산 시간
    [HideInInspector] public float knockbackForce;
    [HideInInspector] public int   damage;

    private CircleCollider2D _col;
    private ParticleSystem    _ps;
    private float             _vfxDuration;
    private bool              _isRunning;

    void Awake()
    {
        _col = GetComponent<CircleCollider2D>();
        _col.isTrigger = true;
        _col.radius = 0f;
        transform.localScale = Vector3.one * 0.01f;

        _ps = GetComponent<ParticleSystem>();
        // emission duration + max lifetime
        _vfxDuration = _ps.main.duration + _ps.main.startLifetime.constantMax;
    }

    /// <summary>
    /// 호출 직후부터 플레이어를 따라다니며, 반 타임으로 재생합니다.
    /// </summary>
    public void FireFollow(Transform player)
    {
        if (_isRunning) return;
        _isRunning = true;
        StartCoroutine(RunFollow(player));
    }

    private IEnumerator RunFollow(Transform player)
    {
        // 절반으로 너프
        float halfExpand = expandTime * 0.5f;
        float halfVfx     = _vfxDuration  * 0.5f;

        // 이펙트 재생
        _ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _ps.Play(true);

        float elapsed = 0f;
        _col.enabled = true;

        // 1) 반 타임 확산
        while (elapsed < halfExpand)
        {
            // 항상 플레이어 위치 동기화
            transform.position = player.position;

            float t = elapsed / halfExpand;
            float r = Mathf.Lerp(0f, maxRadius, t);
            _col.radius = r;
            transform.localScale = Vector3.one * (r / maxRadius);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 2) 반 타임 유지
        float keepTime = halfVfx - halfExpand;
        float timer    = 0f;
        while (timer < keepTime)
        {
            transform.position = player.position;
            _col.radius = maxRadius;
            transform.localScale = Vector3.one;

            timer += Time.deltaTime;
            yield return null;
        }

        // 3) 정리
        _col.enabled = false;
        transform.localScale = Vector3.one * 0.01f;
        _isRunning = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        var enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;

        var rb = other.attachedRigidbody;
        if (rb != null)
        {
            Vector2 dir = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
            rb.AddForce(dir * (knockbackForce * 0.5f), ForceMode2D.Impulse);
        }

        enemy.TakeDamage(damage);
    }
}
