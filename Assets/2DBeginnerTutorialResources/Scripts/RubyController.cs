using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{   
    public enum State{ None, Launch }
    public State state = State.None;

    public static RubyController instance{ get; private set; }

    public int fps = 60;
    public GameObject projectilePrefab;
    public ParticleSystem stunEffect;
    public ParticleSystem healEffect;
    public ParticleSystem hitEffect;

    public UIHealthBar uiHealthBar;

    // public enum AudioIndex{ Hurt, Throw }
    // 0: Hurt
    public AudioClip[] audios = new AudioClip[ 1 ];
    AudioSource audioSource;

    Rigidbody2D myRigidbody;
    float horizontal;
    float vertical;

    public float stepSize = 5.0f;
    float multiply = 1.0f;
    [ReadOnlyInspector]
    public bool running = false;

    public int maxHp = 5;
    int currentHp;
    // Set Property, variable { get { return myPrivateVariable; } }
    public int health{ get{ return currentHp; } }

    public float invincibleTime = 2.0f;
    float invincibleTimer;
    bool isInvincible;
    public bool invincible{ get{ return isInvincible; } }

    [ReadOnlyInspector]
    public bool invincibleDuring = false;

    Animator animator;
    public Vector2 look{ get{ return lookDirection; } }
    Vector2 lookDirection = new Vector2( 0, -1 );
    // [ReadOnlyInspector]
    // public GameObject searchArea;
    public float projectileSpeed = 8.0f;

    public GameObject nearlyEnemy{
        get{ return _enemy; }
        set{ _enemy = value; }
    }
    GameObject _enemy, _npc;

    float launchDuration = 1.0f;
    float launchTime = 0.0f;

    void Awake(){
        instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // set n Frame per second
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = fps;
        
        myRigidbody = GetComponent< Rigidbody2D >();
        animator = GetComponent< Animator >();

        currentHp = maxHp;
        invincibleTimer = invincibleTime;

        // searchArea = GetComponentInChildren< SearchEnemy >().gameObject;
        ParticleSystem[] effects = GetComponentsInChildren< ParticleSystem >();
        foreach( var effect in effects ){
            switch( effect.gameObject.name ){
                case "HealEffect":
                    healEffect = effect;
                    break;
                case "StunEffect":
                    stunEffect = effect;
                    break;
                case "HitEffect":
                    hitEffect = effect;
                    break;
            }
        }
        // Debug.Log( "effects.Length: " + effects.Length + "\nHealEfect: " + healEffect + "\nStunEffect: " + stunEffect );
        uiHealthBar = GetComponent< UIHealthBar >();
        audioSource = GetComponent< AudioSource >();
    }

    // Update is called once per frame
    void Update()
    {
        LockCheck();
        CharacterControl();
    }

    void FixedUpdate(){
        // Fix Character's Jittering
        CharacterMove();
    }

    void LockCheck(){
        HashSet<GameObject> lockedObjects = GetComponent< SearchObject >().lockedObjects;
        if( lockedObjects.Count != 0 ){
            foreach( var locked in lockedObjects ){
                switch( locked.tag ){
                    case "Enemy":
                        _LockCheck( ref _enemy, locked );
                        break;
                    case "NPC":
                        _LockCheck( ref _npc, locked );
                        break;
                }
            }
        }
        else{
            _enemy = null;
            _npc = null;
        }
        // Debug.Log( _enemy );
    }
    void _LockCheck( ref GameObject target, GameObject locked ){
        if( target == null ){ target = locked; }
        else{
            target = ( Vector2.Distance( transform.position, locked.transform.position ) < Vector2.Distance( transform.position, target.transform.position ) )? locked : target;
        }
    }

    // Time.deltaTime: delta time per frame
    // if 60 fps -> Time.deltaTime = 0.017 s
    // if 10 fps -> Time.deltaTime = 0.1s
    void CharacterControl(){
        horizontal = Input.GetAxis( "Horizontal" );
        vertical = Input.GetAxis( "Vertical" );

        Vector2 move = new Vector2( horizontal, vertical );
        switch( state ){
            case State.Launch:
                // Debug.Log( animator.GetAnimatorTransitionInfo(  ).duration  );
                // Debug.Log( "0: " + animator.GetLayerName( 0 ) + "\n-1: " + animator.GetLayerName( -1 ) );
                // Debug.Log( animator.GetCurrentAnimatorStateInfo( 0 ).shortNameHash == launchHashName );
                if( launchTime >= launchDuration ){
                    launchTime = 0.0f;
                    state = State.None;
                }else{
                    launchTime += Time.deltaTime;
                }
                break;
            default:
                if( !Mathf.Approximately( move.x, 0.0f ) || !Mathf.Approximately( move.y, 0.0f ) ){
                    lookDirection = move.normalized;
                }
                animator.SetFloat( "Look X", lookDirection.x );
                animator.SetFloat( "Look Y", lookDirection.y );
                break;
        }
        animator.SetFloat( "Speed", move.magnitude );

        running = false;
        multiply = 1.0f;
        if( Input.GetKey( KeyCode.LeftShift ) ){
            running = true;
            multiply = 1.7f;
        }

        // if( Input.GetKeyDown( KeyCode.Z ) ){
        if( Input.GetButtonDown( "Fire1" ) ){
            if( state == State.Launch ){ return; }
            Launch();
        }

        if( Input.GetButtonDown( "Plant" ) ){
            PlantVirus();
        }

        if( Input.GetButtonDown( "Interact" ) ){
            float dist = 1.0f;
            if( Mathf.Approximately( lookDirection.y, 1.0f ) ){ dist = 0.3f; }
            RaycastHit2D hit = Physics2D.Raycast( (Vector2)transform.position+Vector2.up*0.5f, lookDirection, dist, LayerMask.GetMask( "NPC" ) );
            if( hit.collider != null ){
                NonPlayerCharacter npc = hit.collider.name.Equals("Character") ? hit.transform.parent.GetComponent< NonPlayerCharacter >()
                                                                               : hit.collider.GetComponent< NonPlayerCharacter >();
                if( npc != null ){ npc.DisplayDialog(); }
            }
        }

        if( Input.GetKeyDown( KeyCode.E ) ){
            RubyItem receivedItem = RubyInventoryManager.instance.GetSelectedItem( false );
            if( receivedItem != null ){
                if( receivedItem.itemType == RubyItem.ItemType.Consumables ){
                    RubyInventoryManager.instance.GetSelectedItem( true );
                }
            }
        }

        if( Input.GetKeyDown( KeyCode.G ) ){
            RubyItem receivedItem = RubyInventoryManager.instance.ThrowSelectedItem( transform, lookDirection );
            if( receivedItem != null ){
                Debug.Log( receivedItem + " was threw." );
            }else{
                Debug.Log( "No item received." );
            }
        }

        if( isInvincible ){
            invincibleTimer -= Time.deltaTime;
            if( invincibleTimer < 0 ){
                invincibleDuring = false;
                isInvincible = false;
                invincibleTimer = invincibleTime;
            }
        }
    }
    void CharacterMove(){
        Vector2 position = myRigidbody.position;
        Vector2 move = new Vector2( horizontal, vertical );
        position += stepSize*multiply*move*Time.deltaTime;
        myRigidbody.MovePosition( position );
    }
    
    public void ChangeHp( int amount ){
        if( amount < 0 ){
            if( isInvincible ){ return; }
            invincibleDuring = true;
            isInvincible = true;
            invincibleTimer = invincibleTime;
            animator.SetTrigger( "Hit" );
            ParticleSystem effect = Instantiate( hitEffect, transform );
            effect.transform.localPosition = new Vector2( 0.0f, 0.5f );
            if( _enemy != null ){ effect.transform.LookAt( _enemy.transform ); }
            effect.Play();
            PlaySound( audios[0] );
        }else{
            ParticleSystem effect = Instantiate( healEffect, transform );
            effect.transform.localPosition = new Vector2( 0.0f, 1.0f );
            ParticleSystem.MainModule effectMain = effect.main;
            ParticleSystem.EmissionModule effectEmit = effect.emission;
            float duration = amount/effectEmit.rateOverTime.constant+0.1f/effectEmit.rateOverTime.constant;
            if( duration > 3.0f ){ duration = 3.0f; }
            effectMain.duration = duration;
            effect.Play();
        }
        currentHp = Mathf.Clamp( currentHp+amount, 0, maxHp );
        uiHealthBar.ChangeBar( (float)currentHp/maxHp );
        // Debug.Log( "Hp: " + currentHp + " / " + maxHp );
    }
    
    void Launch(){
        GameObject projectileObject = Instantiate( projectilePrefab, transform.position+(Vector3)( lookDirection*0.4f+Vector2.up*0.3f ), Quaternion.identity );
        Projectile projectile = projectileObject.GetComponent< Projectile >();
        // fixedFps: fps in FixedUpdate.
        float fixedFps = 50;
        projectile._target = _enemy;
        if( _enemy != null ){
            lookDirection = ( _enemy.transform.position-transform.position ).normalized;
        }
        projectile.Launch( gameObject, lookDirection, projectileSpeed*projectilePrefab.GetComponent< Rigidbody2D >().mass*fixedFps );
        animator.SetFloat( "Look X", lookDirection.x );
        animator.SetFloat( "Look Y", lookDirection.y );
        animator.SetTrigger( "Launch" );
        state = State.Launch;
    }

    void PlantVirus(){
        EnemyController[] enemyAll = FindObjectsByType< EnemyController >( FindObjectsSortMode.None );
        foreach( var enemy in enemyAll ){
            if( !enemy.broken ){
                if( Vector2.Distance( enemy.transform.position, transform.position ) <= GetComponent< CircleCollider2D >().radius ){
                    enemy.PlantVirus();
                }
            }
        }
    }

    public void PlaySound( AudioClip clip ){
        audioSource.PlayOneShot( clip );
    }
}
