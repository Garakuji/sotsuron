using UnityEngine;

[CreateAssetMenu(menuName = "Data/Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite icon;
    public GameObject prefab;
    public int maxHealth;
    public float moveSpeed;

    [Header("Starting Weapon")]
    public WeaponData startingWeapon;   // ��� ĳ���Ϳ� ���� ����
    public bool useRandomWeapon;  // true �� ���� ����
}
