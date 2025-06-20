using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectorUI : MonoBehaviour
{
    public List<CharacterData> allCharacters;  // Inspector�� ���µ� �巡��
    public CharacterSlotUI[] slots;         // ������ ���� ���� UI�� ����

    void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < allCharacters.Count)
                slots[i].Set(allCharacters[i]);
            else
                slots[i].Clear();
        }
    }

    public void Select(CharacterData data)
    {
        GameManager.Instance.selectedCharacter = data;
        SceneManager.LoadScene("GameScene");
    }
}
