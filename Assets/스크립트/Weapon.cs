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
    private float waterTimer = 0f;
    public GameObject waterVFXPrefab;


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

            case 4: // 물속성 장판
                waterTimer += Time.deltaTime;
                if (waterTimer > speed)
                {
                    waterTimer = 0f;
                    Vector2 spawnPos = (Vector2)player.position + Random.insideUnitCircle * 2.5f;

                    // VFX 먼저 재생
                    GameObject vfx = Instantiate(waterVFXPrefab, spawnPos, Quaternion.identity);
                    StartCoroutine(DisableAfterDelay(vfx, 1f));

                    // 장판은 잠시 후에 생성
                    StartCoroutine(SpawnWaterZoneAfterDelay(spawnPos));
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
                damage = 5f;
                count = 1;
                speed = -150f;
                Batch();  // 회전 무기만
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
                damage = 2f;  // 틱당 데미지
                count = 3;    // 틱 횟수
                speed = 2.5f; // 장판 쿨타임
                break;

            default:
                damage = 5f;
                count = 1;
                speed = 1f;
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
    IEnumerator SpawnWaterZoneAfterDelay(Vector2 pos)
    {
        yield return new WaitForSeconds(0.3f); // VFX 재생 시간
        GameObject zone = GameManager.Instance.Pool.GetBullet(prefabId);
        zone.transform.position = pos;
        zone.SetActive(true);
        zone.GetComponent<WaterZone>().Init(damage, count, 0.3f, 1.5f);
    }

    private IEnumerator DisableAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
            obj.SetActive(false);
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
