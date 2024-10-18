using UnityEngine;

public class ObjectSpawn : MonoBehaviour
{
    public enum State{ Alive, Spawning }
    [ReadOnlyInspector]
    public State state = State.Alive;
    public GameObject spawnTarget;
    public float respawnTime = 10.0f;
    [ReadOnlyInspector]
    public float respawn = 0.0f;

    [ReadOnlyInspector]
    public string someLog = "";
    // Start is called before the first frame update
    void Start(){
        CollectibleItem target = GetComponentInChildren< CollectibleItem >( true );

        switch( state ){
            case State.Alive:
                if( target == null ){
                    GameObject obj = Instantiate( spawnTarget, gameObject.transform ).GetComponent< GameObject >();
                }
                break;
            case State.Spawning:
                if( target != null ){
                    Destroy( target );
                }
                break;
        }
    }
    // Update is called once per frame
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
                    GameObject obj = Instantiate( spawnTarget, gameObject.transform ).GetComponent< GameObject >();
                }
                break;
        }
    }
}
