using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] private EventReference triggerSound;
    [SerializeField] private GameObject source;

    private FMOD.Studio.EventInstance eventInstance;

    void Awake()
    {
        if (source == null)
        {
            Camera mainCamera = Camera.main;
            source = mainCamera.gameObject;
        }
    }

    public void PlayAudioOnce() {
        AudioManager.instance.PlayOneShot(triggerSound, source.transform.position);
    }

    public void PlayAudioConstant() {
        eventInstance = AudioManager.instance.PlayConstaint(triggerSound, source.transform.position);
    }

    public void PlayForAmount(float setTime)
    {
        eventInstance = AudioManager.instance.PlayConstaint(triggerSound, source.transform.position);
        StartCoroutine(StopSoundAfterDelay(setTime));
    }

    public void StopAudio() {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
    }

    private IEnumerator StopSoundAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Stop the event
        StopAudio();
    }

}
