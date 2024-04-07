using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;
using FMOD;

/**
* @brief Handles audio playback and effects for a virtual music instrument in ThereMelo.
* 
* This class is used to manipulate audio wheher it be to play or stop playing audio. This class is used by others inorder
* to trigger audio feedback. See the diagram for full details on these interations
*/
public class AudioHandler : MonoBehaviour
{
    /**
     * @brief Reference to the FMOD event for the trigger sound.
     */
    [SerializeField] private EventReference triggerSound;
      /**
     * @brief GameObject that acts as the source of the sound.
     */
    [SerializeField] private GameObject source;

    /**
     * @brief Instance of an FMOD event.
     */
    private EventInstance eventInstance;

    /**
    * @brief Initializes the audio source to the main camera if not set.
    *
    */
    void Awake()
    {
        if (source == null)
        {
            Camera mainCamera = Camera.main;
            source = mainCamera.gameObject;
        }
    }

    /**
     * @brief Plays the trigger sound once at the source's current position.
     */
    public void PlayAudioOnce() {
        AudioManager.instance.PlayOneShot(triggerSound, source.transform.position);
    }

    /**
     * @brief Starts playing the trigger sound continuously.
     * 
     * Stops the current audio if it is already playing before starting the new audio playback.
     */
    public void PlayAudioConstant() {
        // check if event is already playing.
        if (eventInstance.isValid()) { StopAudio(); }

        // create
        eventInstance = AudioManager.instance.PlayConstaint(triggerSound, source.transform.position);
    }

    /**
     * @brief Plays the trigger sound continuously for a specified amount of time.
     * 
     * @param setTime The duration in seconds to play the sound for.
     */
    public void PlayForAmount(float setTime)
    {
        if (eventInstance.isValid()) { StopAudio(); }

        // create and play.
        eventInstance = AudioManager.instance.PlayConstaint(triggerSound, source.transform.position);
        StartCoroutine(StopSoundAfterDelay(setTime));
    }

    /**
     * @brief Stops the currently playing audio with a fade out.
     */
    public void StopAudio() {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
    }

    /**
     * @brief Coroutine to stop sound after a specified delay.
     * 
     * @param delay The delay in seconds before stopping the sound.
     */
    private IEnumerator StopSoundAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Stop the event
        StopAudio();
    }
}
