using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    public int id;
    public int prefabId;
    public string weaponName;
    public string description;
    public Sprite icon;
}

