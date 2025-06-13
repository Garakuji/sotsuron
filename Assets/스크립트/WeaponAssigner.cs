using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [Header("무기 선택용 데이터 리스트")]
    public WeaponData[] allWeaponDatas;   // 에디터에서 각종 무기 Data 에셋을 등록

    [Header("생성된 무기를 붙일 부모 (플레이어)")]
    public Transform weaponParent;

    void Start()
    {
        if (allWeaponDatas == null || allWeaponDatas.Length == 0)
        {
            Debug.LogError("[WeaponSpawner] allWeaponDatas가 비어 있습니다.");
            return;
        }

        // 1) 랜덤 Data 고르기
        int idx = Random.Range(0, allWeaponDatas.Length);
        WeaponData data = allWeaponDatas[idx];

        // 2) 프리팹 Instantiate
        GameObject go = Instantiate(data.prefab, weaponParent.position, Quaternion.identity, weaponParent);
        Weapon w = go.GetComponent<Weapon>();
        if (w == null)
        {
            Debug.LogError("[WeaponSpawner] Weapon 컴포넌트가 없습니다.");
            return;
        }

        // 3) ID/PF 설정 및 초기화
        w.id = data.id;
        w.prefabId = data.prefabId;
        w.player = weaponParent;
        w.Init();
        w.ResetOffset();
    }
}
