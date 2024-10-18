using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum State{ Traversal, Lock }
    [ReadOnlyInspector]
    public State state = State.Traversal;
    SearchObject searched;
    SearchByRaycast newSearched;

    public Vector2 destination = new Vector2( 0, 0 );
    public Transform[] movePoints;
    public float speed = 5.0f;
    Vector2 origin;
    public int nowPoint{ get{ return currentPoint; } }
    int currentPoint;
    public float nextThreshold = 0.05f;
    Vector2 direction = Vector2.down;

    Rigidbody2D rigidbody2d;
    Animator animator;
    public ParticleSystem smokeEffectPrefab;
    public AudioClip[] audios = new AudioClip[3];
    AudioSource audioSource, walkingAudio;

    public int maxHp = 3;
    int currentHp;
    public int hurt = 1;
    public bool broken = true;


    // Start is called before the first frame update
    void Start(){
        currentHp = maxHp; 

        rigidbody2d = GetComponent< Rigidbody2D >();
        origin = new Vector2( transform.position.x, transform.position.y );
        currentPoint = -1;
        destination = origin;

        animator = GetComponent< Animator >();
        audioSource = GetComponent< AudioSource >();
        searched = GetComponent< SearchObject >();
        // newSearched = GetComponent< SearchByRaycast >();
        // Debug.Log( newSearched );
        ParticleSystem smokeEffect = Instantiate< ParticleSystem >( smokeEffectPrefab, transform );
        smokeEffect.transform.localPosition = new Vector2( 0.15f, 1.6f );
        smokeEffect.Play();

        walkingAudio = transform.GetChild(0).GetComponent< AudioSource >();
    }

    // Update is called once per frame
    void Update(){
        if( !broken ){
            return;
        }

        if( searched != null ){
            if( searched.lockedObjects.Count == 0 ){
                state = State.Traversal;
            }else{
                state = State.Lock;
            }
        }
        // if( newSearched != null ){ Debug.Log( newSearched.searchedObject ); }
        if( !Mathf.Approximately( rigidbody2d.velocity.x, 0.0f ) || !Mathf.Approximately( rigidbody2d.velocity.y, 0.0f ) ){
            direction = rigidbody2d.velocity.normalized;
        }
        animator.SetFloat( "Move X", direction.x );
        animator.SetFloat( "Move Y", direction.y );
        animator.SetFloat( "Speed", rigidbody2d.velocity.magnitude );

        if( walkingAudio != null ){
            if( walkingAudio.isPlaying ){
                if( rigidbody2d.velocity.magnitude < speed*Time.deltaTime ){
                    // walkingAudio.loop = false;
                    walkingAudio.Stop();
                }
            }else{
                if( rigidbody2d.velocity.magnitude >= speed*Time.deltaTime ){
                    // walkingAudio.loop = true;
                    walkingAudio.Play();
                }
            }
        }
        
    }
    void FixedUpdate(){
        if( !broken ){ return; }
        EnemyMovement();
    }

    void NextPoint(){
        currentPoint++;
        if( currentPoint == movePoints.Length ){
            currentPoint = -1;
            destination = origin;
        }else{
            destination = movePoints[currentPoint].position;
        }
    }

    void EnemyMovement(){
        float step = speed*Time.deltaTime;
        switch( state ){
            case State.Traversal:
                Vector2 position = rigidbody2d.position;
                Vector2 move = Vector2.zero;
                move = destination-rigidbody2d.position;
                if( move.magnitude < step ){ direction = Vector2.zero; }
                else{ direction = move.normalized; }
                // Threshold for going to next point
                if( Vector2.Distance( position, destination ) < nextThreshold ){
                    if( movePoints.Length > 0 ){
                        NextPoint();
                    }
                }
                break;
            case State.Lock:
                Vector2 nearest = Vector2.zero;
                Vector2 lockedVec = Vector2.zero;
                // RaycastHit2D hit;
                foreach( var locked in searched.lockedObjects ){
                    lockedVec = locked.transform.position-transform.position;
                    // hit = Physics2D.Raycast( transform.position, lockedVec.normalized, searched.searchRange, LayerMask.GetMask( "Character", "Obstacle" ) );
                    // if( hit.rigidbody != null ){
                    //     if( hit.rigidbody.gameObject.layer != LayerMask.GetMask( "Character" ) ){ continue; }
                    // }
                    if( nearest.Equals( Vector2.zero ) || lockedVec.magnitude <= nearest.magnitude ){
                        nearest = lockedVec;
                    }
                }
                if( nearest.magnitude < step ){ direction = Vector2.zero; }
                else{ direction = nearest.normalized; }
                break;
        }
        rigidbody2d.velocity = speed*direction;
    }

    void OnCollisionEnter2D( Collision2D collision ){
        if( collision.collider.name.Equals( "Ruby" ) ){
            RubyController ruby = collision.collider.GetComponent< RubyController >();
            if( ruby != null ){
                ruby.ChangeHp( -hurt );
            }
        }
    }

    void ChangeHp( int amount ){
        currentHp = Mathf.Clamp( currentHp+amount, 0, maxHp );
        Debug.Log( gameObject.name + " Hp: " + currentHp + "/" + maxHp );
        if( amount < 0 ){ audioSource.PlayOneShot( audios[UnityEngine.Random.Range( 0, 1+1 )] ); }
    }

    public void Fix( int amount ){
        ChangeHp( amount );
        if( currentHp != 0 ){ return; }

        if( audioSource.isPlaying ){
            audioSource.loop = false;
            audioSource.Pause();
        }

        ParticleSystem[] effects = GetComponentsInChildren< ParticleSystem >();
        ParticleSystem smokeEffect = null;
        foreach( var effect in effects ){
            if( effect.GetType() == smokeEffectPrefab.GetType() ){
                smokeEffect = effect;
                break;
            }
        }
        if( smokeEffect != null ){ smokeEffect.Stop(); }
        broken = false;
        rigidbody2d.simulated = false;
        animator.SetTrigger( "Fixed" );
        audioSource.PlayOneShot( audios[2] );
        walkingAudio.Stop();
    }

    public void PlantVirus(){
        ParticleSystem smokeEffect = GetComponentInChildren< ParticleSystem >();
        if( smokeEffect == null || smokeEffect.GetType() != smokeEffectPrefab.GetType() ){
            smokeEffect = Instantiate< ParticleSystem >( smokeEffectPrefab, transform );
            smokeEffect.transform.localPosition = new Vector2( 0.15f, 1.6f );
        }
        smokeEffect.Play();
        broken = true;
        ChangeHp( maxHp );
        rigidbody2d.simulated = true;
        animator.SetTrigger( "Planted" );
    }
}
