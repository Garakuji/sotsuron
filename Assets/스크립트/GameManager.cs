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

    [Header("# Weapon System")]
    public WeaponSelectorUI weaponSelector;
    public List<WeaponData> allWeaponData;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        health = maxHealth;
        allWeaponData = new List<WeaponData>(Resources.LoadAll<WeaponData>("WeaponData"));
        Debug.Log(allWeaponData.Count);
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

            List<WeaponData> choices = GetRandomWeapons(3);
            weaponSelector.Show(choices);  // 리스트 넘김
        }
    }




    public List<WeaponData> GetRandomWeapons(int count)
    {
        List<WeaponData> result = new();
        List<WeaponData> candidates = new(allWeaponData);

        while (result.Count < count && candidates.Count > 0)
        {
            int idx = Random.Range(0, candidates.Count);
            result.Add(candidates[idx]);
            candidates.RemoveAt(idx);
        }

        return result;
    }
}
