using UnityEngine;

public class DemoScript : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Item[] itemToPickup;

    public void PickupItem( int id ){
        bool isFull = !inventoryManager.AddItem( itemToPickup[id] );

        if( isFull ){
            Debug.Log( "I cannot take anymore." );
        }
    }

    public void GetSelectedItem(){
        Item receivedItem = inventoryManager.GetSelectedItem( false );
        if( receivedItem != null ){
            Debug.Log( "Get item: " + receivedItem.name );
        }else{
            Debug.Log( "No item received." );
        }
    }
    public void UseSelectedItem(){
        Item receivedItem = inventoryManager.GetSelectedItem( true );
        if( receivedItem != null ){
            Debug.Log( "Used item: " + receivedItem.name );
        }else{
            Debug.Log( "Not used item." );
        }
    }
}
