using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class WeaponSlotUI : MonoBehaviour
{
    public Image icon;
    public Text descriptionText;

    private Button selectorBtn;
    private WeaponSelectorUI selector;  // 부모 UI

    void Awake()
    {
        selectorBtn = GetComponent<Button>();
        selector = GetComponentInParent<WeaponSelectorUI>();
    }

    public void SetWeapon(WeaponData data)
    {
        // 아이콘·텍스트 세팅
        icon.sprite = data.icon;
        descriptionText.text = data.description;

        // 클릭 리스너: 절대 AddOrLevelupWeapon 직접 호출 금지!
        selectorBtn.onClick.RemoveAllListeners();
        selectorBtn.onClick.AddListener(() =>
        {
            // 1) 클릭 음성 재생
            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuSelectClip);

            // 2) 레벨업 UI 동작
            selector.OnWeaponSelected(data);
        });

        selectorBtn.interactable = true;
    }

    public void Clear()
    {
        selectorBtn.onClick.RemoveAllListeners();
        selectorBtn.interactable = false;
        icon.sprite = null;
        descriptionText.text = "";
    }
}
