using FMOD;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

/**
 * @brief Handles instrument sound changes and UI interactions for selecting different instruments.
 */
public class InstrumentsHandler : MonoBehaviour
{
    /**
     * @brief Tracks the state of the menu (open or closed).
     */
    private bool menuBool = false;
    private Coroutine mover;

    /**
     * @brief References to the volume slider, instrument menu, and icon within the UI.
     */
    private GameObject volumeSlider, instrumentMenu, icon;
    private Camera mainCamera;

    /**
     * @brief Reference to the AudioManager for playing sounds.
     */
    [SerializeField] private AudioManager audioManager;

    /**
     * @brief The UI GameObject that contains the volume slider and instrument menu.
     */
    [SerializeField] private GameObject UI;

    /**
     * @brief The icon to display when an instrument is selected.
     */
    [SerializeField] private Sprite selectedIcon;

    /**
     * @brief An array of EventReferences representing the different sounds for each instrument.
     */
    [SerializeField] private EventReference[] Sounds;

    /**
     * @brief StudioEventEmitter used to emit sound events.
     */
    [SerializeField] private StudioEventEmitter emitter;

    /**
     * @brief Initializes the handler, finding necessary components in the UI.
     */
    private void Start()
    {
        volumeSlider = UI.transform.Find("Slider Panel").gameObject;
        instrumentMenu = UI.transform.Find("Instrument Panel").gameObject;
        icon = gameObject.transform.Find("Instrument/RoundButtonCircle/Icon").gameObject;

        mainCamera = Camera.main;
    }

    /**
     * @brief Changes the sound of the instrument to the one specified by soundName.
     * 
     * @param soundName The name of the sound to change to.
     */
    public void changeSound(string soundName)
    {
        string path = "event:/Instruments/" + soundName;

        // change sound
        emitter.ChangeEvent(path);
        
        // change icon
        selectedIcon = Resources.Load<Sprite>("Sprites/" + soundName); 
    }

    // mover

    /**
     * @brief Coroutine that moves the volume slider to the target position over the specified duration.
     * 
     * @param target The target position for the volume slider.
     * @param dur The duration over which to move the slider.
     * @return IEnumerator for coroutine management.
     */
    IEnumerator MoveTo(Vector3 target, float dur)
    {
        Vector3 startPosition = volumeSlider.transform.localPosition;
        float timeElapsed = 0;

        while (timeElapsed < dur)
        {
            volumeSlider.transform.localPosition = Vector3.Lerp(startPosition, target, timeElapsed / dur);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        volumeSlider.transform.localPosition = target;
    }

    /**
     * @brief Toggles the instrument menu open or closed, optionally forcing it to close.
     * 
     * @param forceClose If true, forces the menu to close; otherwise, toggles the menu based on its current state.
     */
    public void toggleMenu(bool forceClose)
    {
        // set menuBool opposite of itself
        menuBool = forceClose ? false : !menuBool;

        // check if menu is opening
        float value = menuBool ? 0.125f : 0.025f;

        icon.GetComponent<SpriteRenderer>().sprite = menuBool ? Resources.Load<Sprite>("Sprites/exit") : selectedIcon;

        // start movement if forceClose isn't true.
        Vector3 tar = new Vector3(instrumentMenu.transform.localPosition.x > 0 ? value : -value, 0f, 0f);
        if (!forceClose)
        {
            // sound
            AudioManager.instance.PlayOneShot(menuBool ? Sounds[0] : Sounds[1], mainCamera.gameObject.transform.position);

            if (mover != null)
            {
                StopCoroutine(mover);
                mover = null;
            }
            mover = StartCoroutine(MoveTo(tar, 0.05f));
        }
        else 
        { 
            volumeSlider.transform.localPosition = tar;
            icon.GetComponent<SpriteRenderer>().sprite = selectedIcon;
        }

        // toggle menu
        instrumentMenu.SetActive(menuBool);
    }
}