// WeaponSelectorUI.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponSelectorUI : MonoBehaviour
{
    [Header("무기 선택 슬롯 (Inspector에 연결된 슬롯 개수만큼)")]
    [SerializeField] private WeaponSlotUI[] slotUIs;
    private const int SLOT_COUNT = 3;
    private bool isChoosing = false;

    void Start()
    {
        Hide();
    }

    /// <summary>
    /// 레벨업 혹은 신규 획득 UI를 띄웁니다.
    /// </summary>
    public void Show()
    {
        if (isChoosing) return;

        // 1) 소지 무기 / 풀
        var owned = WeaponManager.Instance.GetAllWeapons();
        var allPool = GameManager.Instance.allWeaponData;

        // 2) 후보 추리기
        List<WeaponData> candidates;
        if (owned.Count < WeaponManager.Instance.maxWeaponCount)
        {
            // 슬롯에 여유가 있을 때:
            //   - 아직 안 가진 무기 OR 가진 무기라도 레벨업 여지 있을 때
            candidates = allPool
                .Where(d =>
                {
                    var w = owned.FirstOrDefault(x => x.id == d.id);
                    return w == null || w.level < Weapon.maxLevel;
                })
                .ToList();
        }
        else
        {
            // 슬롯이 가득 찼을 때:
            //   - 가진 무기 중 레벨업 가능한 것만
            candidates = owned
                .Where(w => w.level < Weapon.maxLevel)
                .Select(w => allPool.FirstOrDefault(d => d.id == w.id))
                .Where(d => d != null)
                .ToList();
        }

        // 3) 후보가 아무도 없으면 UI 띄우지 않고 그냥 리턴
        if (candidates.Count == 0)
        {
            Debug.Log("[WeaponSelectorUI] 레벨업 / 신규 획득 가능한 무기가 없습니다.");
            return;
        }

        // 4) 셔플 후 최대 SLOT_COUNT개 추출
        var finalChoices = candidates
            .OrderBy(_ => Random.value)
            .Take(SLOT_COUNT)
            .ToList();

        // 5) UI 활성화 & 게임 일시정지
        isChoosing = true;
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 6) 슬롯 세팅
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

        // 실제 획득 or 레벨업
        WeaponManager.Instance.AddOrLevelupWeapon(data);

        Hide();
    }

    private void Hide()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.SetActive(false);
    }
}
