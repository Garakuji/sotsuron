using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public int level = 1;
    public const int maxLevel = 7;

    public float damage;
    public int count;
    public float speed;  // attack cooldown

    // Blade weapon (id==0)
    public GameObject bladePrefab;
    public float bladeDistance = 1.5f;
    public float pivotYOffset = 0.3f;
    private Vector3 _bladeOriginalScale;

    // Offset for other weapons
    private Vector3 offset;
    private float timer;
    private float scissorTimer;
    private float waterTimer;

    private float burnTickDamage;
    private float zoneTickDamage;
    private float zoneDuration;
    private float zoneRadius;

    public Transform player;
    public GameObject waterVFXPrefab;

    // Thread Trap stats
    public float scissorSpeed = 5f;
    public float scissorMaxLife = 3.0f;
    public float scissorDirectDamage = 1f;       // 기본 직접 대미지 낮춤

    [Header("Thread Trap Trail DOT")]
    public float scissorTrailDotDamage = 1f;
    public float scissorDotInterval = 0.5f;
    public float scissorTrailRadius = 0.3f;
    // trailDuration 대신 scissorMaxLife 사용

    [Header("Piercing Spear Stats")]
    public float spearProjectileSpeed = 12f;
    public float spearProjectileDistance = 5f;


    [Header("Attraction Field Stats")]
    public float fieldDuration = 3f;
    public float fieldPullForce = 10f;
    public float fieldRadius = 2f;

    [Header("Shockwave Hammer Stats")]
    public float hammerMaxRadius   = 2f;
    public float hammerExpandTime  = 0.3f;
    public float hammerKnockback   = 8f;
    public int   hammerDamage      = 2;

    [Header("Shockwave Hammer Leveling")]
    public float hammerBaseRadius        = 4f;
    public float hammerRadiusPerLevel    = 0.5f;
    public float hammerBaseExpandTime    = 0.2f;
    public float hammerExpandTimePerLevel = 0.01f;
    public float hammerBaseKnockback     = 8f;
    public float hammerKnockbackPerLevel = 1f;
    public int   hammerBaseDamage        = 3;
    public int   hammerDamagePerLevel    = 1;

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
        {
            SetOffset();
        }
    }

    void Update()
    {
        if (player == null) return;

        switch (id)
        {
            case 0:
                Vector3 pivot = player.position + Vector3.up * pivotYOffset;
                transform.position = pivot;
                transform.localScale = _originalScale;
                foreach (Transform blade in transform)
                {
                    blade.RotateAround(pivot, Vector3.forward, speed * Time.deltaTime);
                    blade.localScale = _bladeOriginalScale;
                }
                break;

            case 1:
                FollowPlayer();
                if ((timer += Time.deltaTime) > speed)
                {
                    timer = 0f;
                    FireTrackingArrows();
                }
                break;

            case 2:
                FollowPlayer();
                if ((timer += Time.deltaTime) > speed)
                {
                    timer = 0f;
                    FireLightningBolts();
                }
                break;

            case 3:
                FollowPlayer();
                if ((timer += Time.deltaTime) > speed)
                {
                    timer = 0f;
                    FireFireball();
                }
                break;

            case 4:
                FollowPlayer();
                if ((waterTimer += Time.deltaTime) > speed)
                {
                    waterTimer = 0f;
                    SpawnWaterZone();
                }
                break;

            case 5:
                FollowPlayer();
                if ((timer += Time.deltaTime) > speed)
                {
                    timer = 0f;
                    FireToxicThorns();
                }
                break;

            case 6:
                FollowPlayer();
                if ((timer += Time.deltaTime) > speed)
                {
                    timer = 0f;
                    FireScythe();
                }
                break;

            case 7: // Thread Trap
                FollowPlayer();
                scissorTimer += Time.deltaTime;
                if (scissorTimer > speed)
                {
                    scissorTimer = 0f;
                    FireScissor();
                }
                break;
            case 8:
                FollowPlayer();
                if ((timer += Time.deltaTime) > speed)
                {
                    timer = 0f;
                    FirePiercingSpear();
                }
                break;
            case 9: // 예: 9번을 Attraction Field ID로
                FollowPlayer();
                if ((timer += Time.deltaTime) > speed)
                {
                    timer = 0f;
                    FireAttractionField();
                }
                break;
            case 10:
                FollowPlayer();
                if ((timer += Time.deltaTime) > speed)
                {
                    timer = 0f;
                    FireShockwaveHammer();
                }
                break;
            default:
                FollowPlayer();
                break;

        }
    }

    public void Levelup()
    {
        if (level >= maxLevel) return;
        level++;
        ApplyLevelStats();
        if (id == 0)
            Batch();
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
                count = 1 + ((level >= 2) ? 1 : 0) + ((level >= 3) ? 1 : 0) + ((level >= 4) ? 1 : 0);
                speed = 150f;
                break;

            case 1:
                damage = 6f + 3f * (level - 1);
                count = 1 + ((level >= 2) ? 1 : 0) + ((level >= 3) ? 1 : 0);
                speed = 0.5f;
                break;

            case 2:
                damage = 8f + 1f * (level - 1);
                count = 3;
                speed = 1.2f - 0.05f * (level - 1);
                break;

            case 3:
                damage = 12f + 2f * (level - 1);
                burnTickDamage = 2f + 1f * (level - 1);
                speed = 0.8f;
                break;

            case 4:
                zoneTickDamage = 3f;
                zoneDuration = 3.0f + 0.5f * (level - 1);
                zoneRadius = 1.0f + 0.5f * (level - 1);
                damage = zoneTickDamage;
                speed = 2.5f;
                break;

            case 5:
                damage = 6f + 2f * (level - 1);
                burnTickDamage = damage * 0.3f + level; //posion
                zoneDuration = 3f + 0.3f * (level - 1);
                speed = Mathf.Max(3.0f - 0.2f * (level - 1), 0.5f);
                break;

            case 6:
                damage = 10f + 3f * (level - 1);
                count = 1 + ((level >= 2) ? 1 : 0);
                speed = Mathf.Max(1.5f - 0.2f * (level - 1), 0.5f);
                break;

            case 7: // Thread Trap stats
                scissorDirectDamage = 3f + 0.5f * (level - 1);
                scissorTrailDotDamage = 3f + 0.5f * (level - 1);
                scissorMaxLife = 3.0f + 0.5f * (level - 1);
                speed = scissorSpeed = 5f + 2f * (level - 1);
                scissorDotInterval = Mathf.Max(0.5f - 0.05f * (level - 1), 0.2f);
                // 레벨 증가에 따른 최대 발사 개수 제한: 최대 4
                count = Mathf.Min(level, 2);
                break;

            case 8:  // Piercing Spear
                damage = 10f + 3f * (level - 1);
                spearProjectileSpeed = 12f + 1f * (level - 1);
                spearProjectileDistance = 5f + 0.5f * (level - 1);
                speed = Mathf.Max(2.0f - 0.1f * (level - 1), 0.3f); // 발사 쿨다운
                break;

            case 9: // Attraction Field
                // 레벨에 따라 파라미터 조정
                fieldDuration = 2f + 0.5f * (level - 1);
                fieldPullForce = 8f + 2f * (level - 1);
                fieldRadius = 0.5f + 0.2f * (level - 1);
                speed = Mathf.Max(3f - 0.1f * (level - 1), 1f);
                break;

            case 10: // Shockwave Hammer 레벨업 처리
                // 반경 증가
                hammerMaxRadius = hammerBaseRadius + hammerRadiusPerLevel * (level - 1);
                // 확산 시간 (짧을수록 빠름)
                hammerExpandTime = Mathf.Max(hammerBaseExpandTime + hammerExpandTimePerLevel * (level - 1),0.05f);
                // 넉백 세기 증가
                hammerKnockback = hammerBaseKnockback + hammerKnockbackPerLevel * (level - 1);
                // 데미지 증가
                hammerDamage = hammerBaseDamage + hammerDamagePerLevel * (level - 1);
                // 쿨다운 5%씩 감소, 최소 0.2초
                speed = 2f;
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

    void FireToxicThorns()
    {
        // 플레이어 주변 적 감지
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, 1.5f, LayerMask.GetMask("Enemy"));

        // 레벨에 비례해 공격할 대상 수 제한
        int maxTargets = Mathf.Min(level, targets.Length);

        for (int i = 0; i < maxTargets; i++)
        {
            var target = targets[i];
            Vector3 basePos = target.transform.position;
            int thornCount = 3;
            float spreadRadius = 0.3f;

            for (int j = 0; j < thornCount; j++)
            {
                Vector2 offset = Random.insideUnitCircle * spreadRadius;
                Vector3 spawnPos = basePos + new Vector3(offset.x, offset.y, 0f);

                // ▶ PoolManager를 통한 프리팹 생성으로 수정
                GameObject thorn = GameManager.Instance.Pool.GetBullet(prefabId);
                thorn.transform.position = spawnPos;
                thorn.SetActive(true);

                // ▶ 데미지 및 중독 정보 전달
                var poisonComp = thorn.GetComponent<PoisonThorn>();
                if (poisonComp != null)
                {
                    poisonComp.Init(damage, damage * 0.1f, 3f, 0.5f);  // 독 데미지 낮춤
                }
            }
        }
    }
    void FireScythe()
    {
        // 0) 파라미터: 필요에 따라 이 값을 조정하세요
        float offsetDist = -1.2f;  // <-- 예전 0.8f보다 멀리 띄움

        // 1) 플레이어가 진짜 어디를 보고 있는지 판별
        //    (player 스프라이트를 flipX 로 좌우 반전시킨다고 가정)
        bool facingRight = player.localScale.x > 0f;

        // 2) 스폰 위치 계산 (플레이어 앞 방향으로만)
        Vector3 spawnPos = player.position + Vector3.right * (facingRight ? offsetDist : -offsetDist);

        // 3) 풀에서 낫 오브젝트 꺼내기
        GameObject scythe = GameManager.Instance.Pool.GetBullet(prefabId);
        scythe.transform.position = spawnPos;
        scythe.transform.rotation = Quaternion.identity;

        // 4) 재사용 꼬임 방지: 항상 기본 스케일로 초기화
        scythe.transform.localScale = Vector3.one;

        // 5) 전체 오브젝트를 localScale.x 로만 플립
        scythe.transform.localScale = new Vector3(facingRight ? 1f : -1f, 1f, 1f);

        // 6) 콜라이더 Offset 보정 (콜라이더가 루트에 붙어 있다고 가정)
        var col = scythe.GetComponent<Collider2D>();
        if (col != null)
        {
            Vector2 off = col.offset;
            off.x = Mathf.Abs(off.x) * (facingRight ? 1f : -1f);
            col.offset = off;
        }

        // 7) 데미지 초기화 및 활성화
        var comp = scythe.GetComponent<Scythe>();
        if (comp != null) comp.Init(damage);
        scythe.SetActive(true);
    }

    /// <summary>
    /// Thread Trap 발사
    /// </summary>
    void FireScissor()
    {
        Vector2 baseDir = player.GetComponent<move_test>().lastMoveDir;
        if (baseDir == Vector2.zero) baseDir = Vector2.right;

        for (int i = 0; i < count; i++)
        {
            // 발사 방향에 약간의 분산 추가 (선택사항)
            float angleOffset = (count > 1) ? Mathf.Lerp(-15f, 15f, i / (float)(count - 1)) : 0f;
            Vector2 dir = Quaternion.Euler(0, 0, angleOffset) * baseDir;

            var projObj = GameManager.Instance.Pool.GetBullet(prefabId);
            projObj.transform.position = (Vector2)player.position + dir.normalized * 0.5f;

            // 방향에 따라 스프라이트 회전 설정
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            projObj.transform.rotation = Quaternion.Euler(0f, 0f, angle - 45f);

            var projCol = projObj.GetComponent<Collider2D>();
            var playerCol = player.GetComponent<Collider2D>();
            if (projCol != null && playerCol != null)
                Physics2D.IgnoreCollision(projCol, playerCol, true);

            var sp = projObj.GetComponent<ScissorProjectile>();
            if (sp != null)
            {
                sp.speed = scissorSpeed;
                sp.collisionDamage = scissorDirectDamage;
                sp.maxLife = scissorMaxLife;
                sp.dotDamage = scissorTrailDotDamage;
                sp.dotInterval = scissorDotInterval;
                sp.Init(dir);
            }

            projObj.SetActive(true);
        }
    }
    // ===== Weapon.cs 내 FirePiercingSpear() 수정 =====
    private void FirePiercingSpear()
    {
        // 풀에서 프리팹 꺼내기
        var spearObj = GameManager.Instance.Pool.GetBullet(prefabId);
        spearObj.transform.position = transform.position;

        // 발사 방향 계산 (플레이어의 마지막 이동 방향)
        Vector2 dir = player.GetComponent<move_test>().lastMoveDir;
        if (dir == Vector2.zero) dir = Vector2.right;
        dir.Normalize();

        // 프리팹 회전: 기본 오른쪽(Vector2.right) → dir
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        spearObj.transform.rotation = Quaternion.AngleAxis(angle - 45f, Vector3.forward);

        // 컴포넌트 초기화
        var spear = spearObj.GetComponent<PiercingSpear>();
        if (spear != null)
        {
            spear.damage = Mathf.RoundToInt(damage);
            spear.speed = spearProjectileSpeed;
            spear.maxDistance = spearProjectileDistance;
            spear.direction = dir;           // 새 방향 지정
            spear.ResetState();
        }

        spearObj.SetActive(true);
    }
    private void FireAttractionField()
    {
        // 풀에서 필드 프리팹 가져오기
        var fieldObj = GameManager.Instance.Pool.GetBullet(prefabId);

        // 플레이어 주변 랜덤 위치 계산
        float spawnRadius = fieldRadius; // 원 안에 스폰하고 싶다면 fieldRadius 사용
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        Vector2 spawnPos = (Vector2)player.position + randomOffset;

        fieldObj.transform.position = spawnPos;

        // 컴포넌트 초기화
        var field = fieldObj.GetComponent<AttractionField>();
        if (field != null)
        {
            field.Init(fieldDuration, fieldPullForce, fieldRadius);
        }

        fieldObj.SetActive(true);
    }
// Weapon.cs: FireShockwaveHammer() 호출부에서 너프된 파라미터 설정
private void FireShockwaveHammer()
{
    var obj = GameManager.Instance.Pool.GetBullet(prefabId);
    obj.SetActive(true);

    var hammer = obj.GetComponent<ShockwaveHammer>();
    if (hammer != null)
    {
        // 너프된 파라미터
        hammer.maxRadius      = 2.5f;
        hammer.expandTime     = 0.6f;  // 총 확산 시간의 반을 내부에서 0.3초로 사용
        hammer.knockbackForce = 9f;
        hammer.damage         = hammerDamage;

        // 플레이어를 계속 따라다니며 실행
        hammer.FireFollow(player);
    }
}


}