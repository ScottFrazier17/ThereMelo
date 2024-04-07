using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Displays popup text on the screen by updating a TextMeshProUGUI component with provided advice text.
 */
public class PopUpText : MonoBehaviour
{
    //Object we want to change
    /**
     * @brief The GameObject containing the TextMeshProUGUI component to be updated.
     */
    public GameObject textmeshpro_advice;
    //String we accept
    /**
     * @brief The string of advice to be displayed in the TextMeshProUGUI component.
     */
    public string advice;

    /**
     * @brief The TextMeshProUGUI component where advice text will be displayed.
     */
    TextMeshProUGUI textmeshpro_advice_text;


    // Start is called before the first frame update
    /**
     * @brief Initializes the PopUpText by retrieving the TextMeshProUGUI component from the specified GameObject.
     */
    void Start()
    {
        textmeshpro_advice_text = textmeshpro_advice.GetComponent<TextMeshProUGUI>();
    }

    /**
     * @brief Updates the text displayed by the TextMeshProUGUI component each frame with the current advice string.
     */
    void Update()
    {
        textmeshpro_advice_text.text = advice;
    }
}
