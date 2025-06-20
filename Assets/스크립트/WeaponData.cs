using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    public int id;
    public int prefabId;
    public Sprite icon;
    [TextArea(2, 5)]
    public string description;
    public GameObject prefab;
}

