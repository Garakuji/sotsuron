using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    int type;
    public float speed;
    private float baseSpeed;
    public float health;
    public float maxHealth;

    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer[] spriters;
    private Color[] originalColors;

    private Rigidbody2D target;
    private bool isLive;
    private bool isSlowed;
    private bool isBurning;
    private bool isPoisoned;
    private Coroutine slowRoutine;
    private bool isKnockedBack = false;
    private bool isRanged = false;
    private Coroutine rangedRoutine;
    [HideInInspector] public bool isPulled = false;
    public float knockbackForce = 3f;
    public float knockbackDuration = 0.2f;

    private bool hasChainBrand = false;
    private Color chainBrandColor = new Color(0.4f, 0.2f, 0.5f);

    // 디버프 전이용 저장 (OnDeath에서 사용)
    private float storedBrandDuration;
    private float storedTickDamage;
    private float storedTickInterval;
    private float storedSlowRate;

    [Header("Ranged Settings")]
    public GameObject projectilePrefab;
    public int projectilePoolIndex;      // ← 풀 매니저 인덱스
    public float attackCooldown;
    public float projectileSpeed;
    public float projectileTravel;
    public float fireRange = 5f;
    public float damage;

    private float timer = 0f;
    private Transform playerTf;

    [Tooltip("죽었을 때 드랍할 Exp Orb 개수")]
    public int expOrbCount = 1;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriters = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[spriters.Length];

        for (int i = 0; i < spriters.Length; i++)
            originalColors[i] = spriters[i].color;
    }


    void OnEnable()
    {
        playerTf = GameManager.Instance.player.transform;
        timer = Random.Range(0f, attackCooldown); // staggered start
        isPulled = false;
        isKnockedBack = false;
        isSlowed = false;
        isBurning = false;
        isPoisoned = false;
        hasChainBrand = false;
        isLive = true;
        speed = baseSpeed;
        health = maxHealth;
        gameObject.tag = "Enemy";
        anim.SetBool("isDeath", false);
        GetComponent<Collider2D>().enabled = true;

        for (int i = 0; i < spriters.Length; i++)
        {
            spriters[i].enabled = true;
            spriters[i].color = originalColors[i];
        }
        StartCoroutine(AssignTarget());
    }

    IEnumerator AssignTarget()
    {
        while (GameManager.Instance?.player == null)
            yield return null;

        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
    }

    public void Init(SpawnData data)
    {

        type = data.type;

        baseSpeed = data.speed;
        maxHealth = data.health;
        health = maxHealth;
        speed = baseSpeed;
        isRanged = false;
        projectilePoolIndex = -1;

        switch (type)
        {
            case 0:  //좀비
                maxHealth += 0f;
                speed += 0f;
                expOrbCount = 1;
                break;

            case 1:  //스켈레톤
                maxHealth -= data.health * 0.2f; // 기본 체력 20%감소
                speed *= 1.1f;  // 속도 10%증가
                expOrbCount = 1;
                break;
            case 2:  //뱀파이어
                maxHealth += data.health * 1.1f;
                speed += 0f;
                expOrbCount = 1;
                break;
            case 3:  //악마
                maxHealth += data.health * 1.1f;
                speed += 0f;
                expOrbCount = 1;
                break;
            case 4:  //악마전사 중형보스
                maxHealth += data.health * 3f;
                speed *= 1.2f;
                expOrbCount = 10;
                break;
            case 5:
                maxHealth -= data.health * 0.2f; // 기본 체력 20%감소
                speed *= 1.1f;  // 속도 10%증가
                projectilePoolIndex = 0;   // PoolManager.enemyBulletPrefabs[0]
                attackCooldown = 5f;
                projectileSpeed = 5f;
                projectileTravel = 7f;
                damage = 5f;
                isRanged = true;
                expOrbCount = 3;
                break;
            case 6:
                maxHealth += 0f;
                speed += 0f;
                projectilePoolIndex = 1;
                attackCooldown = 6f;
                projectileSpeed = 5f;
                projectileTravel = 7f;
                damage = 5f;
                isRanged = true;
                expOrbCount = 3;
                break;

            case 7:
                maxHealth += data.health * 20f;
                isRanged = true;
                projectilePoolIndex = 0;
                attackCooldown = 3f;
                projectileSpeed = 5f;
                projectileTravel = 7f;
                damage = 10f;
                speed *= 1.4f;
                expOrbCount = 30;
                break;

            case 8:
                maxHealth += data.health * 15f;
                isRanged = true;
                projectilePoolIndex = 1;
                attackCooldown = 3f;
                projectileSpeed = 5f;
                projectileTravel = 7f;
                damage = 10f;
                speed *= 0.8f;
                expOrbCount = 30;
                break;
            case 9:
                maxHealth += data.health * 25f;
                speed *= 1.4f;
                expOrbCount = 30;
                break;
        }
        if (isRanged)
        {
            // 혹시 이미 돌고 있던 게 있으면 멈추고
            if (rangedRoutine != null)
                StopCoroutine(rangedRoutine);
            // 새로 시작
            rangedRoutine = StartCoroutine(RangedAttackLoop());
        }
    }

    void FixedUpdate()
    {
        if (isKnockedBack || isPulled || !isLive || target == null) return;

        Vector2 dir = target.position - rigid.position;
        rigid.MovePosition(rigid.position + dir.normalized * speed * Time.fixedDeltaTime);
        anim.SetBool("1_Move", dir.magnitude > 0.01f);
    }

    void LateUpdate()
    {
        if (!isLive || target == null) return;

        Vector3 scale = transform.localScale;
        scale.x = target.position.x < rigid.position.x ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    public virtual void TakeDamage(float dmg)
    {
        TakeDamage(dmg, transform.position);
    }

    public virtual void TakeDamage(float dmg, Vector2 sourcePosition)
    {
        if (!isLive) return;

        health -= dmg;
        AudioManager.Instance.PlayEnemyHit();
        if (health <= 0f)
        {
            TransmitChainBrand();
            Die();
            return;
        }

        StartCoroutine(HitFlash());
        StartCoroutine(Knockback(sourcePosition));
    }

    IEnumerator HitFlash()
    {
        for (int i = 0; i < spriters.Length; i++)
        {
            var c = originalColors[i];
            spriters[i].color = new Color(c.r, c.g, c.b, 0f);
        }
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < spriters.Length; i++)
        {
            spriters[i].color = HasDebuff("ChainBrand") ? chainBrandColor : originalColors[i];
        }
    }

    void Die()
    {
        isLive = false;

        rigid.linearVelocity = Vector2.zero;
        rigid.angularVelocity = 0f;
        rigid.bodyType = RigidbodyType2D.Dynamic;

        StopAllCoroutines();
        anim.SetTrigger("4_Death");
        GetComponent<Collider2D>().enabled = false;
        gameObject.tag = "Untagged";

        StartCoroutine(DelayedExp());
    }
    IEnumerator DelayedExp()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < expOrbCount; i++)
        {
            var orb = GameManager.Instance.Pool.GetExp(0);
            orb.transform.position = transform.position
                                     + (Vector3)(Random.insideUnitCircle * 0.5f);
        }

        GameManager.Instance.kill++;
        gameObject.SetActive(false);
    }
    public void ApplyBurn(float duration)
    {
        StartCoroutine(BurnEffect(duration));
    }

    IEnumerator BurnEffect(float duration)
    {
        isBurning = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (!isSlowed && !hasChainBrand) SetColor(Color.red);
            yield return new WaitForSeconds(0.2f);
            if (!isSlowed && !hasChainBrand) ResetColor();
            yield return new WaitForSeconds(0.2f);
            elapsed += 0.4f;
        }

        isBurning = false;
        if (isSlowed) SetColor(Color.blue);
        else if (hasChainBrand) SetColor(chainBrandColor);
        else ResetColor();
    }

    public void ApplySlow(float ratio)
    {
        if (!isLive) return;

        speed = baseSpeed * (1f - ratio);
        isSlowed = true;
        if (!hasChainBrand) SetColor(Color.blue);
    }

    public void RemoveSlow()
    {
        speed = baseSpeed;
        isSlowed = false;

        if (isBurning) SetColor(Color.red);
        else if (hasChainBrand) SetColor(chainBrandColor);
        else ResetColor();
    }

    void SetColor(Color c)
    {
        foreach (var s in spriters)
            if (s != null) s.color = c;
    }

    void ResetColor()
    {
        for (int i = 0; i < spriters.Length; i++)
            if (spriters[i] != null)
                spriters[i].color = HasDebuff("ChainBrand") ? chainBrandColor : originalColors[i];
    }

    public void ApplyPoison(float tickDamage, float duration, float tickInterval)
    {
        if (!isLive || isPoisoned) return;
        StartCoroutine(PoisonEffect(tickDamage, duration, tickInterval));
    }

    IEnumerator PoisonEffect(float tickDamage, float duration, float tickInterval)
    {
        isPoisoned = true;
        float elapsed = 0f;

        if (!isBurning && !isSlowed && !hasChainBrand) SetColor(Color.green);

        while (elapsed < duration)
        {
            TakeDamage(tickDamage);
            elapsed += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }

        isPoisoned = false;

        if (isBurning) SetColor(Color.red);
        else if (isSlowed) SetColor(Color.blue);
        else if (hasChainBrand) SetColor(chainBrandColor);
        else ResetColor();
    }

    private IEnumerator Knockback(Vector2 sourcePos)
    {
        isKnockedBack = true;
        Vector2 kbDir = ((Vector2)transform.position - sourcePos).normalized;
        rigid.AddForce(kbDir * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        rigid.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }

    public void ApplyChainBrand(float duration, float tickDamage, float interval, float slowRate)
    {
        if (HasDebuff("ChainBrand")) return;

        storedBrandDuration = duration;
        storedTickDamage = tickDamage;
        storedTickInterval = interval;
        storedSlowRate = slowRate;

        StartCoroutine(ChainBrandRoutine(duration, tickDamage, interval, slowRate));
    }

    IEnumerator ChainBrandRoutine(float duration, float tickDamage, float interval, float slowRate)
    {
        AddDebuff("ChainBrand");
        float timer = 0f;
        speed *= (1f - slowRate);
        SetColor(chainBrandColor);

        while (timer < duration && isLive)
        {
            TakeDamage(tickDamage);
            yield return new WaitForSeconds(interval);
            timer += interval;
        }

        speed /= (1f - slowRate);
        RemoveDebuff("ChainBrand");
    }

    private void TransmitChainBrand()
    {
        if (!hasChainBrand) return;

        ChainBrandManager.Instance?.PlayTransmitVFX(transform.position);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f, LayerMask.GetMask("Enemy"));
        foreach (var col in hits)
        {
            Enemy e = col.GetComponent<Enemy>();
            if (e != null && !e.HasDebuff("ChainBrand"))
            {
                e.ApplyChainBrand(storedBrandDuration, storedTickDamage, storedTickInterval, storedSlowRate);
                break;
            }
        }
    }

    public bool HasDebuff(string name)
    {
        if (name == "ChainBrand") return hasChainBrand;
        return false;
    }

    public void AddDebuff(string name)
    {
        if (name == "ChainBrand") hasChainBrand = true;
    }

    public void RemoveDebuff(string name)
    {
        if (name == "ChainBrand") hasChainBrand = false;
        ResetColor();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
    public int collisionDamage = 10;    // 플레이어에게 줄 데미지

    // Enemy.cs
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            // collisionDamage는 Enemy 클래스에 public으로 선언돼 있어야 합니다
            GameManager.Instance.TakeDamage(collisionDamage);
        }
    }
    private void FireProjectile()
    {
        // 풀에서 가져오기
        var projGO = PoolManager.Instance.GetEnemyBullet(projectilePoolIndex);
        projGO.transform.position = transform.position;

        // 방향 계산 및 초기화
        var dir = (playerTf.position - transform.position).normalized;
        var ep = projGO.GetComponent<EnemyProjectile>();
        ep.Init(dir, damage, projectileSpeed, projectileTravel);
    }

    private IEnumerator RangedAttackLoop()
    {
        // 스폰 직후 무작위 딜레이 ( stagger )
        yield return new WaitForSeconds(Random.Range(0f, attackCooldown));

        // isLive가 false가 될 때까지 반복
        while (isLive)
        {
            // 플레이어와 거리 체크
            float dist = Vector2.Distance(transform.position, playerTf.position);
            if (dist <= fireRange)
            {
                // 사거리 안이면 발사
                FireProjectile();
                // 발사 후에는 쿨다운만큼 대기
                yield return new WaitForSeconds(attackCooldown);
            }
            else
            {
                // 사거리 밖이면 다음 프레임까지 대기
                yield return null;
            }
        }
    }
}