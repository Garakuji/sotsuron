using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("# Game Control")]
    public float GameTime;

    [Header("# Player Info")]
    public int health;
    public int maxHealth;
    public int level;
    public int kill;
    public int exp;

    [Header("# Level-Up Curve")]
    [Tooltip("레벨업에 필요한 기본 경험치")]
    public float expBase = 2f;
    [Tooltip("레벨업 필요 경험치 증가 비율 (>1)")]
    public float expGrowthRate = 1.2f;

    [Header("# Character Selection")]
    [HideInInspector] public CharacterData selectedCharacter;

    [Header("# Game Objects")]
    public Transform player;      // 플레이어 Transform
    private Animator playerAnim;  // 플레이어 Animator 캐시

    // 플레이어 본체 및 자식 스프라이트들
    private SpriteRenderer[] _playerRenderers;
    private Color[] _origColors;

    public move_test playerPrefab;    // 플레이어 프리팹
    public PoolManager Pool;

    [Header("# Weapon System")]
    public WeaponSelectorUI weaponSelector;
    public List<WeaponData> allWeaponData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Pool = PoolManager.Instance;
        allWeaponData = new List<WeaponData>(
            Resources.LoadAll<WeaponData>("WeaponData")
        );
    }

    private void Update()
    {
        GameTime += Time.deltaTime;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
            SpawnPlayer();

        if (scene.name == "ResultScene")
            ShowResult();
    }

    private void SpawnPlayer()
    {
        // 1) 인스턴스화
        var go = Instantiate(selectedCharacter.prefab);
        go.name = "Player";
        player = go.transform;

        // 2) 스탯 세팅
        maxHealth = selectedCharacter.maxHealth;
        health = maxHealth;
        go.GetComponent<move_test>().speed = selectedCharacter.moveSpeed;

        // 3) Animator & SpriteRenderer 캐싱
        playerAnim = go.GetComponent<Animator>();
        _playerRenderers = go.GetComponentsInChildren<SpriteRenderer>();
        _origColors = new Color[_playerRenderers.Length];
        for (int i = 0; i < _playerRenderers.Length; i++)
            _origColors[i] = _playerRenderers[i].color;

        // 이하 네가 이미 갖고 있던 세팅들
        Pool = PoolManager.Instance;
        WeaponManager.Instance.weaponParent = player;
        weaponSelector = Object.FindFirstObjectByType<WeaponSelectorUI>();
        if (weaponSelector == null)
            Debug.LogError("[GameManager] WeaponSelectorUI를 씬에서 찾을 수 없습니다!");
        foreach (var f in Object.FindObjectsByType<CameraFollow>(FindObjectsSortMode.None))
            f.target = player;

        if (selectedCharacter.useRandomWeapon)
        {
            int idx = Random.Range(0, allWeaponData.Count);
            WeaponManager.Instance.AddOrLevelupWeapon(allWeaponData[idx]);
        }
        else if (selectedCharacter.startingWeapon != null)
        {
            WeaponManager.Instance.AddOrLevelupWeapon(selectedCharacter.startingWeapon);
        }
    }

    /// <summary>
    /// 경험치 획득 및 레벨업 UI 호출
    /// </summary>
    public void GetExp(int amount = 1)
    {
        exp += amount;
        bool leveledUp = false;
        while (exp >= GetRequiredExp(level))
        {
            exp -= GetRequiredExp(level);
            level++;
            leveledUp = true;
        }
        if (leveledUp)
        {
            AudioManager.Instance.PlayLevelUp();
            ShowLevelUpChoices();
        }
    }

    private void ShowLevelUpChoices()
    {
        weaponSelector.Show();
    }

    public int GetRequiredExp(int lvl)
        => Mathf.FloorToInt(expBase * Mathf.Pow(expGrowthRate, lvl));

    public List<WeaponData> GetRandomWeapons(int count)
    {
        var result = new List<WeaponData>();
        var candidates = new List<WeaponData>(allWeaponData);
        while (result.Count < count && candidates.Count > 0)
        {
            int idx = Random.Range(0, candidates.Count);
            result.Add(candidates[idx]);
            candidates.RemoveAt(idx);
        }
        return result;
    }

    /// <summary>
    /// 플레이어 피격 시 호출
    /// </summary>
    public void TakeDamage(int dmg)
    {
        if (health <= 0) return;

        // 1) 체력 차감
        health = Mathf.Max(health - dmg, 0);

        // 2) 피격 애니메이션 및 플래시
        if (playerAnim != null)
            playerAnim.SetTrigger("3_Damaged");
        StartCoroutine(PlayerFlash());
        AudioManager.Instance.PlayPlayerHit();
        // 3) 사망 체크
        if (health == 0)
            StartCoroutine(PlayerDie());
    }

    private IEnumerator PlayerFlash()
    {
        // 빨강으로
        foreach (var sr in _playerRenderers)
            sr.color = Color.red;
        yield return new WaitForSeconds(0.4f);
        // 원래색으로 복구
        for (int i = 0; i < _playerRenderers.Length; i++)
            _playerRenderers[i].color = _origColors[i];
    }

    private IEnumerator PlayerDie()
    {
        // 애니메이터 사망 파라미터
        if (playerAnim != null)
        {
            playerAnim.SetBool("isDeath", true);
            playerAnim.SetTrigger("4_Death");
        }
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlayPlayerDeath();
        // 결과 씬 로드
        SceneManager.LoadScene("ResultScene");
    }

    private void ShowResult()
    {
        // ResultUI 컴포넌트를 찾아서 최종 스탯을 전달
        var ui = Object.FindFirstObjectByType<ResultUI>();
        if (ui != null)
            ui.SetStats(level, kill, GameTime);
        else
            Debug.LogError("[GameManager] ResultUI를 찾을 수 없습니다!");
    }

}
