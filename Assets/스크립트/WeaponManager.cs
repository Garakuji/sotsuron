using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    public Weapon weapon; // weapon.cs�� �پ��ִ� ������Ʈ
    private HashSet<int> ownedWeapons = new HashSet<int>();

    void Awake() => Instance = this;

    public void AddWeapon(int id, int prefabId)
    {
        if (ownedWeapons.Contains(id))
        {
            weapon.Levelup(weapon.damage + 5f, weapon.count + 1); // �ӽ� ����
        }
        else
        {
            weapon.id = id;
            weapon.prefabId = prefabId;
            weapon.Init();
            ownedWeapons.Add(id);
        }
    }

    public bool HasWeapon(int id)
    {
        return ownedWeapons.Contains(id);
    }
}
