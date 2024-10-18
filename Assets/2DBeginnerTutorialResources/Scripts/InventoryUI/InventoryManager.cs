using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance{ get; private set; }
    public int maxStackedCount = 64;
    public InventorySlot[] inventorySlots;
    Dictionary<Item, List<InventorySlot>> slotDict = new Dictionary<Item, List<InventorySlot>>();
    public GameObject inventoryItemPrefab;

    int selectedSlot = -1;

    private void Awake(){
        instance = this;
    }

    private void Start(){
        ChangeSelectedSlot( 0 );
    }

    private void Update(){
        // 可直接在 out 參數宣告一個變數裝載此 out 內容
        if( int.TryParse( Input.inputString, out int number ) ){
            if( 0 < number && number < 9 ){
                ChangeSelectedSlot( number-1 );
            }
        }
    }

    void ChangeSelectedSlot( int value ){
        if( selectedSlot >= 0 ){ inventorySlots[ selectedSlot ].Deselect(); }
        inventorySlots[ value ].Select();
        selectedSlot = value;
    }

    public bool AddItem( Item item ){
        InventoryItem inventoryItem, newInventoryItem;
        int addCount = Random.Range( 1, 5 );
        int over = 0;

        // --- Dictionary Log ---
        // string log = "";
        // foreach( var key in slotDict.Keys ){
        //     log += key + ": ";
        //     log += "[";
        //     List<InventorySlot> slotList;
        //     if( slotDict.TryGetValue( key, out slotList ) ){
        //         foreach( var value in slotList ){
        //             log += value + ", ";
        //         }
        //         log = log.Substring( 0, log.Length-2 ); 
        //     }
        //     log += "], ";
        // }
        // if( log.Length != 0 ){ log = log.Substring( 0, log.Length-2 ); }
        // Debug.Log( "Dictionary.Keys[" + log + "]" );
        // ---------------------
        
        // Dictionary version
        // Check if any slot has the same item with count lower than max
        if( slotDict.TryGetValue( item, out List< InventorySlot > slots ) ){
            foreach( var slot in slots ){
                inventoryItem = slot.GetComponentInChildren< InventoryItem >();
                if( inventoryItem.count < maxStackedCount &&
                    inventoryItem.item.stackable ){
                    over = addCount+inventoryItem.count-maxStackedCount;
                    if( over > 0 ){
                        inventoryItem.ChangeCount( maxStackedCount-inventoryItem.count );
                        addCount = over;
                        continue;
                    }else{
                        inventoryItem.ChangeCount( addCount );
                        return true;
                    }
                }
            }
        }
        
        // Find any empty slot
        foreach( var slot in inventorySlots ){
            inventoryItem = slot.GetComponentInChildren< InventoryItem >();
            if( inventoryItem == null ){
                newInventoryItem = SpawnNewItem( item, slot );

                // new slot is added
                if( slots != null ){
                    slots.Add( slot );
                }else{
                    slotDict.Add( item, new List<InventorySlot>(){ slot } );
                }

                // over >  0: item exist but other slot is full.
                // over <= 0: item not exist.
                if( over > 0 ){
                    newInventoryItem.ChangeCount( over-1 );
                }else if( item.stackable ){
                    newInventoryItem.ChangeCount( Random.Range( 0, 4 ) );
                }
                return true;
            }
        }
        return false;
    }

    InventoryItem SpawnNewItem( Item item, InventorySlot slot ){
        GameObject newItem = Instantiate( inventoryItemPrefab, slot.transform );
        InventoryItem newInventoryItem = newItem.GetComponent< InventoryItem >();
        newInventoryItem.InitializeItem( item );
        return newInventoryItem;
    }

    public Item GetSelectedItem( bool use ){
        InventoryItem inventoryItem = inventorySlots[selectedSlot].GetComponentInChildren< InventoryItem >();
        if( inventoryItem != null ){
            if( use ){
                inventoryItem.ChangeCount( -1 );
                if( inventoryItem.count == 0 ){
                    if( slotDict.TryGetValue( inventoryItem.item, out List<InventorySlot> slots ) ){
                        slots.Remove( inventorySlots[selectedSlot] );
                    }
                    Destroy( inventoryItem.gameObject );
                }
            }
            return inventoryItem.item;
        }
        return null;
    }
    
    public void UpdateDictionaryByDrag( InventoryItem dragItem, InventorySlot dragSlot, InventorySlot dropSlot, bool isEmpty=true ){

        if( slotDict.TryGetValue( dragItem.item, out List<InventorySlot> slotList ) ){
            slotList.Remove( dragSlot );
            slotList.Add( dropSlot );
        }
        if( !isEmpty ){
            InventoryItem itemInSlot = dropSlot.GetComponentInChildren< InventoryItem >();
            if( slotDict.TryGetValue( itemInSlot.item, out slotList ) ){
                slotList.Remove( dropSlot );
                slotList.Add( dragSlot );
            }
        }
    }
}
