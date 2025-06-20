using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    public static PoolManager Instance;

    [Header("monster Prefabs")]
    public GameObject[] monsterPrefabs;

    [Header("weapon Prefabs")]
    public GameObject[] bulletPrefabs;

    [Header("Exp Prefabs")]
    public GameObject[] expPrefabs;

    [Header("enemy Projectile Prefabs")]      // ① 인스펙터용 배열 추가
    public GameObject[] enemyBulletPrefabs;

    private List<GameObject>[] monsterPools;
    private List<GameObject>[] bulletPools;
    private List<GameObject>[] enemyBulletPools;
    private List<GameObject>[] expPools;
    

    private void Awake()
    {

        Instance = this;
        // 몬스터 풀 초기화
        monsterPools = new List<GameObject>[monsterPrefabs.Length];
        for (int i = 0; i < monsterPrefabs.Length; i++)
            monsterPools[i] = new List<GameObject>();

        // 총알 풀 초기화
        bulletPools = new List<GameObject>[bulletPrefabs.Length];
        for (int i = 0; i < bulletPrefabs.Length; i++)
            bulletPools[i] = new List<GameObject>();

        // 경험치 풀 초기화
        expPools = new List<GameObject>[expPrefabs.Length];
        for (int i = 0; i < expPrefabs.Length; i++)
            expPools[i] = new List<GameObject>();

        enemyBulletPools = new List<GameObject>[enemyBulletPrefabs.Length];
                for (int i = 0; i < enemyBulletPrefabs.Length; i++)
            enemyBulletPools[i] = new List<GameObject>();
    }

    public GameObject GetMonster(int index)
    {
        GameObject select = null;

        foreach (GameObject item in monsterPools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        if (select == null)
        {
            select = Instantiate(monsterPrefabs[index], transform);
            monsterPools[index].Add(select);
        }

        return select;
    }

    public GameObject GetBullet(int index)
    {
        GameObject select = null;

        foreach (GameObject item in bulletPools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        if (select == null)
        {
            select = Instantiate(bulletPrefabs[index], transform);
            bulletPools[index].Add(select);
        }

        return select;
    }

    public GameObject GetExp(int index)
    {
        GameObject select = null;

        foreach (GameObject item in expPools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        if (select == null)
        {
            select = Instantiate(expPrefabs[index], transform);
            expPools[index].Add(select);
        }

        return select;
    }
    public GameObject GetEnemyBullet(int index)
    {
        GameObject select = null;
        foreach (GameObject item in enemyBulletPools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        if (select == null)
        {
            select = Instantiate(enemyBulletPrefabs[index], transform);
            enemyBulletPools[index].Add(select);
        }
        return select;
    }
}
