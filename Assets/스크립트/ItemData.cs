using UnityEngine;

public abstract class ItemDataBase : ScriptableObject
{
    public int id;
    public Sprite icon;
    [TextArea] public string description;
}

// ȸ�� ���븸 RecoveryItemData
[CreateAssetMenu(menuName = "Data/RecoveryItem")]
public class RecoveryItemData : ItemDataBase
{
    public int recoveryAmount;
}

// ���� �ٸ� ȿ�� �����ۿ�
[CreateAssetMenu(menuName = "Data/BuffItem")]
public class BuffItemData : ItemDataBase
{
    public float buffDuration;
    public float buffStrength;
}
