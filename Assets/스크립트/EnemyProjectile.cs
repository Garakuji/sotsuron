using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed;
    public float damage;
    public float maxTravel;

    private Vector2 dir;
    private Vector2 startPos;

    /// <summary>
    /// �߻� �ʱ�ȭ: ����, ������, �ӵ�, �ִ� ��Ÿ� ����
    /// </summary>
    public void Init(Vector2 direction, float dmg, float spd, float travel)
    {
        dir = direction.normalized;
        damage = dmg;
        speed = spd;
        maxTravel = travel;
        startPos = (Vector2)transform.position;
        gameObject.SetActive(true);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle-45f, Vector3.forward);
    }

    void Update()
    {
        // ����ü �����̱� (���� ��ǥ�� ����)
        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        // �ִ� ��Ÿ� ����� ��Ȱ��ȭ
        if (((Vector2)transform.position - startPos).magnitude >= maxTravel)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // �÷��̾� �±׿� �ε����� �����
        if (col.CompareTag("Player"))
        {
            // GameManager�� ���� ����� ����
            GameManager.Instance.TakeDamage(Mathf.RoundToInt(damage));
            gameObject.SetActive(false);
        }
        // �ʿ��ϴٸ�, ��(Wall) � �ε��� ���� ���ָ� �˴ϴ�.
        // else if (col.CompareTag("Wall"))
        // {
        //     gameObject.SetActive(false);
        // }
    }
}
