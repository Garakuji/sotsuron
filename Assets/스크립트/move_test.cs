using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class move_test : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public Scanner scanner;
    private Rigidbody2D rigid;
    private SpriteRenderer spriter;
    private Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
    }
    private void Update()
    {
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
    }
    private void FixedUpdate()
    {
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void LateUpdate()
    {
        bool isMoving = inputVec != Vector2.zero;
        anim.SetBool("1_Move", isMoving);

        if (inputVec.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * -(inputVec.x < 0 ? -1 : 1);
            transform.localScale = scale;
        }
    }
}