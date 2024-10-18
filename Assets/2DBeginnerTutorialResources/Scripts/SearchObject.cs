using System.Collections.Generic;
using UnityEngine;

public class SearchObject : MonoBehaviour
{
    // Check target in targets, and then lock them.
    public GameObject[] targets;
    HashSet< string > tags = new HashSet< string >();
    public HashSet< GameObject > lockedObjects{ get{ return _lockedObjects; } }
    HashSet< GameObject > _lockedObjects = new HashSet< GameObject >();
    public float searchRange{ get{ return GetComponent< CircleCollider2D >().radius; } }
    public bool active = true;

    void Start(){
        foreach( var target in targets ){ tags.Add( target.tag ); }
    }

    void OnTriggerEnter2D( Collider2D collided ){
        if( !active ){ return; }
        if( tags.Contains( collided.tag ) ){
            if( collided != collided.GetComponent< CapsuleCollider2D >() ){ return; }
            if( _lockedObjects.Add( collided.gameObject ) ){
                // Debug.Log( "Add " + collided.tag + " object" );
            }
        }
    }

    void OnTriggerExit2D( Collider2D collided ){
        if( !active ){ return; }
        if( _lockedObjects.Contains( collided.gameObject ) ){
            if( collided != collided.GetComponent< CapsuleCollider2D >() ){ return; }
            if( _lockedObjects.Remove( collided.gameObject ) ){
                // Debug.Log( "Remove " + collided.tag + " object" );
            }
        }
    }
}
