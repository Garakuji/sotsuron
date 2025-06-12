using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per;

    private Vector3 direction;
    private float speed;

    public void Init(float damage, int per)
    {
        this.damage = damage;
        this.per = per;
    }

    public void Init(float damage, int per, Vector3 direction, float speed)
    {
        this.damage = damage;
        this.per = per;
        this.direction = direction.normalized;
        this.speed = speed;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 적이 아니툈E무시
        if (!collision.CompareTag("Enemy"))
            return;

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy == null)
            return;

        enemy.TakeDamage(damage);

        if (per > 0)
        {
            per--;
            if (per <= 0)
                gameObject.SetActive(false);
        }
        else if (per == 0)
        {
            gameObject.SetActive(false);
        }
    }


}
