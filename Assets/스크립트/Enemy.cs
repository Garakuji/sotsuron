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
    private Coroutine slowRoutine;

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
        isLive = true;
        speed = baseSpeed;
        health = maxHealth;
        gameObject.tag = "Enemy";
        anim.SetBool("isDeath", false);
        GetComponent<Collider2D>().enabled = true;
        for (int i = 0; i < spriters.Length; i++)
        {
            if (spriters[i] != null)
            {
                spriters[i].enabled = true; // ✅ 반드시 초기화
                spriters[i].color = originalColors[i]; // ✅ 색상도 초기화
            }
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
        if (!isLive || target == null) return;

        Vector2 dir = target.position - rigid.position;
        rigid.MovePosition(rigid.position + dir.normalized * speed * Time.fixedDeltaTime);
        rigid.linearVelocity = Vector2.zero;

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
        if (!isLive) return;

        health -= dmg;

        if (health > 0)
            StartCoroutine(HitFlash());
        else
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
}
