using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RubyInventorySlot : MonoBehaviour, IDropHandler
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
        RubyInventoryManager manager = RubyInventoryManager.instance;
        if( transform.childCount == 0 ){
            GameObject dropped = eventData.pointerDrag;
            RubyInventoryItem droppedItem = dropped.GetComponent<RubyInventoryItem>();
            manager.UpdateDictionaryByDrag( droppedItem, droppedItem.parentAfterDrag.GetComponent<RubyInventorySlot>(), this );
            droppedItem.parentAfterDrag = transform;
        }else{
            GameObject dropped = eventData.pointerDrag;
            RubyInventoryItem droppedItem = dropped.GetComponent<RubyInventoryItem>();
            RubyInventoryItem itemInSlot = GetComponentInChildren< RubyInventoryItem >();
            if( droppedItem.item.Equals( itemInSlot.item ) ){
                if( !droppedItem.item.stackable ){
                    itemInSlot.transform.SetParent( droppedItem.parentAfterDrag );
                    droppedItem.parentAfterDrag = transform;
                }else{
                    int over = droppedItem.count+itemInSlot.count-manager.maxStackedCount;
                    if( over <= 0 ){
                        manager.UpdateDictionaryByDrag( droppedItem, droppedItem.parentAfterDrag.GetComponent<RubyInventorySlot>(), this );
                        droppedItem.parentAfterDrag = transform;
                    }else{
                        itemInSlot.ChangeCount( manager.maxStackedCount-itemInSlot.count );
                        droppedItem.ChangeCount( -droppedItem.count+over );
                    }
                }
            }else{
                manager.UpdateDictionaryByDrag( droppedItem, droppedItem.parentAfterDrag.GetComponent<RubyInventorySlot>(), this, false );
                itemInSlot.transform.SetParent( droppedItem.parentAfterDrag );
                droppedItem.parentAfterDrag = transform;
            }
        }
    }
}
