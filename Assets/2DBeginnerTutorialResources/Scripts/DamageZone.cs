using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public int hurt = 1;
    public void OnTriggerStay2D( Collider2D collided ){
        if( collided.name.Equals( "Ruby" ) ){
            RubyController ruby = collided.GetComponent< RubyController >();
            if( collided == collided.GetComponent< CapsuleCollider2D >() ){
                if( ruby != null ){
                    ruby.ChangeHp( -hurt );
                }
            }
        }
    }
}
