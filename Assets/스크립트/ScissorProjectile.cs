using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(LineRenderer))]
public class ScissorProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 50f;
    public float maxLife = 5f;
    public float collisionDamage = 5f;   // 충돌 시 입힐 대미지

    [Header("Trail Trap Settings")]
    public float dotDamage = 2f;
    public float dotInterval = 0.5f;
    public float trailThickness = 0.1f;

    [Header("Bounce Settings")]
    public LayerMask bounceLayers;

    private Rigidbody2D _rb;
    private LineRenderer _liveLine;
    private float _lifeTimer;

    private bool trailStarted;
    private GameObject trailObj;
    private LineRenderer trailLR;
    private EdgeCollider2D trailEdge;
    private List<Vector2> trailPts = new List<Vector2>();

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.gravityScale = 0f;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var col2d = GetComponent<Collider2D>();
        col2d.sharedMaterial = new PhysicsMaterial2D { friction = 0f, bounciness = 1f };

        _liveLine = GetComponent<LineRenderer>();
        _liveLine.positionCount = 0;
        _liveLine.material = new Material(Shader.Find("Sprites/Default"));
        _liveLine.startWidth = 0.05f;
        _liveLine.endWidth = 0.05f;
        _liveLine.colorGradient = CreateGradient();
    }

    public void Init(Vector2 dir)
    {
        _lifeTimer = 0f;
        trailStarted = false;
        _liveLine.positionCount = 0;
        _rb.linearVelocity = dir.normalized * speed;

        // 발사 직후부터 궤적 트랩 시작
        StartTrail(transform.position);
        trailStarted = true;
    }

    void Update()
    {
        _lifeTimer += Time.deltaTime;
        if (_lifeTimer >= maxLife)
        {
            DeactivateProjectile();
            return;
        }

        // 비행 궤적 그리기
        Vector3 pos = transform.position;
        if (_liveLine.positionCount == 0 ||
            Vector3.Distance(_liveLine.GetPosition(_liveLine.positionCount - 1), pos) > 0.05f)
        {
            int cnt = _liveLine.positionCount;
            _liveLine.positionCount = cnt + 1;
            _liveLine.SetPosition(cnt, pos);
        }

        // 트레일 콜라이더 업데이트
        if (trailStarted)
        {
            if (trailObj == null || trailEdge == null)
            {
                trailStarted = false;
                return;
            }

            Vector2 pt = transform.position;
            if (trailPts.Count == 0 || Vector2.Distance(trailPts[trailPts.Count - 1], pt) > 0.1f)
            {
                trailPts.Add(pt);
                UpdateTrailCollider();
                trailLR.positionCount = trailPts.Count;
                trailLR.SetPosition(trailPts.Count - 1, pt);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // 적과 충돌 시 즉시 대미지
        if (col.collider.CompareTag("Enemy"))
        {
            var e = col.collider.GetComponent<Enemy>();
            if (e != null) e.TakeDamage(collisionDamage);
        }

        // 벽 레이어에 부딪히면 반사
        if (((1 << col.gameObject.layer) & bounceLayers.value) != 0)
        {
            Vector2 normal = col.contacts[0].normal;
            Vector2 dir = Vector2.Reflect(_rb.linearVelocity.normalized, normal);
            _rb.linearVelocity = dir * speed;
        }
    }

    private void StartTrail(Vector2 startPos)
    {
        trailObj = new GameObject("ScissorTrailTrap");
        trailObj.layer = LayerMask.NameToLayer("Default");

        var rb = trailObj.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        trailEdge = trailObj.AddComponent<EdgeCollider2D>();
        trailEdge.isTrigger = true;
        trailEdge.edgeRadius = trailThickness;

        trailLR = trailObj.AddComponent<LineRenderer>();
        trailLR.material = new Material(Shader.Find("Sprites/Default"));
        trailLR.startWidth = 0.05f;
        trailLR.endWidth = 0.05f;
        trailLR.colorGradient = _liveLine.colorGradient;

        var trailScript = trailObj.AddComponent<ScissorTrail>();
        trailScript.Init(dotDamage, dotInterval, maxLife);

        trailPts.Clear();
        trailPts.Add(startPos);
        trailPts.Add(startPos);
        trailLR.positionCount = 2;
        trailLR.SetPosition(0, startPos);
        trailLR.SetPosition(1, startPos);
        UpdateTrailCollider();
    }

    private void UpdateTrailCollider()
    {
        if (trailObj == null || trailEdge == null) return;
        Vector2[] localPts = new Vector2[trailPts.Count];
        for (int i = 0; i < trailPts.Count; i++)
            localPts[i] = trailObj.transform.InverseTransformPoint(trailPts[i]);
        trailEdge.points = localPts;
    }

    private Gradient CreateGradient()
    {
        var grad = new Gradient();
        grad.SetKeys(
            new[] {
                new GradientColorKey(Color.cyan, 0f),
                new GradientColorKey(Color.blue, 1f)
            },
            new[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );
        return grad;
    }

    private void DeactivateProjectile()
    {
        _rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
    }
}