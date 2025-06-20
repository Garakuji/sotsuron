using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;  // LINQ ���

[System.Serializable]
public class OwnedWeaponSlot
{
    public GameObject slotObj;
    public Image iconImage;

    public void Set(WeaponData data)
    {
        if (slotObj != null)
            slotObj.SetActive(true);
        if (iconImage != null && data != null)
            iconImage.sprite = data.icon;
    }

    public void Clear()
    {
        if (slotObj != null)
            slotObj.SetActive(false);
    }
}

public class OwnedWeaponsPanel : MonoBehaviour
{
    public OwnedWeaponSlot[] slots;

    void Update()
    {
        // ���� ���� ����Ʈ �����ͼ� ID ������ ����
        List<Weapon> owned = WeaponManager.Instance.GetAllWeapons()
            .OrderBy(w => w.id)
            .ToList();

        // ���Ժ��� ����
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < owned.Count)
            {
                Weapon w = owned[i];
                // WeaponData �������� (GameManager���� �ҷ���)
                WeaponData data = GameManager.Instance.allWeaponData
                    .FirstOrDefault(d => d.id == w.id);
                slots[i].Set(data);
            }
            else
            {
                slots[i].Clear();
            }
        }
    }
}
