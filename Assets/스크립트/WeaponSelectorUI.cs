using System.Collections.Generic;
using System.Linq;            // ★ LINQ 사용
using UnityEngine;

public class WeaponSelectorUI : MonoBehaviour
{
    public WeaponSlotUI[] slotUIs;
    private bool isChoosing = false;

    void Start()
    {
        Hide();
    }

    public void Show(List<WeaponData> allWeaponPool)
    {
        const int maxChoices = 3;
        List<WeaponData> finalChoices = new();

        // 1. 만렙이 아닌 무기 후보
        List<WeaponData> underMaxWeapons = allWeaponPool
            .Where(data =>
            {
                var w = WeaponManager.Instance.GetWeaponById(data.id);
                return w == null || w.level < Weapon.maxLevel;
            })
            .OrderBy(_ => Random.value)
            .ToList();

        // 2. 만렙 무기 후보
        List<WeaponData> maxedWeapons = allWeaponPool
            .Where(data =>
            {
                var w = WeaponManager.Instance.GetWeaponById(data.id);
                return w != null && w.level >= Weapon.maxLevel;
            })
            .OrderBy(_ => Random.value)
            .ToList();

        // 3. 우선 만렙 아닌 무기에서 채우기
        finalChoices.AddRange(underMaxWeapons.Take(maxChoices));

        // 4. 부족하면 만렙 무기 중 랜덤 추가
        if (finalChoices.Count < maxChoices)
        {
            int need = maxChoices - finalChoices.Count;
            finalChoices.AddRange(maxedWeapons.Take(need));
        }

        // 5. 선택지 없으면 닫기
        if (finalChoices.Count == 0)
        {
            Hide();
            return;
        }

        // 6. 표시
        isChoosing = true;
        gameObject.SetActive(true);
        Time.timeScale = 0f;

        for (int i = 0; i < slotUIs.Length; i++)
        {
            if (i < finalChoices.Count)
            {
                slotUIs[i].gameObject.SetActive(true);
                slotUIs[i].Set(finalChoices[i]);
            }
            else
            {
                slotUIs[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnWeaponSelected(WeaponData data)
    {
        if (!isChoosing) return;
        isChoosing = false;

        Hide();
        WeaponManager.Instance.AddOrLevelupWeapon(data);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

}
