using UnityEngine;

public class HealSpawn : ObjectSpawn
{
    public int heal = 1;
    
    void Start(){
        CollectibleItem target = GetComponentInChildren< CollectibleItem >( true );

        switch( state ){
            case State.Alive:
                if( target == null ){
                    HealItem healItem = Instantiate( spawnTarget, gameObject.transform ).GetComponent< HealItem >();
                    healItem.heal = heal;
                }
                break;
            case State.Spawning:
                if( target != null ){
                    Destroy( target );
                }
                break;
        }
    }

    void Update()
    {
        CollectibleItem target = GetComponentInChildren< CollectibleItem >( true );
        switch( state ){
            case State.Alive:
                if( target == null ){
                    state = State.Spawning;
                    return;
                }
                break;
            case State.Spawning:
                if( target != null ){
                    state = State.Alive;
                    return;
                }
                respawn += Time.deltaTime;
                if( respawn >= respawnTime ){
                    respawn = 0.0f;
                    HealItem healItem = Instantiate( spawnTarget, gameObject.transform ).GetComponent< HealItem >();
                    healItem.heal = heal;
                }
                break;
        }
    }
}
