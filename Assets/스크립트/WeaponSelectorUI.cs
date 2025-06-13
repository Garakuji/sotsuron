// WeaponSelectorUI.cs
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectorUI : MonoBehaviour
{
    public WeaponData[] allWeaponDatas;
    public WeaponSlotUI[] slotUIs;

    void Start()
    {
        Hide();
    }

    public void Show(List<WeaponData> choices)
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;

        for (int i = 0; i < slotUIs.Length; i++)
        {
            if (i < choices.Count)
                slotUIs[i].Set(choices[i]);
            else
                slotUIs[i].gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        // UI 닫힐 때마다 Time.timeScale 복구
        Time.timeScale = 1f;
    }

    public void OnWeaponSelected(WeaponData data)
    {
        if (data == null) return;

        // ① UI 숨기기 + 시간 복구
        Hide();

        // ② 무기 추가 / 레벨업 로직 전부 WeaponManager에 위임
        WeaponManager.Instance.AddOrLevelupWeapon(data);
    }
}
