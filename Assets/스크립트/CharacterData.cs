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
    public WeaponData startingWeapon;   // 평소 캐릭터용 고정 무기
    public bool useRandomWeapon;  // true 면 랜덤 지급
}
