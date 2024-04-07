using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using static Unity.VisualScripting.Member;

/**
 * @brief Manages the appearance and disappearance of a menu through animation and audio feedback.
 */
public class MenuHandler : MonoBehaviour
{
    private Vector3 size = new Vector3(0.7f, 0.4f, 0.0001f);
// private Vector3 smallSize = new Vector3(0.01f, 0.01f, 0.01f);

    /**
     * @brief Coroutine reference for the currently running animation (growing or shrinking the menu).
     */
    private Coroutine currentThread;
    /**
     * @brief Duration of the animation for opening or closing the menu.
     */
    private float dur = 0.075f;
    private Camera mainCamera;

    /**
     * @brief Reference to the HandManager to check and set the menu's enabled state.
     */
    [SerializeField] private HandManager handManager;
    /**
     * @brief An array of EventReferences for playing sound effects associated with menu actions.
     */
    [SerializeField] private EventReference[] Sounds;

    /**
     * @brief Initializes the handler, finding the main camera.
     */
    private void Start()
    {
        mainCamera = Camera.main;
    }

    /**
     * @brief Easing function for the animation, creating a smooth start effect.
     * 
     * @param x The animation's progress as a fraction (0 to 1).
     * @return The eased progress value.
     */
    private float easeInSine(float x)
    {
        return 1 - Mathf.Cos((x * Mathf.PI) / 2);
    }

    /**
     * @brief Toggles the menu's visibility, optionally forcing it to close, with animations and sound effects.
     * 
     * @param forceClose If true, forces the menu to close; otherwise, toggles the menu based on its current state.
     */
    public void toggleMenu(bool forceClose)
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

    /**
     * @brief Coroutine to animate the menu growing to its target size.
     * 
     * @return IEnumerator for coroutine management.
     */
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

    /**
     * @brief Coroutine to animate the menu shrinking to disappear.
     * 
     * @return IEnumerator for coroutine management.
     */
    IEnumerator Shrink()
    {
        float time = 0;
        Vector3 initialScale = transform.localScale;

        while (time < dur)
        {
            time += Time.deltaTime;
            float factor = easeInSine(time / dur);
            transform.localScale = Vector3.Lerp(initialScale, new Vector3(0, 0, 0.000001f), factor);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}