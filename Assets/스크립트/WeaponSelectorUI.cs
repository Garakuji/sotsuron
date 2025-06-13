using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectorUI : MonoBehaviour
{
    public Weapon weapon;  // Inspector에서 할당
    public WeaponSlotUI[] slotUIs;

    void Start()
    {
        Hide();
    }
    public void OnWeaponSelected(WeaponData data)
    {
        if (data == null) { Debug.LogError("WeaponData null"); return; }
        
        if (data == null)
        {
            Debug.LogError("선택된 WeaponData가 null입니다.");
            return;
        }

        if (weapon == null)
        {
            weapon = FindAnyObjectByType<Weapon>();
            if (weapon == null)
            {
                Debug.LogError("Weapon 오브젝트를 찾을 수 없습니다.");
                return;
            }
        }


        if (weapon.id == data.id)
        {
            Debug.Log("이미 이 무기 ID가 설정되어 있습니다. 레벨업만 합니다.");
            weapon.Levelup(weapon.damage + 5f, weapon.count + 1);
        }
        else
        {
            weapon.id = data.id;
            weapon.prefabId = data.prefabId;
            weapon.Init();
        }
            Time.timeScale = 1f;
            Hide();

            // WeaponManager에 위임
            WeaponManager.Instance.AddOrLevelupWeapon(data);
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
        Time.timeScale = 1f;
    }

}
