// ===== PiercingSpear.cs 수정 =====
using UnityEngine;

public class PiercingSpear : MonoBehaviour
{
    [HideInInspector] public int damage;
    [HideInInspector] public float speed;
    [HideInInspector] public float maxDistance;

    private Vector2 _startPos;
    public Vector2 direction;  // 외부에서 지정할 수 있도록 공개

    void OnEnable()
    {
        _startPos = transform.position;
    }

    public void ResetState()
    {
        _startPos = transform.position;
        // direction 은 FirePiercingSpear() 에서 이미 세팅해 줌
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        if (Vector2.Distance(_startPos, transform.position) >= maxDistance)
            gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(damage);
        }
        else if (other.CompareTag("Wall"))
        {
            gameObject.SetActive(false);
        }
    }
}
