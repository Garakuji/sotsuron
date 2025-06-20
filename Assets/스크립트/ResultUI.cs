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
    /// GameManager에서 최종 스탯을 받아와서 화면에 표시
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
    /// 다시 캐릭터 선택 씬으로 돌아가기
    /// </summary>
    public void OnRestartButton()
    {
        // 초기화 필요하면 GameManager.Instance 초기화 메서드 호출
        SceneManager.LoadScene("CharacterSelectScene");
    }
}
