using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;  // LINQ 사용

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
        // 소유 무기 리스트 가져와서 ID 순으로 정렬
        List<Weapon> owned = WeaponManager.Instance.GetAllWeapons()
            .OrderBy(w => w.id)
            .ToList();

        // 슬롯별로 세팅
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < owned.Count)
            {
                Weapon w = owned[i];
                // WeaponData 가져오기 (GameManager에서 불러옴)
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
