using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    private Vector3 size = new Vector3(0.7f, 0.4f, 0.0001f);
    private Coroutine currentThread;
    private float dur = 0.075f;

    public HandManager handManager;

    private float easeInSine(float x) 
    {
        return 1 - Mathf.Cos((x * Mathf.PI) / 2);
    }

    public void ToggleMenu()
    {
        // halt any running threads
        if (currentThread != null) { StopCoroutine(currentThread); currentThread = null; } 

        // if disabled, enable object
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            StartCoroutine(Grow());
        }
        else 
        {
            StartCoroutine(Shrink());
        }
    }

    IEnumerator Grow()
    {
        float time = 0;
        Vector3 initialScale = transform.localScale;

        while (time < dur)
        {
            time += Time.deltaTime;
            float factor = easeInSine(time / dur);
            transform.localScale = Vector3.Lerp(initialScale, size, factor);
            yield return null;
        }

        transform.localScale = size;
    }

    IEnumerator Shrink()
    {
        float time = 0;
        Vector3 initialScale = transform.localScale;

        while (time < dur)
        {
            time += Time.deltaTime;
            float factor = easeInSine(time / dur);
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, factor);
            yield return null;
        }

        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }
}
