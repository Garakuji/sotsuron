using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 5f;
    public GameObject explosionPrefab;
    private Vector2 moveDir;

    public void Init(Vector2 direction)
    {
        moveDir = direction.normalized;
        CancelInvoke();
        Invoke(nameof(DisableSelf), 5f); // 5초 뒤 자동 비활성화
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Update()
    {
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            gameObject.SetActive(false); // 파괴 대신 비활성화
        }
    }

    void OnEnable()
    {
        CancelInvoke(); // 중복 예약 방지
    }

    void DisableSelf()
    {
        gameObject.SetActive(false); // 시간이 지나도 비활성화
    }
}
