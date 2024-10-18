using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioVolume : MonoBehaviour
{   
    public AudioSource audioSource;
    public Slider slider;

    void Start(){
        audioSource = GetComponent< AudioSource >();
        slider = FindObjectOfType< Slider >();
        slider.onValueChanged.AddListener( ChangeVolume );
    }
    public void ChangeVolume( float percent ){
        audioSource.volume = percent;
    }
}
