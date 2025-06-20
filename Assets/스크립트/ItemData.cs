using UnityEngine;

public abstract class ItemDataBase : ScriptableObject
{
    public int id;
    public Sprite icon;
    [TextArea] public string description;
}

// 회복 전용만 RecoveryItemData
[CreateAssetMenu(menuName = "Data/RecoveryItem")]
public class RecoveryItemData : ItemDataBase
{
    public int recoveryAmount;
}

// 추후 다른 효과 아이템용
[CreateAssetMenu(menuName = "Data/BuffItem")]
public class BuffItemData : ItemDataBase
{
    public float buffDuration;
    public float buffStrength;
}
