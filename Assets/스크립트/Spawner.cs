using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goldmetal.UndeadSurvivor;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Points (자식 Transform)")]
    public Transform[] spawnPoints;

    [Header("몬스터 스폰 주기 커스터마이즈")]
    [Tooltip("Level 0일 때 간격")]
    public float startInterval = 0.5f;
    [Tooltip("몇 레벨에 걸쳐 서서히 줄어들지")]
    public float intervalRampLevel = 30f;
    [Tooltip("최소 스폰 간격")]
    public float endInterval = 0.2f;

    [Header("몬스터 속도 비율 (플레이어 속도 대비)")]
    [Tooltip("기본 비율: 몬스터 속도 = player.speed × baseSpeedRatio")]
    public float baseSpeedRatio = 0.5f;
    [Tooltip("10레벨마다 이만큼 비율을 추가로 올림")]
    public float speedRatioIncrement = 0.05f;

    private float timer;
    private int level;
    private PoolManager pool;
    private move_test playerController;

    void Awake()
    {
        // 자식으로 붙은 SpawnPoint들
        spawnPoints = GetComponentsInChildren<Transform>();

        // PoolManager 싱글턴
        pool = PoolManager.Instance;
        if (pool == null)
            Debug.LogError("Spawner: PoolManager.Instance가 없습니다!");

        // 플레이어 이동 속도 가져오기
        playerController = GameManager.Instance.player.GetComponent<move_test>();
        if (playerController == null)
            Debug.LogError("Spawner: move_test 컴포넌트가 없습니다!");
    }

    void Update()
    {
        timer += Time.deltaTime;
        level = Mathf.FloorToInt(GameManager.Instance.GameTime / 10f);

        // 스폰 간격을 선형 보간으로 서서히 감소시킴
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
        // PoolManager에 등록된 몬스터 프리팹 개수에 맞춰 타입 결정
        int maxType = pool.monsterPrefabs.Length - 1;
        int type = Mathf.Clamp(level, 0, maxType);

        // 풀에서 몬스터 꺼내오기
        GameObject enemyGO = pool.GetMonster(type);

        // 랜덤 스폰 포인트 (인덱스 0은 자기 자신 Transform)
        int idx = Random.Range(1, spawnPoints.Length);
        enemyGO.transform.position = spawnPoints[idx].position;

        // 스탯 세팅
        Enemy enemyLogic = enemyGO.GetComponent<Enemy>();
        SpawnData data = new SpawnData();

        // — 체력(기존 로직 그대로) —
        int baseHealth = 5;
        int healthGrow = 2;
        data.health = baseHealth + level * healthGrow;

        // — 속도: 플레이어 속도 × 비율(레벨마다 소폭 증가) —
        int speedTiers = level / 10;
        float speedRatio = baseSpeedRatio + speedRatioIncrement * speedTiers;
        data.speed = playerController.speed * speedRatio;

        data.spawnTime = 0f;

        enemyLogic.Init(data);
    }
}

[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public int health;
    public float speed;
}
