using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class WaterZone : MonoBehaviour
{
    public float tickDamage;
    public float slowRatio;
    public float duration;

    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        circleCollider.isTrigger = true;

        // Sprite의 크기를 기준으로 콜라이더 반지름 설정 (화면상 범위와 정확히 맞춤)
        SyncColliderToSprite();
    }

    void SyncColliderToSprite()
    {
        if (spriteRenderer.sprite == null) return;

        // Sprite의 절반 너비 (extents) == 반지름
        float radiusWorld = spriteRenderer.bounds.extents.x;
        circleCollider.radius = radiusWorld;
    }

    public void Init(float tickDamage, float duration, float slowRatio, float radius)
    {
        this.tickDamage = tickDamage;
        this.duration = duration;
        this.slowRatio = slowRatio;

        SyncColliderToSprite(radius); // 범위 정확하게 동기화

        Invoke(nameof(Deactivate), duration);
    }


    private void SyncColliderToSprite(float desiredWorldRadius)
    {
        var col = GetComponent<CircleCollider2D>();
        var sr = GetComponent<SpriteRenderer>();
        if (sr == null || col == null || sr.sprite == null) return;

        // ✅ 항상 기본 스케일로 초기화한 후 계산 (중복 계산 방지)
        transform.localScale = Vector3.one;

        float currentSpriteWorldRadius = sr.bounds.extents.x;
        float scale = desiredWorldRadius / currentSpriteWorldRadius;

        transform.localScale = Vector3.one * scale;

        // 콜라이더는 sprite bounds가 아닌 원본 sprite 기준으로
        col.radius = sr.sprite.bounds.extents.x;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.ApplySlow(slowRatio);
                enemy.TakeDamage(tickDamage);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.ApplySlow(slowRatio);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RemoveSlow();
            }
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
