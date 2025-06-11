using Goldmetal.UndeadSurvivor;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;

    // 이걸 새로 추가
    public GameObject[] monsterPrefabs;

    int level;
    float timer;

    private void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        level = Mathf.FloorToInt(GameManager.Instance.GameTime / 10f);

        if (timer > (level == 0 ? 0.5f : 0.2f))
        {
            timer = 0;
            Spawn();
        }
    }
    void Spawn()
    {
        int prefabIndex = Mathf.Min(level, GameManager.Instance.Pool.monsterPrefabs.Length - 1);
        GameObject enemy = GameManager.Instance.Pool.GetMonster(prefabIndex);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;

        Enemy enemyLogic = enemy.GetComponent<Enemy>();

        float baseSpeed = 2.0f;
        int baseHealth = 5;
        float speedGrowth = 0.2f;
        int healthGrowth = 2;

        SpawnData data = new SpawnData();
        data.speed = baseSpeed + level * speedGrowth;
        data.health = baseHealth + level * healthGrowth;
        data.spawnTime = 0;

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
