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
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 새 무기 추가 또는 중복 무기 레벨업
    /// (부모 자식 관계 없이 월드 상에 스폰)
    /// </summary>
    public void AddOrLevelupWeapon(WeaponData data)
    {
        if (data == null)
        {
            Debug.LogError("[WeaponManager] WeaponData가 null입니다.");
            return;
        }

        // 1) 이미 같은 ID가 있으면 레벨업만
        Weapon existing = weapons.Find(w => w.id == data.id);
        if (existing != null)
        {
            existing.Levelup();
            return;
        }

        // 2) 부모 지정 없이 월드에 인스턴스 생성
        GameObject go = Instantiate(data.prefab);
        Weapon w = go.GetComponent<Weapon>();
        if (w == null)
        {
            Debug.LogError("[WeaponManager] Prefab에 Weapon 컴포넌트가 없습니다.");
            Destroy(go);
            return;
        }

        // 3) 필수 초기 세팅
        w.id = data.id;
        w.prefabId = data.prefabId;
        w.player = weaponParent;   // 내부 로직에서 player 참조로 사용

        // 4) 피봇 위치(플레이어 기준 높이 오프셋)로 이동
        Vector3 pivot = weaponParent.position + Vector3.up * w.pivotYOffset;
        go.transform.position = pivot;

        // 5) 리스트에 추가
        weapons.Add(w);
    }

    public Weapon GetWeaponById(int id)
    {
        return weapons.Find(w => w.id == id);
    }
}
