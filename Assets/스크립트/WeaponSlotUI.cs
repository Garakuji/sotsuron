using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    public Image icon;
    private WeaponData weaponData;

    public void Set(WeaponData data)
    {
        weaponData = data;
        icon.sprite = data.icon;
        gameObject.SetActive(true);
    }

    public void OnClick()
    {
        GameManager.Instance.weaponSelector.OnWeaponSelected(weaponData);
    }
}
