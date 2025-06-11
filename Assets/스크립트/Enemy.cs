using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public Rigidbody2D target;
    private Animator anim;
    private SpriteRenderer[] spriters;
    private Color[] originalColors;
    private bool isKnockback;
    private float knockbackTime = 0.1f;

    bool isLive;

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriters = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[spriters.Length];

        for (int i = 0; i < spriters.Length; i++)
            originalColors[i] = spriters[i].color;
    }

    void FixedUpdate()
    {
        if (!isLive || target == null || isKnockback) return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.linearVelocity = Vector2.zero;

        bool isMoving = dirVec.magnitude > 0.01f;
        anim.SetBool("1_Move", isMoving);
    }


    private void LateUpdate()
    {
        if (!isLive || target == null)
            return;

        Vector3 scale = transform.localScale;

        if (target.position.x < rigid.position.x)
            scale.x = Mathf.Abs(scale.x);
        else
            scale.x = -Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    private void OnEnable()
    {
        isLive = true;
        health = maxHealth;

        anim.SetBool("isDeath", false);

        // 충돌 및 타겟팅 다시 가능하도록 복구
        GetComponent<Collider2D>().enabled = true;
        gameObject.tag = "Enemy";

        StartCoroutine(AssignTargetLater());
    }

    IEnumerator AssignTargetLater()
    {
        while (GameManager.Instance == null || GameManager.Instance.player == null)
            yield return null;

        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
    }

    public void Init(SpawnData data)
    {
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLive) return; // 죽었으면 무시

        if (!collision.CompareTag("Bullet")) return;

        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet == null) return;

        TakeDamage(bullet.damage);

        // 관통 처리
        if (bullet.per > 0)
        {
            bullet.per--;
            if (bullet.per <= 0)
                bullet.gameObject.SetActive(false);
        }
        else if (bullet.per == 0)
        {
            bullet.gameObject.SetActive(false);
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isLive) return;

        health -= damage;
        if (health > 0)
        {
            StartCoroutine(DamageRoutine());
        }
        else
        {
            Dead();
        }
    }


    IEnumerator HitEffect()
    {
        for (int i = 0; i < spriters.Length; i++)
        {
            if (spriters[i] != null)
                spriters[i].color = Color.red; 
        }

        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < spriters.Length; i++)
        {
            if (spriters[i] != null)
                spriters[i].color = originalColors[i]; // 복원
        }
    }

    void Dead()
    {
        isLive = false;
        StopAllCoroutines();
        anim.SetTrigger("4_Death");

        GetComponent<Collider2D>().enabled = false;
        gameObject.tag = "Untagged";

        // 애니메이션 시간 후 ExpOrb 생성 및 비활성화
        StartCoroutine(DelayedExpAndDeactivate());
    }

    IEnumerator DelayedExpAndDeactivate()
    {
        yield return new WaitForSeconds(0.5f);  // 애니메이션 끝까지 재생

        GameObject orb = GameManager.Instance.Pool.GetExp(0); // 0번 오브 가져오기
        orb.transform.position = transform.position;

        GameManager.Instance.kill++;  // Kill 카운트 증가

        gameObject.SetActive(false);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
    IEnumerator Knockback(Vector3 hitFrom)
    {
        isKnockback = true;

        yield return new WaitForFixedUpdate(); // 1프레임 물리 딜레이

        Vector3 dirVec = transform.position - hitFrom;
        rigid.linearVelocity = Vector2.zero;
        rigid.AddForce(dirVec.normalized * 1.8f, ForceMode2D.Impulse); // 힘 조절 가능

        yield return new WaitForSeconds(knockbackTime);

        rigid.linearVelocity = Vector2.zero;
        isKnockback = false;
    }


    IEnumerator DamageRoutine()
    {
        isLive = false;
        anim.SetTrigger("3_Damaged");
        StartCoroutine(HitEffect());

        // 피격 후 잠시 멈춤
        yield return new WaitForSeconds(0.05f);

        // 넉백
        Vector2 knockBackDir = ((Vector2)transform.position - target.position).normalized;
        rigid.linearVelocity = Vector2.zero;
        rigid.AddForce(knockBackDir * 5f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.15f);

        rigid.linearVelocity = Vector2.zero;
        isLive = true;
    }


}
