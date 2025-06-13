// WeaponAssigner.cs
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class WeaponAssigner : MonoBehaviour
{
    [Header("랜덤 지급할 무기 Data 리스트")]
    public WeaponData[] allWeaponDatas;  // 에디터에서 모든 무기 Data 에셋을 등록

    void Start()
    {
        if (allWeaponDatas == null || allWeaponDatas.Length == 0)
        {
            Debug.LogError("[WeaponAssigner] allWeaponDatas가 비어 있습니다.");
            return;
        }

        // 0~(Length-1) 사이 랜덤 인덱스
        int idx = Random.Range(0, allWeaponDatas.Length);
        WeaponData data = allWeaponDatas[idx];

        // WeaponManager에게 위임하면, 자동으로 Instantiate 및 Init 해 줌
        WeaponManager.Instance.AddOrLevelupWeapon(data);
    }
}
