using System.Collections.Generic;
using UnityEngine;

public class Scythe : MonoBehaviour
{
    public float lifeTime = 0.3f;
    public float damage = 10f;

    private HashSet<Enemy> _hitEnemies = new HashSet<Enemy>();

    public void Init(float dmg)
    {
        damage = dmg;
        _hitEnemies.Clear();
        CancelInvoke();
        Invoke(nameof(Disable), lifeTime);
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        var enemy = other.GetComponent<Enemy>();
        if (enemy != null && !_hitEnemies.Contains(enemy))
        {
            enemy.TakeDamage(damage);
            _hitEnemies.Add(enemy);
        }
    }
}
