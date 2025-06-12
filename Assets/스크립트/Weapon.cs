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
    float timer;

    void Start()
    {
        Init();
        offset = transform.position - player.position;
    }

    private void Update()
    {
        transform.position = player.position + offset;

        switch (id)
        {
            case 0:
                // 회전 무기 (예: 차륜)
                transform.Rotate(0, 0, speed * Time.deltaTime, Space.World);
                break;

            case 1:
                // 타겟 추적 발사 무기 (예: 화살)
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;
                    FireTargeting();
                }
                break;

            case 2:
                // 번개 마도서 - 랜덤 위치 낙뢰
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;
                    FireLightning();
                }
                break;

            case 3:
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;

                    Vector2 dir = player.GetComponent<move_test>().lastMoveDir;
                    if (dir == Vector2.zero) dir = Vector2.right;

                    GameObject fireballObj = GameManager.Instance.Pool.GetBullet(prefabId);
                    fireballObj.transform.position = transform.position;
                    fireballObj.GetComponent<Fireball>().Init(dir);
                }
                break;
            case 4:  // WaterZone Attack
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;

                    // 1. 랜덤 위치 선택
                    Vector2 randPos = (Vector2)player.position + Random.insideUnitCircle * 3f;

                    // 2. VFX 이펙트 먼저 소환
                    GameObject vfx = GameManager.Instance.Pool.GetBullet(prefabId); // prefabId는 VFX 이펙트
                    vfx.transform.position = randPos;
                    vfx.SetActive(true);

                    // 3. VFX 이후 장판 생성 (0.5초 후)
                    StartCoroutine(SpawnWaterZoneAfterDelay(randPos, 0.5f));
                }
                break;


            default:
                break;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Levelup(damage + 5f, count + 1); // 예시 레벨업 효과
        }
    }

    public void Levelup(float damage, int count)
    {
        this.damage = damage;
        this.count = count;

        if (id == 0)
        {
            Batch();
        }
    }

    public void Init()
    {
        switch (id)
        {
            case 0:
                speed = -150;
                Batch();
                break;
            default:
                speed = 0.3f;
                break;
        }
    }

    void Batch()
    {
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
            bullet.GetComponent<Bullet>().Init(damage, -1); // 무한 관통
        }
    }
    IEnumerator SpawnWaterZoneAfterDelay(Vector2 pos, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject zone = GameManager.Instance.Pool.GetBullet(prefabId + 1); // prefabId+1 = WaterZone 본체
        zone.transform.position = pos;
        zone.SetActive(true);

        // 장판 초기화
        zone.GetComponent<WaterZone>().Init(damage, 3f, 0.3f,2f); // 기본값: 데미지, 지속시간, 슬로우율
    }

    void FireTargeting()
    {
        if (player == null)
            return;

        Scanner scanner = player.GetComponent<move_test>().scanner;
        if (scanner == null || scanner.nearestTarget == null)
            return;

        GameObject bulletObj = GameManager.Instance.Pool.GetBullet(prefabId);
        if (bulletObj == null)
        {
            Debug.LogError($"Bullet prefabId {prefabId} is invalid.");
            return;
        }

        Transform bullet = bulletObj.transform;
        bullet.position = transform.position;

        Vector3 dir = (scanner.nearestTarget.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bullet.rotation = Quaternion.Euler(0, 0, angle - 45f);

        bullet.GetComponent<Bullet>().Init(damage, 1, dir, 10f);
    }

    void FireLightning()
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 3f;
            Vector2 spawnPos = (Vector2)transform.position + offset;

            GameObject lightning = GameManager.Instance.Pool.GetBullet(prefabId);
            lightning.transform.position = spawnPos;
            lightning.transform.rotation = Quaternion.identity;
            lightning.SetActive(true);

            LightningHitArea hit = lightning.GetComponent<LightningHitArea>();
            if (hit != null)
            {
                hit.damage = damage;
            }
        }
    }
}
