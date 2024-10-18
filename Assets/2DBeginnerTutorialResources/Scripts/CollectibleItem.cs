using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{    
    public AudioClip[] clips;
    public bool canLoot = false;
    
    public void OnTriggerEnter2D( Collider2D collided ){
        if( collided.name.Equals( "Ruby" ) ){
            RubyController ruby = collided.GetComponent< RubyController >();
            if( ruby != null ){
                Destroy( gameObject );
            }
        }
    }
}
