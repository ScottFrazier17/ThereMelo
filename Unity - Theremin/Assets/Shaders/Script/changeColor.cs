using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeColor : MonoBehaviour
{
    public Color onPressColor;
    public float lerpDuration = 1f;

    private Color originalColor;

    private void Awake()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;
            originalColor = mat.color;
        }
    }
    public void SetColor(bool touched)
    {
        if (gameObject.activeInHierarchy){
            StartCoroutine(LerpColor(touched));
        }
        
    }

    IEnumerator LerpColor(bool touched)
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;
            Color startColor = (touched) ? originalColor : onPressColor;
            Color endColor = (touched) ? onPressColor : originalColor;
            float startTime = Time.time;

            while (Time.time - startTime < lerpDuration)
            {
                float lerpFactor = (Time.time - startTime) / lerpDuration;
                mat.color = Color.Lerp(startColor, endColor, lerpFactor);
                yield return null; // Wait for the next frame
            }

            mat.color = endColor; // Ensure the final color is set
        }
    }
}