using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private long id;

    public Image image;
    public TextMeshProUGUI countText;
    [HideInInspector]
    public Transform parentAfterDrag;
    [SerializeField]
    int count = 1;
    [SerializeField]
    bool stackable = true;
    public bool isStackable{ get{ return stackable; } }

    void Start(){
        image = GetComponent< Image >();
        countText = GetComponentInChildren< TextMeshProUGUI >();
        ChangeCount( 0 );
        if( !stackable ){ countText.gameObject.SetActive( false ); }
    }

    public void OnBeginDrag( PointerEventData eventData ){
        Debug.Log( "Begin Drag" );
        parentAfterDrag = transform.parent;
        transform.SetParent( transform.root );
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }
    public void OnDrag( PointerEventData eventData ){
        Debug.Log( "Dragging" );
        transform.position = Input.mousePosition;
    }
    public void OnEndDrag( PointerEventData eventData ){
        Debug.Log( "End Drag" );
        transform.SetParent( parentAfterDrag );
        image.raycastTarget = true;
    }
    
    public void ChangeCount( int amount ){
        count += amount;
        countText.SetText( count.ToString() );
    }

    public long GetId(){ return id; }
    public int GetCount(){ return count; }
}
