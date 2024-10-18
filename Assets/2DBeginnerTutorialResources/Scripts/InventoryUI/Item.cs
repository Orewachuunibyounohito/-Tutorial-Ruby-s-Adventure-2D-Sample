using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu( menuName = "Scriptable Object/Item" )]
public class Item: ScriptableObject
{
    [Header( "Only gameplay" )]
    [SerializeField]
    private long _id;
    public long id{ get{ return _id; } }
    public TileBase tile;
    public ItemType itemType;
    public ActionType actionType;
    public Vector2Int range = new Vector2Int( 5, 4 );

    [Header( "Only in UI" )]
    public bool stackable = true;

    [Header( "Both" )]
    public Sprite image;

    public enum ItemType{ BuildingBlock, Tool }

    public enum ActionType{ Dig, Mine }

    override public string ToString(){
        return name + "[itemType=" + itemType + ", actionType=" + actionType + ", stackable=" + stackable + " ]";
    }
}
