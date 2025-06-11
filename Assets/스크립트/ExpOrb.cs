using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    public int expValue = 1;
    private Transform player;
    private float moveSpeed = 5f;
    private float attractRange = 3f;  // 일정 거리 내에서만 끌림

    void OnEnable()
    {
        player = GameManager.Instance.player.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);
        if (dist < attractRange)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        GameManager.Instance.GetExp(expValue);
        gameObject.SetActive(false);
    }
}
