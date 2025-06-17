using UnityEngine;

public class PoisonThorn : MonoBehaviour
{
    public float riseHeight = 1f;
    public float riseSpeed = 10f;
    public float lifeTime = 0.5f;  // 🟡 훨씬 짧게

    private float damage;
    private float dotDamage;
    private float dotDuration;
    private float dotTickRate;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float timer;

    public void Init(float damage, float dotDamage, float duration, float tickRate)
    {
        this.damage = damage;
        this.dotDamage = dotDamage;
        this.dotDuration = duration;
        this.dotTickRate = tickRate;

        timer = 0f;
        startPos = transform.position - new Vector3(0, riseHeight, 0);
        targetPos = transform.position;
        transform.position = startPos;
    }

    void OnEnable()
    {
        // 다시 활성화될 때도 초기화 보장
        timer = 0f;
        startPos = transform.position - new Vector3(0, riseHeight, 0);
        targetPos = transform.position;
        transform.position = startPos;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, riseSpeed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer > lifeTime)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                enemy.ApplyPoison(dotDamage, dotDuration, dotTickRate);
            }

            gameObject.SetActive(false); // 🟢 충돌 후 바로 제거
        }
    }
}
