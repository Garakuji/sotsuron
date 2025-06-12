using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectorUI : MonoBehaviour
{
    public Weapon weapon;
    public WeaponSlotUI[] slotUIs; // Inspector�� ������ 3�� ����

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

    private void Start()
    {
        Hide();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnWeaponSelected(WeaponData data)
    {
        // ���ο� Weapon ������Ʈ ����
        GameObject newWeaponObj = new GameObject("Weapon_" + data.id);
        newWeaponObj.transform.SetParent(GameManager.Instance.player.transform);
        newWeaponObj.transform.localPosition = Vector3.zero;

        Weapon weapon = newWeaponObj.AddComponent<Weapon>();
        weapon.player = GameManager.Instance.player.transform;
        weapon.id = data.id;
        weapon.prefabId = data.prefabId;
        weapon.Init();

        Hide();
        Time.timeScale = 1f;
    }


}
