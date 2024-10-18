using UnityEngine;

public class SearchByRaycast : MonoBehaviour
{
    public GameObject[] searchable;
    string[] rayMask;

    public GameObject searchedObject{ get{ return searched; } }
    GameObject searched = null;
    public GameObject target;

    [Range( 0, 360 )]
    public float searchAngle = 60.0f;   // will search -15~15 degree
    float angleThreshold = 20.0f; // When deltaAngle < Threshold, add a ray with deltaAngle/2, deltaAngle is from this ray to nearby ray
    public float searchRadius = 5.0f;

    public Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {   
        // foreach( var obj in searchable ){
        //     if( rayMask == 0 ){ rayMask = obj.layer; }
        //     else              { rayMask &= obj.layer; }
        // }
        // Debug.Log( rayMask == LayerMask.GetMask( "Character", "Obstacle" ) );
        rayMask = new string[ searchable.Length ];
        for( int idx = 0; idx < searchable.Length; idx++ ){
            rayMask[idx] = LayerMask.LayerToName( searchable[idx].layer );
        }
        // Debug.Log( LayerMask.GetMask( rayMask ) == LayerMask.GetMask( "Character", "Obstacle" ) );
        direction = GetComponent< Rigidbody2D >().velocity.normalized;
        if( direction.Equals( Vector2.zero ) ){ direction = Vector2.down; }
    }

    // Update is called once per frame
    void Update()
    {
        if( !enabled ){ return; }
        Searching();
    }

    public void Searching(){
        Vector2 beginDirection = direction;
        Vector2 endDirection = direction;
        beginDirection = new Vector2( Mathf.Cos( Mathf.Acos( beginDirection.x )-searchAngle*Mathf.Deg2Rad ), Mathf.Sin( Mathf.Asin( beginDirection.y )-searchAngle*Mathf.Deg2Rad ) );
        // beginDirection /= 2;
        endDirection = new Vector2( Mathf.Cos( Mathf.Acos( endDirection.x )+searchAngle*Mathf.Deg2Rad ), Mathf.Sin( Mathf.Asin( endDirection.y )+searchAngle*Mathf.Deg2Rad ) );
        // endDirection /= 2;
        // Debug.Log( "begin: " + Mathf.Acos( beginDirection.x )*Mathf.Rad2Deg + "\nend: " + Mathf.Acos( endDirection.x )*Mathf.Rad2Deg );
        Debug.Log( "direction: " + Mathf.Asin( direction.y )*Mathf.Rad2Deg );
        RaycastHit2D hit = Physics2D.Raycast( transform.position, beginDirection, searchRadius, LayerMask.GetMask( rayMask ) );
        if( hit.collider != null ){
            if( hit.collider.gameObject.Equals( target ) ){
                searched = hit.collider.gameObject;
                return;
            }
        }
        hit = Physics2D.Raycast( transform.position, endDirection, searchRadius, LayerMask.GetMask( rayMask ) );
        if( hit.collider != null ){
            if( hit.collider.gameObject.Equals( target ) ){
                searched = hit.collider.gameObject;
                return;
            }
        }
        searched = _Searching( target, beginDirection, endDirection );
    }
    GameObject _Searching( GameObject target, Vector2 beginDirection, Vector2 endDirection ){
        GameObject left = null;
        GameObject right = null;
        float angle = Vector2.SignedAngle( beginDirection, endDirection );
        if( angle < 0.0f ){ angle += 360; }
        if( angle >= angleThreshold ){
            left = _Searching( target, beginDirection, ( beginDirection+endDirection )/2 );
            if( left != null ){
                if( left.Equals( target ) ){ return left; }
            }
            right = _Searching( target, ( beginDirection+endDirection )/2, endDirection );
            if( right != null ){
                if( right.Equals( target ) ){ return right; }
            }
        }
        return null;
    }
}
