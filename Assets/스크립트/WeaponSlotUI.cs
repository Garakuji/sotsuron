using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    public Image icon;
    private WeaponData weaponData;

    public void Set(WeaponData data)
    {
        weaponData = data;

        if (icon != null && data.icon != null)
            icon.sprite = data.icon;

        Debug.Log($"WeaponData Set 완료 - ID: {data.id}");

        // 혹시 모를 중복 리스너 방지
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (weaponData == null)
        {
            Debug.LogError("선택된 WeaponData가 null입니다.");
            return;
        }

        GameManager.Instance.weaponSelector.OnWeaponSelected(weaponData);
    }
}
