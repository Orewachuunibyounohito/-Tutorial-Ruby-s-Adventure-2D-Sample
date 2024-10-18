using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyInventoryManager : MonoBehaviour
{
    public static RubyInventoryManager instance{ get; private set; }
    public int maxStackedCount = 64;
    public RubyInventorySlot[] inventorySlots;
    Dictionary<RubyItem, List<RubyInventorySlot>> slotDict = new Dictionary<RubyItem, List<RubyInventorySlot>>();
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

    public bool AddItem( RubyItem item, int count=1 ){
        RubyInventoryItem inventoryItem, newInventoryItem;
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
        if( slotDict.TryGetValue( item, out List< RubyInventorySlot > slots ) ){
            foreach( var slot in slots ){
                inventoryItem = slot.GetComponentInChildren< RubyInventoryItem >();
                if( inventoryItem.count < maxStackedCount &&
                    inventoryItem.item.stackable ){
                    over = count+inventoryItem.count-maxStackedCount;
                    if( over > 0 ){
                        inventoryItem.ChangeCount( maxStackedCount-inventoryItem.count );
                        count = over;
                        continue;
                    }else{
                        inventoryItem.ChangeCount( count );
                        return true;
                    }
                }
            }
        }
        
        // Find any empty slot
        foreach( var slot in inventorySlots ){
            inventoryItem = slot.GetComponentInChildren< RubyInventoryItem >();
            if( inventoryItem == null ){
                newInventoryItem = SpawnNewItem( item, slot );

                // new slot is added
                if( slots != null ){
                    slots.Add( slot );
                }else{
                    slotDict.Add( item, new List<RubyInventorySlot>(){ slot } );
                }

                // over >  0: item exist but other slot is full.
                // over <= 0: item not exist.
                if( over > 0 ){
                    newInventoryItem.ChangeCount( over-1 );
                }else if( item.stackable ){
                    // newInventoryItem.ChangeCount( Random.Range( 0, 4 ) );
                }
                return true;
            }
        }
        return false;
    }

    RubyInventoryItem SpawnNewItem( RubyItem item, RubyInventorySlot slot ){
        GameObject newItem = Instantiate( inventoryItemPrefab, slot.transform );
        RubyInventoryItem newInventoryItem = newItem.GetComponent< RubyInventoryItem >();
        newInventoryItem.InitializeItem( item );
        return newInventoryItem;
    }

    public RubyItem GetSelectedItem( bool use ){
        RubyInventoryItem inventoryItem = inventorySlots[selectedSlot].GetComponentInChildren< RubyInventoryItem >();
        if( inventoryItem != null ){
            if( use ){
                if( inventoryItem.item.hasEffect ){
                    // -- Start effect --
                    StartCoroutine( inventoryItem.item.itemPrefab.GetComponent< IItemEffect >().Effect( RubyController.instance.gameObject ) );
                }else{
                    return inventoryItem.item;
                }
                inventoryItem.ChangeCount( -1 );

                if( inventoryItem.count == 0 ){
                    // --- Update Dictionary
                    if( slotDict.TryGetValue( inventoryItem.item, out List<RubyInventorySlot> slots ) ){
                        slots.Remove( inventorySlots[selectedSlot] );
                    }
                    // ---
                    Destroy( inventoryItem.gameObject );
                }
            }
            return inventoryItem.item;
        }
        return null;
    }

    public RubyItem ThrowSelectedItem( Transform thrower, Vector2 direction ){
        RubyInventoryItem inventoryItem = inventorySlots[selectedSlot].GetComponentInChildren< RubyInventoryItem >();
        if( inventoryItem != null ){
            inventoryItem.ChangeCount( -1 );
            // a little range angle
            direction += new Vector2( Random.Range( -0.1f, 0.1f ), Random.Range( -0.1f, 0.1f ) );
            GameObject threwItem = Instantiate( inventoryItem.item.itemPrefab, (Vector2)thrower.position+direction.normalized, inventoryItem.item.itemPrefab.transform.rotation );
            threwItem.transform.localScale = Vector2.one*0.5f;
            StartCoroutine( LootDelay( threwItem.GetComponent< CollectibleItem >() ) );
            if( threwItem.GetComponent< Animator >() != null ){
                threwItem.GetComponent< Animator >().enabled = false;
            }
            Rigidbody2D itemRb = threwItem.GetComponent< Rigidbody2D >();
            // range force
            itemRb.AddForce( itemRb.mass*direction*Random.Range( 1f, 2f ), ForceMode2D.Impulse );

            if( inventoryItem.count == 0 ){
                // --- Update Dictionary
                if( slotDict.TryGetValue( inventoryItem.item, out List<RubyInventorySlot> slots ) ){
                    slots.Remove( inventorySlots[selectedSlot] );
                }
                // ---
                Destroy( inventoryItem.gameObject );
            }
            return inventoryItem.item;
        }
        return null;
    }

    IEnumerator LootDelay( CollectibleItem threwItem ){
        yield return new WaitForSeconds( 1.0f );
        threwItem.canLoot = true;
    }
    
    public void UpdateDictionaryByDrag( RubyInventoryItem dragItem, RubyInventorySlot dragSlot, RubyInventorySlot dropSlot, bool isEmpty=true ){

        if( slotDict.TryGetValue( dragItem.item, out List<RubyInventorySlot> slotList ) ){
            slotList.Remove( dragSlot );
            slotList.Add( dropSlot );
        }
        if( !isEmpty ){
            RubyInventoryItem itemInSlot = dropSlot.GetComponentInChildren< RubyInventoryItem >();
            if( slotDict.TryGetValue( itemInSlot.item, out slotList ) ){
                slotList.Remove( dropSlot );
                slotList.Add( dragSlot );
            }
        }
    }
}
