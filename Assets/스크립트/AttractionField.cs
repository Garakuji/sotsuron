// ===== AttractionField.cs =====
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class AttractionField : MonoBehaviour
{
    [HideInInspector] public float duration;
    [HideInInspector] public float pullSpeed;
    [HideInInspector] public float radius;

    public LayerMask targetLayer;

    private CircleCollider2D _col;
    private float _elapsed;

    void Awake()
    {
        _col = GetComponent<CircleCollider2D>();
        _col.isTrigger = true;
    }

    void OnEnable()
    {
        _elapsed = 0f;
        StartCoroutine(LifeCycle());
    }

    private IEnumerator LifeCycle()
    {
        while (_elapsed < duration)
        {
            _elapsed += Time.deltaTime;
            yield return null;
        }
        // 필드 종료 시 모든 적의 isPulled 해제
        foreach (var enemy in FindObjectsOfType<Enemy>())
            enemy.isPulled = false;

        gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        // 1) 모든 Enemy.isPulled 초기화
        foreach (var e in FindObjectsOfType<Enemy>())
            e.isPulled = false;

        // 2) 범위 내 콜라이더 검색
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);
        Debug.Log($"[AttractionField] hits count: {hits.Length}");

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<Enemy>();
            if (enemy == null) continue;

            // 3) 끌림 상태 설정
            enemy.isPulled = true;
            Debug.Log($"[AttractionField] pulling: {enemy.name}");

            // 4) 실제 끌어당김
            var rb = hit.attachedRigidbody;
            if (rb != null)
            {
                Vector2 dir = ((Vector2)transform.position - rb.position).normalized;
                Vector2 newPos = rb.position + dir * pullSpeed * Time.fixedDeltaTime;
                rb.MovePosition(newPos);
            }
            else
            {
                Transform t = hit.transform;
                Vector2 dir = ((Vector2)transform.position - (Vector2)t.position).normalized;
                t.position = Vector2.MoveTowards(t.position, transform.position, pullSpeed * Time.fixedDeltaTime);
            }
        }
    }

    public void Init(float duration, float pullSpeed, float radius)
    {
        this.duration = duration;
        this.pullSpeed = pullSpeed;
        this.radius = radius;

        _col.radius = radius;
        float diameter = radius * 2f;
        transform.localScale = new Vector3(diameter, diameter, 1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
