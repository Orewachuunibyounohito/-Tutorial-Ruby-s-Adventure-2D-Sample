using System.Collections;
using UnityEngine;

public class HealItem : CollectibleItem, IItemEffect, ILoot
{   
    public RubyItem healItem;
    public int heal = 1;

    new public void OnTriggerEnter2D( Collider2D collided ){
        if( collided.name.Equals( "Ruby" ) ){
            RubyController ruby = collided.GetComponent< RubyController >();
            if( collided == collided.GetComponent< CapsuleCollider2D >() ){
                StartCoroutine( Effect( ruby.gameObject ) );
                Destroy( gameObject );
                ruby.PlaySound( clips[0] );
            }else if( collided == collided.GetComponent< CircleCollider2D >() ){
                if( !canLoot ){ return; }
                if( collided.tag.Equals( "Player" ) ){
                    if( collided == collided.GetComponent< CircleCollider2D >() ){
                        StartCoroutine( MoveToLooter( collided.transform, healItem ) );
                    }
                }
            }
        }
    }

    public IEnumerator Effect( GameObject target ){
        RubyController ruby = target.GetComponent< RubyController >();
        if( ruby != null ){
            if( ruby.health < ruby.maxHp ){
                ruby.ChangeHp( heal );
            }else{
                RubyInventoryManager.instance.AddItem( healItem );
            }
        }
        yield return 0;
    }

    public IEnumerator MoveToLooter( Transform looter, RubyItem item ){
        Collider2D col = GetComponent< Collider2D >();
        col.enabled = false;

        float speedMultiplier = 1.0f;
        Rigidbody2D itemRb = GetComponent<Rigidbody2D>();
        while( Vector2.Distance( transform.position, looter.position ) > 0.3f ){
            itemRb.MovePosition( Vector2.MoveTowards( transform.position, looter.position, ILoot.LOOTSPEED*speedMultiplier*Time.fixedDeltaTime ) );
            yield return new WaitForFixedUpdate();
            speedMultiplier *= 1.01f;
        }
        RubyInventoryManager.instance.AddItem( item );
        Destroy( gameObject );
    }
}
