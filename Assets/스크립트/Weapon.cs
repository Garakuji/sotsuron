using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public int level = 1;
    public const int maxLevel = 5;

    public float damage;
    public int count;
    public float speed;

    // 회전 무기 전용
    public GameObject bladePrefab;       // Inspector에 Blade 프리팹 할당
    public float bladeDistance = 1.5f;   // 회전 반지름
    public float pivotYOffset = 0.3f;    // 플레이어 기준 높이 오프셋
    private Vector3 _bladeOriginalScale;

    // Offset for non-rotating weapons
    private Vector3 offset;
    private float timer;
    private float waterTimer;

    private float burnTickDamage;
    private float zoneTickDamage;
    private float zoneDuration;
    private float zoneRadius;

    public Transform player;
    public GameObject waterVFXPrefab;

    private Vector3 _originalScale;

    void Awake()
    {
        if (player == null && GameManager.Instance != null)
            player = GameManager.Instance.player.transform;
        _originalScale = transform.localScale;
    }

    void Start()
    {
        ApplyBaseStats();
        if (id == 0)
        {
            if (bladePrefab == null)
                Debug.LogError("[Weapon] bladePrefab을 Inspector에서 설정하세요.");
            else
            {
                _bladeOriginalScale = bladePrefab.transform.localScale;
                Batch();
            }
        }
        else
            SetOffset();
    }

    void Update()
    {
        if (player == null) return;

        if (id == 0)
        {
            // 회전 무기: pivot 위치에 고정
            Vector3 pivot = player.position + Vector3.up * pivotYOffset;
            transform.position = pivot;
            transform.localScale = _originalScale;
            // 자식 블레이드만 회전
            foreach (Transform blade in transform)
            {
                blade.RotateAround(pivot, Vector3.forward, speed * Time.deltaTime);
                blade.localScale = _bladeOriginalScale;
            }
        }
        else
        {
            FollowPlayer();
            if (id == 1 && (timer += Time.deltaTime) > speed)
            {
                timer = 0f;
                FireTrackingArrows();
            }
            else if (id == 2 && (timer += Time.deltaTime) > speed)
            {
                timer = 0f;
                FireLightningBolts();
            }
            else if (id == 3 && (timer += Time.deltaTime) > speed)
            {
                timer = 0f;
                FireFireball();
            }
            else if (id == 4 && (waterTimer += Time.deltaTime) > speed)
            {
                waterTimer = 0f;
                SpawnWaterZone();
            }

           /* else if (id == 10) // Toxic Thorns
            {
                InvokeRepeating(nameof(FireToxicThorns), 1f, 2.5f); // 2.5초마다 발동
            }*/
        }
    }

    public void Levelup()
    {
        if (level >= maxLevel) return;
        level++;
        ApplyLevelStats();
        if (id == 0)
            Batch(); // 블레이드 수 재설정
    }

    private void ApplyBaseStats()
    {
        level = 1;
        ApplyLevelStats();
    }

    private void ApplyLevelStats()
    {
        switch (id)
        {
            case 0:
                damage = 5f + 2f * (level - 1);
                count = 1 + ((level >= 3) ? 1 : 0) + ((level >= 5) ? 1 : 0);
                speed = 150f;
                break;
            case 1:
                damage = 10f + 3f * (level - 1);
                count = 1 + ((level >= 3) ? 1 : 0) + ((level >= 5) ? 1 : 0);
                speed = 0.5f;
                break;
            case 2:
                damage = 8f + 1f * (level - 1);
                count = 3;
                speed = 1.2f - 0.02f * (level - 1);
                break;
            case 3:
                damage = 12f + 2f * (level - 1);
                burnTickDamage = 2f + 1f * (level - 1);
                speed = 0.8f;
                break;
            case 4:
                zoneTickDamage = 3f;
                zoneDuration = 3.0f + 0.5f * (level - 1);
                zoneRadius = 2.0f + 0.5f * (level - 1);
                damage = zoneTickDamage;
                speed = 2.5f;
                break;
        }
    }

    private void SetOffset()
    {
        Vector3 pivot = player.position + Vector3.up * pivotYOffset;
        offset = transform.position - pivot;
    }

    void FollowPlayer()
    {
        Vector3 pivot = player.position + Vector3.up * pivotYOffset;
        transform.position = pivot + offset;
        transform.localScale = _originalScale;
    }

    void Batch()
    {
        // 기존 블레이드 제거
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        // 새로운 블레이드 생성 및 데미지 초기화
        Vector3 pivot = player.position + Vector3.up * pivotYOffset;
        for (int i = 0; i < count; i++)
        {
            GameObject blade = Instantiate(bladePrefab, pivot, Quaternion.identity, transform);
            Vector3 dir = Quaternion.Euler(0f, 0f, 360f * i / count) * Vector3.up;
            blade.transform.position = pivot + dir * bladeDistance;
            blade.transform.localScale = _bladeOriginalScale;
            // Blade에 Bullet 컴포넌트가 있다면 데미지와 관통 설정
            var bComp = blade.GetComponent<Bullet>();
            if (bComp != null)
                bComp.Init(damage, -1);
        }
    }

    void FireTrackingArrows()
    {
        // 1) MoveTest 経由で Scanner を取得
        var moveTest = player.GetComponent<move_test>();
        if (moveTest == null)
        {
            Debug.LogError("MoveTest がアタッチされていません");
            return;
        }

        var scanner = moveTest.scanner;
        if (scanner == null)
        {
            Debug.LogError("Scanner がセットされていません");
            return;
        }

        // 2) 最寄りターゲットを取得
        var target = scanner.nearestTarget;
        if (target == null)
        {
            // 範囲内に敵がいなければ発射しない
            return;
        }

        // 3) ターゲット方向の単位ベクトルと角度を計算
        Vector3 dir = (target.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 4) スプレッド（扇状）をかけたい場合はオフセットを足す
        float spread = 30f;
        int n = count;

        float denom = n - 1;          // 분모

        for (int i = 0; i < n; i++)
        {
            // count==1 이면 offset=0
            float offset = denom == 0
                ? 0f
                : spread * (i - denom / 2f) / denom;

            float ang = baseAngle + offset;

            var b = GameManager.Instance.Pool.GetBullet(prefabId);
            b.transform.position = transform.position;
            b.transform.rotation = Quaternion.Euler(0f, 0f, ang - 45f);

            Vector3 velocity = new Vector3(
                Mathf.Cos(ang * Mathf.Deg2Rad),
                Mathf.Sin(ang * Mathf.Deg2Rad),
                0f
            );
            b.GetComponent<Bullet>().Init(damage, 1, velocity, 10f);
        }
    }

    void FireLightningBolts()
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 pos = (Vector2)transform.position + Random.insideUnitCircle * 3f;
            var lt = GameManager.Instance.Pool.GetBullet(prefabId);
            lt.transform.position = pos;
            lt.SetActive(true);
            var hit = lt.GetComponent<LightningHitArea>(); if (hit) hit.damage = damage;
        }
    }

    void FireFireball()
    {
        Vector2 dir = player.GetComponent<move_test>().lastMoveDir;
        if (dir == Vector2.zero) dir = Vector2.right;
        var fb = GameManager.Instance.Pool.GetBullet(prefabId);
        fb.transform.position = transform.position;
        fb.GetComponent<Fireball>().Init(dir);
    }

    void SpawnWaterZone()
    {
        Vector2 pos = (Vector2)player.position + Random.insideUnitCircle * zoneRadius;
        var vfx = Instantiate(waterVFXPrefab, pos, Quaternion.identity);
        StartCoroutine(DisableAfterDelay(vfx, 1f));
        StartCoroutine(SpawnWaterZoneAfterDelay(pos));
    }

    IEnumerator SpawnWaterZoneAfterDelay(Vector2 pos)
    {
        yield return new WaitForSeconds(0.3f);
        var zone = GameManager.Instance.Pool.GetBullet(prefabId);
        zone.transform.position = pos;
        zone.SetActive(true);
        zone.GetComponent<WaterZone>().Init(zoneTickDamage, zoneDuration, 0.3f, zoneRadius);
    }

    IEnumerator DisableAfterDelay(GameObject obj, float d)
    {
        yield return new WaitForSeconds(d);
        if (obj) Destroy(obj);
    }

    /*void FireToxicThorns()
    {
        // 일정 범위 내의 적들을 찾아서 가시 생성
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, 1.5f, LayerMask.GetMask("Enemy"));

        foreach (var target in targets)
        {
            // 이펙트 생성 (선택사항)
            GameObject thorn = Instantiate(Resources.Load<GameObject>("Effect/ToxicThorn"), target.transform.position, Quaternion.identity);

            // 데미지 및 중독 처리
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // 기본 데미지
                if (!enemy.gameObject.GetComponent<PoisonEffect>())
                {
                    PoisonEffect poison = enemy.gameObject.AddComponent<PoisonEffect>();
                    poison.Apply(damage * 0.3f, 3f, 0.5f); // 중독 데미지, 지속 시간, 틱 간격
                }
            }
        }
    }*/
}