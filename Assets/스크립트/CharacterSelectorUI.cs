using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectorUI : MonoBehaviour
{
    public List<CharacterData> allCharacters;  // Inspector에 에셋들 드래그
    public CharacterSlotUI[] slots;         // 위에서 만든 슬롯 UI들 연결

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
