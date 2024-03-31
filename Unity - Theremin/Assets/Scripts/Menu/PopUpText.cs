using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpText : MonoBehaviour
{
    //Object we want to change
    public GameObject textmeshpro_advice;
    //String we accept
    public string advice;

    TextMeshProUGUI textmeshpro_advice_text;


    // Start is called before the first frame update
    void Start()
    {
        textmeshpro_advice_text = textmeshpro_advice.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        textmeshpro_advice_text.text = advice;
    }


}
