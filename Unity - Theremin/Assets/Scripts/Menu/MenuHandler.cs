using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using static Unity.VisualScripting.Member;

public class MenuHandler : MonoBehaviour
{
    private Vector3 size = new Vector3(0.7f, 0.4f, 0.0001f);
    private Coroutine currentThread;
    private float dur = 0.075f;
    private Camera mainCamera;

    public HandManager handManager;

    [SerializeField] private EventReference[] Sounds;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private float easeInSine(float x) 
    {
        return 1 - Mathf.Cos((x * Mathf.PI) / 2);
    }

    public void ToggleMenu()
    {
        // halt any running threads
        gameObject.SetActive(true);

        if (currentThread != null) { StopCoroutine(currentThread); currentThread = null; }

        // if disabled, enable object
        if (!handManager.menuEnabled)
        {
            AudioManager.instance.PlayOneShot(Sounds[0], mainCamera.gameObject.transform.position);
            StartCoroutine(Grow());
        }
        else 
        {
            AudioManager.instance.PlayOneShot(Sounds[1], mainCamera.gameObject.transform.position);
            StartCoroutine(Shrink());
        }
        handManager.menuEnabled = (!handManager.menuEnabled);
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
