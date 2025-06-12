using UnityEngine;

public class LightningHitArea : MonoBehaviour
{
    public float damage = 10f;

    void OnEnable()
    {
        Invoke(nameof(Disable), 0.3f);  // 이펙트 유지 시간
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy e = collision.GetComponent<Enemy>();
            if (e != null)
                e.TakeDamage(damage);
        }
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }
}
