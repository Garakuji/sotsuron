using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health }
    public InfoType type;

    private Text myText;
    private Slider mySlider;

    private void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Exp:
                // ���� ����ġ
                float curExp = GameManager.Instance.exp;
                // ���� �������� �ʿ��� ����ġ (�Լ� ȣ��)
                int lvl = GameManager.Instance.level;
                int needExp = GameManager.Instance.GetRequiredExp(lvl);
                // �����̴��� ���� ����
                mySlider.value = curExp / (float)needExp;
                break;

            case InfoType.Level:
                myText.text = string.Format("Lv.{0}", GameManager.Instance.level);
                break;

            case InfoType.Kill:
                myText.text = string.Format("{0}", GameManager.Instance.kill);
                break;

            case InfoType.Time:
                float elapsed = GameManager.Instance.GameTime;
                int min = Mathf.FloorToInt(elapsed / 60f);
                int sec = Mathf.FloorToInt(elapsed % 60f);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;

            case InfoType.Health:
                float curHp = GameManager.Instance.health;
                float maxHp = GameManager.Instance.maxHealth;
                mySlider.value = curHp / maxHp;
                break;
        }
    }
}
