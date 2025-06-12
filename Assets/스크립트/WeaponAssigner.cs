using UnityEngine;

public class WeaponAssigner : MonoBehaviour
{
    public Weapon weapon;
    public int maxWeaponCount = 5; // 현재 Bullet 0~4 까지 존재하므로 5개

    void Start()
    {
        int randomId = Random.Range(0, maxWeaponCount);
        weapon.id = randomId;
        weapon.prefabId = randomId;

        weapon.Init(); // 무기 설정 초기화
    }
}
