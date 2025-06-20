using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goldmetal.UndeadSurvivor;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Points (자식 Transform)")]
    public Transform[] spawnPoints;

    [Header("몬스터 스폰 주기 커스터마이즈")]
    public float startInterval = 0.5f;
    public float intervalRampLevel = 30f;
    public float endInterval = 0.2f;

    [Header("몬스터 속도 비율 (플레이어 속도 대비)")]
    public float baseSpeedRatio = 0.5f;
    public float speedRatioIncrement = 0.05f;

    private float timer;
    private int level;
    private PoolManager pool;
     // monsterPrefabs[7] 에 보스 프리팹이 있다고 가정

    void Start()
    {
        spawnPoints = GetComponentsInChildren<Transform>();
        pool = PoolManager.Instance;
        if (pool == null)
            Debug.LogError("Spawner: PoolManager.Instance가 없습니다!");
    }

    void Update()
    {
        timer += Time.deltaTime;
        level = Mathf.FloorToInt(GameManager.Instance.GameTime / 15f);

        float t = Mathf.Clamp01(level / intervalRampLevel);
        float spawnInterval = Mathf.Lerp(startInterval, endInterval, t);

        if (timer > spawnInterval)
        {
            timer = 0f;
            Spawn();
        }
    }

    private void Spawn()
    {
        float elapsed = GameManager.Instance.GameTime;


        // ───── 기존 일반 몬스터 스폰 로직 ─────
        int baseHealth = 10;
        int healthGrow = 8;
        int type;

        if (elapsed > 60f && Random.value < 0.1f)
        {
            type = 4;
        }
        else if (elapsed > 90f && Random.value < 0.1f)
        {
            type = 5;
        }
        else if (elapsed > 120f && Random.value < 0.1f)
        {
            type = 6;
        }
        else if (elapsed > 90f && Random.value < 0.05f)
        {
            type = 7;
        }
        else if (elapsed > 90f && Random.value < 0.05f)
        {
            type = 8;
        }
        else if (elapsed > 90f && Random.value < 0.05f)
        {
            type = 9;
        }
        else
        {
            var spawnable = new List<int>();
            if (level < 2) spawnable.Add(0);
            else if (level < 4) spawnable.AddRange(new[] { 0, 1 });
            else spawnable.AddRange(new[] { 0, 1, 2, 3 });
            type = spawnable[Random.Range(0, spawnable.Count)];
        }

        var enemyGO = pool.GetMonster(type);
        int idx = Random.Range(1, spawnPoints.Length);
        enemyGO.transform.position = spawnPoints[idx].position;

        var data = new SpawnData
        {
            type = type,
            health = baseHealth + level * healthGrow,
            speed = 3f * (baseSpeedRatio + speedRatioIncrement * (level / 10f))
        };
        enemyGO.GetComponent<Enemy>().Init(data);
    }
}

[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public int type;    // 0~3: 일반, 4: 엘리트, 5~6: 원거리, 7: 보스
    public float health;
    public float speed;
}
