using UnityEngine;

public class WeaponAssigner : MonoBehaviour
{
    public Weapon weapon;
    public int maxWeaponCount = 5; // ���� Bullet 0~4 ���� �����ϹǷ� 5��

    void Start()
    {
        int randomId = Random.Range(0, maxWeaponCount);
        weapon.id = randomId;
        weapon.prefabId = randomId;

        weapon.Init(); // ���� ���� �ʱ�ȭ
    }
}
