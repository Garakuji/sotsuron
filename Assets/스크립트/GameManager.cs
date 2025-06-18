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

    // 더 이상 배열이 아니라, 함수로 계산합니다.
    [Header("# Level-Up Curve")]
    [Tooltip("레벨업에 필요한 기본 경험치")]
    public float expBase = 2f;
    [Tooltip("레벨업 필요 경험치 증가 비율 (>1)")]
    public float expGrowthRate = 1.2f;

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
        Debug.Log($"[GameManager] GetExp called with {amount}. Current EXP: {exp}, Level: {level}");
        exp += amount;

        bool leveledUp = false;

        while (exp >= GetRequiredExp(level))
        {

            Debug.Log($"[GameManager] Level UP! Current Level: {level} → {level + 1}");
            exp -= GetRequiredExp(level);
            level++;
            leveledUp = true;

            // 경험치 과다 누적으로 여러 번 레벨업될 수 있으므로 루프
            // 단, 무기 선택 UI는 한 번만 호출
        }

        if (leveledUp)
            ShowLevelUpChoices(); // ✅ UI는 한 번만 열림
    }

    private void ShowLevelUpChoices()
    {
        List<WeaponData> choices = GetRandomWeapons(3);
        weaponSelector.Show(choices);
    }

    /// <summary>
    /// 현재 레벨에서 레벨업까지 필요한 경험치
    /// Level 0→1: floor(expBase * (expGrowthRate^0)) = expBase
    /// Level 1→2: expBase * expGrowthRate
    /// Level 2→3: expBase * expGrowthRate^2
    /// … 무한 확장 가능
    /// </summary>
    public int GetRequiredExp(int lvl)
    {
        // lvl 이 0이면 expBase * 1, 1이면 expBase * expGrowthRate^1
        return Mathf.FloorToInt(expBase * Mathf.Pow(expGrowthRate, lvl));
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
