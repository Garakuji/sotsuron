using System.Collections.Generic;
using System.Linq;            // ★ LINQ 사용
using UnityEngine;

public class WeaponSelectorUI : MonoBehaviour
{
    public WeaponSlotUI[] slotUIs;

    void Start()
    {
        Hide();
    }

    public void Show(List<WeaponData> choices)
    {
        // 1) 이미 maxLevel에 도달한 무기는 제외
        var available = choices
            .Where(data =>
            {
                var w = WeaponManager.Instance.GetWeaponById(data.id);
                // 새로 얻는 경우(w==null)거나, 아직 maxLevel 아래인 경우만 남김
                return w == null || w.level < Weapon.maxLevel;
            })
            .ToList();

        // 2) 필터 후 남은 게 없으면 그냥 UI 닫기
        if (available.Count == 0)
        {
            Hide();
            return;
        }

        // 3) 화면에 표시
        gameObject.SetActive(true);
        Time.timeScale = 0f;

        for (int i = 0; i < slotUIs.Length; i++)
        {
            if (i < available.Count)
            {
                slotUIs[i].gameObject.SetActive(true);
                slotUIs[i].Set(available[i]);
            }
            else
            {
                slotUIs[i].gameObject.SetActive(false);
            }
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnWeaponSelected(WeaponData data)
    {
        if (data == null) return;

        Hide();
        WeaponManager.Instance.AddOrLevelupWeapon(data);
    }
}
