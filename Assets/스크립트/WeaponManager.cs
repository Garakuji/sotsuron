using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    [Header("무기 설치 위치 (플레이어 트랜스폼)")]
    public Transform weaponParent;


    // 현재 장착된 무기 리스트
    private List<Weapon> weapons = new List<Weapon>();

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 무기 추가 또는 중복 시 레벨업 처리
    /// </summary>
    public void AddOrLevelupWeapon(WeaponData data)
    {
        if (data == null)
        {
            Debug.LogError("[WeaponManager] WeaponData가 null입니다.");
            return;
        }

        // 1) 이미 같은 ID가 있으면 레벨업
        Weapon existing = weapons.Find(w => w.id == data.id);
        if (existing != null)
        {
            existing.Levelup(existing.damage + 5f, existing.count + 1);
            return;
        }

        // 2) 새 무기 인스턴스 생성
        GameObject go = Instantiate(data.prefab, weaponParent);
        Weapon w = go.GetComponent<Weapon>();
        w.id = data.id;
        w.prefabId = data.prefabId;
        w.player = weaponParent;  // Weapon 스크립트에서 player Transform 사용 시
        w.Init();
        weapons.Add(w);

    }
}
