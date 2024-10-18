using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header( "UI" )]
    public Image image;
    public TextMeshProUGUI countText;

    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public Item item;
    private int _count = 1;
    public int count{ get{ return _count; } }

    public void InitializeItem( Item newItem ){
        item = newItem;
        image.sprite = newItem.image;
        // amount = 0 -> Refresh countText
        ChangeCount( 0 );
    }

    public void ChangeCount( int amount ){
        _count += amount;
        countText.SetText( _count.ToString() );
        if( _count > 1 ){
            countText.gameObject.SetActive( true );
        }
    }

    public void OnBeginDrag( PointerEventData eventData ){
        parentAfterDrag = transform.parent;
        transform.SetParent( transform.root );
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }
    public void OnDrag( PointerEventData eventData ){
        transform.position = Input.mousePosition;
    }
    public void OnEndDrag( PointerEventData eventData ){
        transform.SetParent( parentAfterDrag );
        image.raycastTarget = true;
    }
}
