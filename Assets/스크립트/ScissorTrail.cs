using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissorTrail : MonoBehaviour
{
    private float dotDamage, dotInterval, duration;
    private Dictionary<Collider2D, Coroutine> coros = new Dictionary<Collider2D, Coroutine>();

    public void Init(float dmg, float interval, float life)
    {
        dotDamage = dmg;
        dotInterval = interval;
        duration = life;
        StartCoroutine(DisableAfter());
    }

    private IEnumerator DisableAfter()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other) => TryStartDOT(other);
    void OnTriggerStay2D(Collider2D other) => TryStartDOT(other);

    private void TryStartDOT(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !coros.ContainsKey(other))
            coros[other] = StartCoroutine(ApplyDOT(other));
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (coros.TryGetValue(other, out var c))
        {
            StopCoroutine(c);
            coros.Remove(other);
        }
    }

    private IEnumerator ApplyDOT(Collider2D col)
    {
        var e = col.GetComponent<Enemy>();
        while (true)
        {
            if (e != null)
                e.TakeDamage(dotDamage);
            yield return new WaitForSeconds(dotInterval);
        }
    }
}
