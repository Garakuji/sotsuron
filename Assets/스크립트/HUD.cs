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
        // �� ������Ʈ�� �پ� �ִ� ������Ʈ�� �õ��ؼ� �����ɴϴ�.
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        if (GameManager.Instance == null) return;

        switch (type)
        {
            case InfoType.Exp:
                if (mySlider == null) return;    // Slider ������ ��ŵ
                float curExp = GameManager.Instance.exp;
                int lvl = GameManager.Instance.level;
                int needExp = GameManager.Instance.GetRequiredExp(lvl);
                mySlider.value = curExp / (float)needExp;
                break;

            case InfoType.Level:
                if (myText == null) return;      // Text ������ ��ŵ
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
