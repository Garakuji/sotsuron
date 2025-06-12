using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    private float baseSpeed;
    public float health;
    public float maxHealth;
    public Rigidbody2D target;
    private Animator anim;
    private SpriteRenderer[] spriters;
    private Color[] originalColors;
    private bool isKnockback;
    private float knockbackTime = 0.1f;
    private bool isBurning = false;

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

        GetComponent<Collider2D>().enabled = true;
        gameObject.tag = "Enemy";

        // 🔧 상태이상 색 초기화
        for (int i = 0; i < spriters.Length; i++)
        {
            if (spriters[i] != null)
                spriters[i].color = originalColors[i];
        }

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
        baseSpeed = data.speed;   // 원래 속도 저장
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
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < spriters.Length; j++)
            {
                if (spriters[j] != null)
                    spriters[j].enabled = false;
            }

            yield return new WaitForSeconds(0.05f);

            for (int j = 0; j < spriters.Length; j++)
            {
                if (spriters[j] != null)
                    spriters[j].enabled = true;
            }

            yield return new WaitForSeconds(0.05f);
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



    public void ApplyBurn(float duration)
    {
        StartCoroutine(BurnEffect(duration));
    }

    private IEnumerator BurnEffect(float duration)
    {
        isBurning = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (!isSlowed)  // 슬로우 중이 아니면 빨간색
            {
                for (int i = 0; i < spriters.Length; i++)
                    spriters[i].color = Color.red;
            }

            yield return new WaitForSeconds(0.2f);

            if (!isSlowed)
            {
                for (int i = 0; i < spriters.Length; i++)
                    spriters[i].color = originalColors[i];
            }

            yield return new WaitForSeconds(0.2f);
            elapsed += 0.4f;
        }

        isBurning = false;

        // 화상 종료 후 슬로우 상태면 파란색 유지, 아니면 원상복구
        if (isSlowed)
            SetWaterVisual();
        else
            ResetColor();
    }


    private bool isSlowed = false;

    public void SetWaterVisual()
    {
        isSlowed = true;

        // 화상이 활성화 중이라도 파란색을 우선시
        for (int i = 0; i < spriters.Length; i++)
        {
            if (spriters[i] != null)
                spriters[i].color = Color.blue;
        }
    }

    public void ResetColor()
    {
        isSlowed = false;

        // 화상이 동시에 있는 경우는 시각화 유지
        if (isBurning)
            return;

        for (int i = 0; i < spriters.Length; i++)
        {
            if (spriters[i] != null)
                spriters[i].color = originalColors[i];
        }
    }

    public void ApplySlow(float ratio)
    {
        if (!isLive) return;

        speed = baseSpeed * (1f - ratio); // 원래 속도를 기준으로 계산
        SetWaterVisual();
    }

    public void RemoveSlow()
    {
        speed = baseSpeed; // 원래 속도로 복구
        ResetColor();
    }


}
