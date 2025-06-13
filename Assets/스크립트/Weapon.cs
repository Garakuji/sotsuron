using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;
    public Transform player;
    private Vector3 offset;
    private float timer;
    private float waterTimer = 0f;
    public GameObject waterVFXPrefab;

    void Awake()
    {
        // 플레이어 참조가 없으면 GameManager를 통해 가져오기
        if (player == null && GameManager.Instance != null)
        {
            player = GameManager.Instance.player.transform;
        }
    }

    void Start()
    {
        Init();
        ResetOffset();
    }

    void Update()
    {
        if (player == null) return;

        switch (id)
        {
            case 0:
                // 회전 무기: 플레이어를 중심으로 궤도 회전
                transform.position = player.position;
                transform.RotateAround(
                    player.position,
                    Vector3.forward,
                    speed * Time.deltaTime
                );
                break;

            case 1:
                // 타겟 추적 발사 무기
                FollowPlayer();
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;
                    FireTargeting();
                }
                break;

            case 2:
                // 번개 마도서
                FollowPlayer();
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;
                    FireLightning();
                }
                break;

            case 3:
                // 파이어볼 발사 무기
                FollowPlayer();
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;
                    FireFireball();
                }
                break;

            case 4:
                // 물속성 장판
                FollowPlayer();
                waterTimer += Time.deltaTime;
                if (waterTimer > speed)
                {
                    waterTimer = 0f;
                    SpawnWaterZone();
                }
                break;

            default:
                FollowPlayer();
                break;
        }

        // 디버그용 레벨업 테스트
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Levelup(damage + 5f, count + 1);
        }
    }

    /// <summary>
    /// 플레이어 위치에서 offset만큼 유지
    /// </summary>
    void FollowPlayer()
    {
        transform.position = player.position + offset;
    }

    /// <summary>
    /// offset 벡터 재계산
    /// </summary>
    public void ResetOffset()
    {
        if (player != null)
            offset = transform.position - player.position;
    }

    /// <summary>
    /// 무기 레벨업 처리
    /// </summary>
    public void Levelup(float newDamage, int newCount)
    {
        damage = newDamage;
        count = newCount;
        if (id == 0)
        {
            Batch();
        }
    }

    /// <summary>
    /// 초기화: id에 따라 속성 설정
    /// </summary>
    public void Init()
    {
        timer = 0f;
        waterTimer = 0f;

        switch (id)
        {
            case 0:
                damage = 5f;
                count = 1;
                speed = -150f;
                Batch();
                break;

            case 1:
                damage = 10f;
                count = 1;
                speed = 0.5f;
                break;

            case 2:
                damage = 8f;
                count = 3;
                speed = 1.2f;
                break;

            case 3:
                damage = 12f;
                count = 1;
                speed = 0.8f;
                break;

            case 4:
                damage = 2f;
                count = 3;
                speed = 2.5f;
                break;

            default:
                damage = 5f;
                count = 1;
                speed = 1f;
                break;
        }
    }

    /// <summary>
    /// child로 총알 배치 (회전형 무기 전용)
    /// </summary>
    void Batch()
    {
        // 자식 오브젝트가 현재 count보다 많으면 초과된 것 제거
        for (int i = transform.childCount - 1; i >= count; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        for (int index = 0; index < count; index++)
        {
            Transform bullet;

            if (index < transform.childCount)
            {
                bullet = transform.GetChild(index);
            }
            else
            {
                bullet = GameManager.Instance.Pool.GetBullet(prefabId).transform;
                bullet.parent = transform;
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -1);
        }
    }

    /// <summary>
    /// 물 장판 생성 및 VFX 재생
    /// </summary>
    void SpawnWaterZone()
    {
        Vector2 spawnPos = (Vector2)player.position + Random.insideUnitCircle * 2.5f;
        var vfx = Instantiate(waterVFXPrefab, spawnPos, Quaternion.identity);
        StartCoroutine(DisableAfterDelay(vfx, 1f));
        StartCoroutine(SpawnWaterZoneAfterDelay(spawnPos));
    }

    IEnumerator SpawnWaterZoneAfterDelay(Vector2 pos)
    {
        yield return new WaitForSeconds(0.3f);
        GameObject zone = GameManager.Instance.Pool.GetBullet(prefabId);
        zone.transform.position = pos;
        zone.SetActive(true);
        zone.GetComponent<WaterZone>().Init(damage, count, 0.3f, 1.5f);
    }

    IEnumerator DisableAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
            obj.SetActive(false);
    }

    void FireTargeting()
    {
        var scanner = player.GetComponent<move_test>()?.scanner;
        if (scanner == null || scanner.nearestTarget == null) return;

        var bulletObj = GameManager.Instance.Pool.GetBullet(prefabId);
        bulletObj.transform.position = transform.position;

        Vector3 dir = (scanner.nearestTarget.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bulletObj.transform.rotation = Quaternion.Euler(0, 0, angle - 45f);
        bulletObj.GetComponent<Bullet>().Init(damage, 1, dir, 10f);
    }

    void FireLightning()
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 offsetPos = Random.insideUnitCircle * 3f;
            var lightning = GameManager.Instance.Pool.GetBullet(prefabId);
            lightning.transform.position = (Vector2)transform.position + offsetPos;
            lightning.transform.rotation = Quaternion.identity;
            lightning.SetActive(true);

            var hit = lightning.GetComponent<LightningHitArea>();
            if (hit != null) hit.damage = damage;
        }
    }

    void FireFireball()
    {
        Vector2 dir = player.GetComponent<move_test>().lastMoveDir;
        if (dir == Vector2.zero) dir = Vector2.right;
        var fireballObj = GameManager.Instance.Pool.GetBullet(prefabId);
        fireballObj.transform.position = transform.position;
        fireballObj.GetComponent<Fireball>().Init(dir);
    }
}