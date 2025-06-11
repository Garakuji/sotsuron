using System.Threading;
using Goldmetal.UndeadSurvivor;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour 
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;
    public Transform player;
    private Vector3 offset;
    float timer;

    void Start()
    {
        Init();
        offset = transform.position - player.position;
    }

    private void Update()
    {
        transform.position = player.position + offset;

        switch (id)
        {
            case 0:
                transform.Rotate(0, 0, speed * Time.deltaTime, Space.World);
                break;
            default:
                timer += Time.deltaTime;

                if (timer > speed)
                {
                    timer = 0f;
                    Fire();
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Levelup(20, count + 1);
        }

    }

    public void Levelup(float damage, int count)
    {
        this.damage = damage;
        this.count = count;

        if (id == 0)
        {
            Batch();
        }
    }


    public void Init()
    {
        switch (id)
        {
            case 0:
                speed = -150;

                Batch ();

                break;
            default:
                speed = 0.3f;
                break;
        }
    }
    void Batch()
    {
        for (int index = 0; index < count; index++)
        {
            Transform bullet;

            if (index < transform.childCount)
            {
                bullet = transform.GetChild(index);
            }
            else
            {
                bullet = GameManager.Instance.Pool.GetBullet(prefabId).transform;
                bullet.parent = transform;
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -1); // -1 is Infinity per.
        }
    }


    void Fire()
    {
        if (player == null)
            return;

        Scanner scanner = player.GetComponent<move_test>().scanner;
        if (scanner == null || scanner.nearestTarget == null)
            return;

        GameObject bulletObj = GameManager.Instance.Pool.GetBullet(prefabId);
        if (bulletObj == null)
        {
            Debug.LogError($"Bullet prefabId {prefabId} is invalid.");
            return;
        }

        Transform bullet = bulletObj.transform;
        bullet.position = transform.position;

        // 타겟 방향 계산
        Vector3 dir = (scanner.nearestTarget.position - transform.position).normalized;

        // 방향 맞춰 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bullet.rotation = Quaternion.Euler(0, 0, angle - 45f);

        // 발사
        bullet.GetComponent<Bullet>().Init(damage, 1, dir, 10f);
    }

}
