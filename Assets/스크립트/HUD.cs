using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health }
    public InfoType type;

    private Text myText;
    private Slider mySlider;

    private void Awake()
    {
        // 이 오브젝트에 붙어 있는 컴포넌트를 시도해서 가져옵니다.
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        if (GameManager.Instance == null) return;

        switch (type)
        {
            case InfoType.Exp:
                if (mySlider == null) return;    // Slider 없으면 스킵
                float curExp = GameManager.Instance.exp;
                int lvl = GameManager.Instance.level;
                int needExp = GameManager.Instance.GetRequiredExp(lvl);
                mySlider.value = curExp / (float)needExp;
                break;

            case InfoType.Level:
                if (myText == null) return;      // Text 없으면 스킵
                myText.text = $"Lv.{GameManager.Instance.level}";
                break;

            case InfoType.Kill:
                if (myText == null) return;
                myText.text = $"{GameManager.Instance.kill}";
                break;

            case InfoType.Time:
                if (myText == null) return;
                float elapsed = GameManager.Instance.GameTime;
                int min = Mathf.FloorToInt(elapsed / 60f);
                int sec = Mathf.FloorToInt(elapsed % 60f);
                myText.text = $"{min:D2}:{sec:D2}";
                break;

            case InfoType.Health:
                if (mySlider == null) return;
                float curHp = GameManager.Instance.health;
                float maxHp = GameManager.Instance.maxHealth;
                mySlider.value = curHp / maxHp;
                break;
        }
    }
}
