using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
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
    [HideInInspector] public bool isPulled = false;
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;
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
        // ── 여기서 모든 상태 이상 플래그 초기화 ──
        isPulled = false;
        isKnockedBack = false;
        isSlowed = false;
        isBurning = false;
        isPoisoned = false;

        // (기존 초기화들)
        isLive = true;
        speed = baseSpeed;
        health = maxHealth;
        gameObject.tag = "Enemy";
        anim.SetBool("isDeath", false);
        GetComponent<Collider2D>().enabled = true;
        // 스프라이트 색·활성화 초기화…
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
        baseSpeed = data.speed;
        maxHealth = data.health;
        health = maxHealth;
        speed = baseSpeed;
    }

    void FixedUpdate()
    {
        if (isKnockedBack)
        {
            Debug.Log($"[Enemy] {name} skipping move: knocked back");
            return;
        }
        if (isPulled)
        {
            Debug.Log($"[Enemy] {name} skipping move: isPulled");
            return;
        }

        if (!isLive || target == null) return;

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
    public void TakeDamage(float dmg)
    {
        // 넉백 원점을 enemy 자기 위치로 넘겨줌
        TakeDamage(dmg, transform.position);
    }
    public void TakeDamage(float dmg, Vector2 sourcePosition)
    {
        if (!isLive) return;

        health -= dmg;
        StartCoroutine(HitFlash());

        // 넉백 코루틴 호출
        StartCoroutine(Knockback(sourcePosition));

        if (health <= 0f)
            Die();
    }

    IEnumerator HitFlash()
{
    for (int i = 0; i < 2; i++)
    {
        foreach (var s in spriters) s.enabled = false;
        yield return new WaitForSeconds(0.05f);
        foreach (var s in spriters) s.enabled = true;
        yield return new WaitForSeconds(0.05f);
    }
}

    void Die()
    {
        isLive = false;
        StopAllCoroutines();
        anim.SetTrigger("4_Death");

        GetComponent<Collider2D>().enabled = false;
        gameObject.tag = "Untagged";

        StartCoroutine(DelayedExp());
    }

    IEnumerator DelayedExp()
    {
        yield return new WaitForSeconds(0.5f);
        var orb = GameManager.Instance.Pool.GetExp(0);
        orb.transform.position = transform.position;
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
            if (!isSlowed) SetColor(Color.red);
            yield return new WaitForSeconds(0.2f);

            if (!isSlowed) ResetColor();
            yield return new WaitForSeconds(0.2f);

            elapsed += 0.4f;
        }

        isBurning = false;
        if (isSlowed)
            SetColor(Color.blue);
        else
            ResetColor();
    }

    public void ApplySlow(float ratio)
    {
        if (!isLive) return;

        speed = baseSpeed * (1f - ratio);
        isSlowed = true;
        SetColor(Color.blue);
    }

    public void RemoveSlow()
    {
        speed = baseSpeed;
        isSlowed = false;

        if (isBurning)
            SetColor(Color.red);
        else
            ResetColor();
    }

    void SetColor(Color c)
    {
        foreach (var s in spriters)
            if (s != null)
                s.color = c;
    }

    void ResetColor()
    {
        for (int i = 0; i < spriters.Length; i++)
            if (spriters[i] != null)
                spriters[i].color = originalColors[i];
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

        if (!isBurning && !isSlowed)
            SetColor(Color.green);  // 독 색상

        while (elapsed < duration)
        {
            TakeDamage(tickDamage);
            elapsed += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }

        isPoisoned = false;

        // 색상 초기화 혹은 다른 상태이상에 따라 재설정
        if (isBurning)
            SetColor(Color.red);
        else if (isSlowed)
            SetColor(Color.blue);
        else
            ResetColor();
    }
    private IEnumerator Knockback(Vector2 sourcePos)
    {
        isKnockedBack = true;

        // 넉백 방향: (Enemy 위치 − 데미지 원점 위치)
        Vector2 kbDir = ((Vector2)transform.position - sourcePos).normalized;
        rigid.AddForce(kbDir * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        // 넉백 후 관성을 없애고 상태 복원
        rigid.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }

}
