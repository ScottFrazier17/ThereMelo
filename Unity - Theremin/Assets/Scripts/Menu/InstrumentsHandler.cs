using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InstrumentsHandler : MonoBehaviour
{
    private bool menuBool = false;
    private Coroutine mover;
    private GameObject volumeSlider, instrumentMenu, icon;
    private Camera mainCamera;

    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameObject UI;
    [SerializeField] private Sprite selectedIcon;
    [SerializeField] private EventReference[] Sounds;
    [SerializeField] private StudioEventEmitter emitter;

    private void Start()
    {
        volumeSlider = UI.transform.Find("Slider Panel").gameObject;
        instrumentMenu = UI.transform.Find("Instrument Panel").gameObject;
        icon = gameObject.transform.Find("Instrument/RoundButtonCircle/Icon").gameObject;

        mainCamera = Camera.main;
    }

    public void changeSound(string soundName)
    {
        string path = "event:/Instruments/" + soundName;

        // change sound
        emitter.ChangeEvent(path);
        
        // change icon
        selectedIcon = Resources.Load<Sprite>("Sprites/" + soundName); 
    }

    // mover
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

    public void toggleMenu(bool forceClose)
    {
        // set menuBool opposite of itself
        menuBool = forceClose ? false : !menuBool;

        // check if menu is opening
        float value = menuBool ? 0.125f : 0.025f;

        icon.GetComponent<SpriteRenderer>().sprite = menuBool ? Resources.Load<Sprite>("Sprites/exit") : selectedIcon;

        // start movement
        Vector3 tar = new Vector3(instrumentMenu.transform.localPosition.x > 0 ? value : -value, 0f, 0f);
        if (mover != null)
        {
            StopCoroutine(mover);
            mover = null;
        }
        mover = StartCoroutine(MoveTo(tar, 0.05f));

        // toggle menu
        instrumentMenu.SetActive(menuBool);

        // sound
        AudioManager.instance.PlayOneShot(menuBool ? Sounds[0] : Sounds[1], mainCamera.gameObject.transform.position);
    }

}
