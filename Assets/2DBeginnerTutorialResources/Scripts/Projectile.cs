using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    public enum State{ Lock, Unlock }
    [ReadOnlyInspector]
    public State state = State.Unlock;

    Rigidbody2D rigidbody2d;
    
    [ReadOnlyInspector]
    public GameObject _target;
    public float aliveTime = 4.0f;
    float _aliveTime = 0.0f;

    public float speed{
        get{ return _speed; }
        set{ _speed = value; }
    }
    float _speed = 8.0f;
    public float distRange = 5.0f;
    Vector2 _originPos;

    public AudioClip[] clips;

    int damageMin = 1;
    int damageMax = 3;

    void Awake(){
        rigidbody2d = GetComponent< Rigidbody2D >();
        _originPos = rigidbody2d.transform.position;
    }

    // Update is called once per frame
    void Update()
    {   
        if( _target != null ) state = State.Lock;
        // Debug.Log( rigidbody2d.velocity );
        _aliveTime += Time.deltaTime;
        if( _aliveTime >= aliveTime ){
            Destroy( gameObject );
        }else if( Vector2.Distance( transform.position, _originPos ) > distRange ){
            Destroy( gameObject );
        }
    }

    void FixedUpdate(){
        switch( state ){
            case State.Lock:
                Vector2 direction = ( _target.transform.position-transform.position ).normalized;
                rigidbody2d.velocity = direction*rigidbody2d.velocity.magnitude;
                break;
        }
    }

    void OnCollisionEnter2D( Collision2D collision ){
        EnemyController enemy = collision.gameObject.GetComponent< EnemyController >();
        if( enemy != null ){
            enemy.Fix( -Random.Range( damageMin, damageMax+1 ) );
        }
        Destroy( gameObject );
    }

    public void Launch( GameObject owner, Vector2 direct, float force ){
        // AddForce is used in FixedUpdate that fps = 50, so deltaTime = 1/50
        // if force = 300 & mass = 1, v = force/mass*deltaTime = 6
        RubyController ruby;
        if( ( ruby = owner.GetComponent< RubyController >() ) != null ){
            ruby.PlaySound( clips[0] );
        }
        rigidbody2d.AddForce( direct*force );
    }    
}
