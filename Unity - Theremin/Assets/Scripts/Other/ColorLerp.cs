using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorLerp : MonoBehaviour
{
    private (int, int, int)[] noteColors = {
        (58, 92, 255),
        (103, 58, 255),
        (205, 58, 255),
        (255, 76, 58),
        (255, 133, 58),
        (255, 153, 58),
        (255, 215, 58),
        (162, 255, 58),
        (100, 255, 58),
        (67, 255, 58),
        (58, 255, 180),
        (58, 167, 255),
    };

    private Dictionary<GameObject, Coroutine> lerpCoroutines = new Dictionary<GameObject, Coroutine>();

    private Color convertColor((int, int, int) colorTuple)
    {
        return new Color(colorTuple.Item1 / 255.0f, colorTuple.Item2 / 255.0f, colorTuple.Item3 / 255.0f);
    }

    public void startLerp(int noteIndex) {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Colors");
        foreach (GameObject obj in objects)
        {
            // check for exisiting coroutine.
            if (lerpCoroutines.ContainsKey(obj) && lerpCoroutines != null)
            {
                StopCoroutine(lerpCoroutines[obj]);
                lerpCoroutines[obj] = null;
            }

            lerpCoroutines[obj] = StartCoroutine(lerpColor(obj, convertColor(noteColors[noteIndex]), 0.25f));

        }
    }

    IEnumerator lerpColor(GameObject obj, Color targetColor, float dur)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && renderer.material.HasProperty("_EmissionColor"))
        {
            Color startColor = renderer.material.GetColor("_EmissionColor");
            float time = 0;

            while (time < dur)
            {
                time += Time.deltaTime;
                Color currentColor = Color.Lerp(startColor, targetColor, time / dur);
                renderer.material.SetColor("_EmissionColor", currentColor);
                yield return null;
            }

            // Ensure the final color is set
            renderer.material.SetColor("_EmissionColor", targetColor);
        }
    }

}
