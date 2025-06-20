using UnityEngine;
using UnityEngine.UI;

public class CharacterSlotUI : MonoBehaviour
{
    public Image icon;
    public Text nameText;

    private CharacterData data;
    private Button btn;

    void Awake()
    {
        btn = GetComponent<Button>();
    }

    public void Set(CharacterData cd)
    {
        data = cd;
        icon.sprite = cd.icon;
        nameText.text = cd.characterName;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClick);
        btn.interactable = true;
    }

    public void Clear()
    {
        btn.onClick.RemoveAllListeners();
        btn.interactable = false;
        icon.sprite = null;
        nameText.text = "";
    }

    private void OnClick()
    {
        AudioManager.Instance.PlayMenuSelect();
        CharacterSelectorUI selector = GetComponentInParent<CharacterSelectorUI>();
        selector.Select(data);
    }
}
