using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;

    private void Awake(){
        Deselect();
    }
    
    public void Select(){
        image.color = selectedColor;
    }

    public void Deselect(){
        image.color = notSelectedColor;
    }

    public void OnDrop( PointerEventData eventData ){
        InventoryManager manager = InventoryManager.instance;
        if( transform.childCount == 0 ){
            GameObject dropped = eventData.pointerDrag;
            // DraggableItem draggableItem = dropped.GetComponent< DraggableItem >();
            // draggableItem.parentAfterDrag = transform;
            InventoryItem droppedItem = dropped.GetComponent<InventoryItem>();
            manager.UpdateDictionaryByDrag( droppedItem, droppedItem.parentAfterDrag.GetComponent<InventorySlot>(), this );
            droppedItem.parentAfterDrag = transform;
        }else{
            GameObject dropped = eventData.pointerDrag;
            InventoryItem droppedItem = dropped.GetComponent<InventoryItem>();
            InventoryItem itemInSlot = GetComponentInChildren< InventoryItem >();
            if( droppedItem.item.Equals( itemInSlot.item ) ){
                if( !droppedItem.item.stackable ){
                    itemInSlot.transform.SetParent( droppedItem.parentAfterDrag );
                    droppedItem.parentAfterDrag = transform;
                }else{
                    int over = droppedItem.count+itemInSlot.count-manager.maxStackedCount;
                    if( over <= 0 ){
                        manager.UpdateDictionaryByDrag( droppedItem, droppedItem.parentAfterDrag.GetComponent<InventorySlot>(), this );
                        droppedItem.parentAfterDrag = transform;
                    }else{
                        itemInSlot.ChangeCount( manager.maxStackedCount-itemInSlot.count );
                        droppedItem.ChangeCount( -droppedItem.count+over );
                    }
                }
            }else{
                manager.UpdateDictionaryByDrag( droppedItem, droppedItem.parentAfterDrag.GetComponent<InventorySlot>(), this, false );
                itemInSlot.transform.SetParent( droppedItem.parentAfterDrag );
                droppedItem.parentAfterDrag = transform;
            }
        }
    }
}
