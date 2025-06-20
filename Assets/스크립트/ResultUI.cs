using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultUI : MonoBehaviour
{
    public Text levelText;
    public Text killsText;
    public Text timeText;
    public Button restartButton;

    void Start()
    {
        restartButton.onClick.AddListener(OnRestartButton);
    }

    /// <summary>
    /// GameManager���� ���� ������ �޾ƿͼ� ȭ�鿡 ǥ��
    /// </summary>
    public void SetStats(int level, int kills, float time)
    {
        levelText.text = $"Level: {level}";
        killsText.text = $"Kills: {kills}";
        int m = Mathf.FloorToInt(time / 60f);
        int s = Mathf.FloorToInt(time % 60f);
        timeText.text = $"Time: {m:D2}:{s:D2}";
    }

    /// <summary>
    /// �ٽ� ĳ���� ���� ������ ���ư���
    /// </summary>
    public void OnRestartButton()
    {
        // �ʱ�ȭ �ʿ��ϸ� GameManager.Instance �ʱ�ȭ �޼��� ȣ��
        SceneManager.LoadScene("CharacterSelectScene");
    }
}
