using UnityEngine;

[CreateAssetMenu( menuName = "Scriptable Object/RubyItem" )]
public class RubyItem : ScriptableObject
{
    public enum ItemType{ Weapon, Consumables, Sundries }
    public enum ActionType{ Attack, Eat }

    [Header( "Only Gameplay" )]
    public Sprite image;
    public GameObject itemPrefab;
    public ItemType itemType;
    public ActionType actionType;

    [Header( "Only UI" )]
    public bool stackable;
    public bool hasEffect;

}
