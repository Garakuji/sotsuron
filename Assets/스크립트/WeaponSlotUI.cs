using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class WeaponSlotUI : MonoBehaviour
{
    public Image icon;
    public Text descriptionText;

    private Button selectorBtn;
    private WeaponSelectorUI selector;  // �θ� UI

    void Awake()
    {
        selectorBtn = GetComponent<Button>();
        selector = GetComponentInParent<WeaponSelectorUI>();
    }

    public void SetWeapon(WeaponData data)
    {
        // �����ܡ��ؽ�Ʈ ����
        icon.sprite = data.icon;
        descriptionText.text = data.description;

        // Ŭ�� ������: ���� AddOrLevelupWeapon ���� ȣ�� ����!
        selectorBtn.onClick.RemoveAllListeners();
        selectorBtn.onClick.AddListener(() =>
        {
            // 1) Ŭ�� ���� ���
            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuSelectClip);

            // 2) ������ UI ����
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
