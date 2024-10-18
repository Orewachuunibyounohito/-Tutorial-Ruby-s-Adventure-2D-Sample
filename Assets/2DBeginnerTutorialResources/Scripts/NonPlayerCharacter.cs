using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class NonPlayerCharacter : MonoBehaviour
{
    public float duration = 4.0f;
    float time = 0.0f;
    [SerializeField, ReadOnlyInspector]
    GameObject dialog;

    // Start is called before the first frame update
    void Start()
    {
        dialog = GetComponentInChildren< Canvas >( true ).gameObject;
    }

    void Update(){
        if( time >= 0.0f ){
            time -= Time.deltaTime;
        }else{
            dialog.SetActive( false );
        }
    }

    public void DisplayDialog(){
        if( dialog.activeSelf ){
            time = -1.0f;
            dialog.SetActive( false );
        }else{
            time = duration;
            dialog.SetActive( true );
        }
    }
}
