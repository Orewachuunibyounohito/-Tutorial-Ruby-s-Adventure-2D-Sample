using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public static UIHealthBar instance{ get; private set; }
    public UnityEngine.UI.Image mask;
    float originSize;

    void Awake(){
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        originSize = mask.rectTransform.rect.width;
    }

    public void ChangeBar( float percent ){
        mask.rectTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, originSize*percent );
    }
}
