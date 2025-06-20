using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponSelectorUI : MonoBehaviour
{
    [Header("무기 선택 슬롯 (Inspector에 3개 연결)")]
    public WeaponSlotUI[] slotUIs;
    private const int SLOT_COUNT = 3;
    private bool isChoosing = false;

    void Start()
    {
        Hide();
    }

    /// <summary>
    /// 슬롯을 열어 보여 줍니다. allWeaponData를 내부에서 직접 참조
    /// </summary>
    public void Show()
    {
        // 1) 전체 무기 데이터 풀
        var allWeaponPool = GameManager.Instance.allWeaponData;

        // 2) 플레이어 소지 무기 리스트
        var owned = WeaponManager.Instance.GetAllWeapons();

        // 3) 후보 결정
        List<WeaponData> candidates;
        if (owned.Count < WeaponManager.Instance.maxWeaponCount)
        {
            candidates = allWeaponPool.ToList();
        }
        else
        {
            candidates = owned
                .Where(w => w.level < Weapon.maxLevel)
                .Select(w => allWeaponPool.FirstOrDefault(d => d.id == w.id))
                .Where(d => d != null)
                .ToList();
        }

        // 4) 랜덤 섞고 최대 3개만
        var finalChoices = candidates
            .OrderBy(_ => Random.value)
            .Take(SLOT_COUNT)
            .ToList();

        // 5) 비어 있으면 닫기
        if (finalChoices.Count == 0)
        {
            Hide();
            return;
        }

        // 6) UI 띄우기 & 게임 정지
        isChoosing = true;
        gameObject.SetActive(true);
        Time.timeScale = 0f;

        // 7) 슬롯 세팅 (나머지는 숨김)
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            if (i < finalChoices.Count)
            {
                slotUIs[i].gameObject.SetActive(true);
                slotUIs[i].SetWeapon(finalChoices[i]);
            }
            else
            {
                slotUIs[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnWeaponSelected(WeaponData data)
    {
        if (!isChoosing) return;
        isChoosing = false;

        // 무기 획득 / 레벨업
        WeaponManager.Instance.AddOrLevelupWeapon(data);

        // 선택창 닫기 & 시간 복귀
        Hide();
    }

    public void Hide()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}
