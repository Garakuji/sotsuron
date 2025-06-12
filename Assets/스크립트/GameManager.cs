using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("# Game Control")]
    public float GameTime;
    [Header("# Player Info")]
    public int health;
    public int maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 10, 30, 60, 100, 150, 210, 280, 360, 450, 600 };
    [Header("# Game object")]
    public move_test player;
    public PoolManager Pool;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        health = maxHealth;
    }
    private void Update()
    {
        GameTime += Time.deltaTime;
    
    }

    public void GetExp(int amount = 1)
    {
        exp += amount;

        if (exp >= nextExp[level])
        {
            exp = 0;
            level++;
        }
    }
}
