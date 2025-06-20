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
    /// 발사 초기화: 방향, 데미지, 속도, 최대 사거리 세팅
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
        // 투사체 움직이기 (월드 좌표계 기준)
        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        // 최대 사거리 벗어나면 비활성화
        if (((Vector2)transform.position - startPos).magnitude >= maxTravel)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // 플레이어 태그에 부딪히면 대미지
        if (col.CompareTag("Player"))
        {
            // GameManager를 통해 대미지 적용
            GameManager.Instance.TakeDamage(Mathf.RoundToInt(damage));
            gameObject.SetActive(false);
        }
        // 필요하다면, 벽(Wall) 등에 부딪힐 때도 꺼주면 됩니다.
        // else if (col.CompareTag("Wall"))
        // {
        //     gameObject.SetActive(false);
        // }
    }
}
