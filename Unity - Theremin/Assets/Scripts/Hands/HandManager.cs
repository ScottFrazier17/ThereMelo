using Leap.Unity;
using UnityEngine;
using Leap;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using FMOD;

/**
 * @brief Manages hand interactions for ThereMelo, handling input from Leap Motion to control audio parameters.
 */
public class HandManager : MonoBehaviour
{
    public LeapProvider leapProvider;
    
    /**
     * @brief Flags to indicate the movement state of the pad, rod, and whether the menu is enabled.
     */
    public bool movingPad = false, movingRod = false, menuEnabled = false, isPlaying;

    private bool movingObject => movingPad || movingRod;

    /**
     * @brief Stores the chirality of the user's hand preferred for interaction.
     */
    private string userHand;
    private float vol, threshold = 0.05f;

    [SerializeField] private StudioEventEmitter audioManagerEmitter;

    /**
     * @brief Objects representing the control points for volume and pitch.
     */
    [SerializeField] private GameObject volumeObj, pitchObj;

    // Singleton instance
    public static HandManager instance { get; private set; }

    /**
     * @brief Ensures a single instance of HandManager and initializes it.
     */
    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        }
        else{
            instance = this;
        }
    }

    /**
     * @brief Updates the user's hand preference.
     * 
     * @param key The preference key to be updated.
     * @param value The new value for the preference.
     */
    private void updateHand(string key, object value)
    {
        if (key == "UserHand")
        {
            userHand = value.ToString();
        }
    }

    /**
     * @brief Subscribes to Leap Motion frame updates and user preference changes.
     */
    private void OnEnable()
    {
        leapProvider.OnUpdateFrame += OnUpdateFrame;
        UserPrefs.OnPreferenceChanged += updateHand;
    }

    /**
     * @brief Unsubscribes from Leap Motion frame updates and user preference changes when disabled.
     */
    private void OnDisable()
    {
        leapProvider.OnUpdateFrame -= OnUpdateFrame;
        UserPrefs.OnPreferenceChanged -= updateHand;
    }

    /**
     * @brief Checks if the audio is currently playing.
     * 
     * @param instance The FMOD event instance to check.
     * @return True if the event instance is playing; false otherwise.
     */
    private bool IsPlaying(EventInstance instance)
    {
        PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != PLAYBACK_STATE.STOPPED;
    }

    /**
     * @brief Calculates the closest distance from the hand to a target object.
     * 
     * @param hand The hand to calculate the distance from.
     * @param targetObj The target object.
     * @return The closest distance between the hand and the target object.
     */
    private float calcClosest(Hand hand, GameObject targetObj)
    {
        float palmDis = (hand.PalmPosition - targetObj.transform.position).sqrMagnitude;

        float minDis = palmDis; // start with dist from palm as default.

        // get fingers
        foreach (Finger fingerTip in hand.Fingers)
        {
            float fingerDis = (fingerTip.TipPosition - targetObj.transform.position).sqrMagnitude;
            if (fingerDis < minDis)
            {
                minDis = fingerDis;
            }
        }

        return Mathf.Sqrt(minDis);
    }

    /**
     * @brief Processes hand data each frame to control audio playback and parameters based on hand position and movement.
     * 
     * @param frame The current frame of hand data from Leap Motion.
     */
    void OnUpdateFrame(Frame frame) {
        //Use a helpful utility function to get the first hand that matches the Chirality
        Hand _leftHand = frame.GetHand(Chirality.Left);
        Hand _rightHand = frame.GetHand(Chirality.Right);
        if ((_leftHand != null && _rightHand != null) && !(menuEnabled || movingObject)){
            if (!isPlaying) {
                isPlaying = true;
                audioManagerEmitter.Play();
            }

            // get hand pref
            float pDis, vDis;
            pDis = calcClosest(userHand == "Right" ? _rightHand : _leftHand, pitchObj) - 0.125f;
            vDis = Vector3.Distance(userHand == "Right" ? _leftHand.PalmPosition : _rightHand.PalmPosition, volumeObj.transform.position) - 0.125f;

            vol = Mathf.Sqrt(Mathf.Max(vDis, 0.0f));

            // if volume is lower than 0, stop audio, else continue
            if (vol < threshold && IsPlaying(audioManagerEmitter.EventInstance)) {
                audioManagerEmitter.Stop();
            }
            else if (vol > threshold && !IsPlaying(audioManagerEmitter.EventInstance)) {
                audioManagerEmitter.Play();
            }

            audioManagerEmitter.EventInstance.setParameterByName("Volume", (Mathf.Max(vol, 0.0f)));
            audioManagerEmitter.EventInstance.setParameterByName("Pitch", pDis);

        }
        else if (isPlaying)
        {
            // stop theremin sound.
            isPlaying = false;

            audioManagerEmitter.Stop();
        }
    }

    // getters
    public float getVolume() { return vol; }

    // setters.
    public void setMenuBoolean(bool x) { menuEnabled = x; }

    public void setMovePad(bool x) { movingPad = x; }

    public void setMoveRod(bool x) { movingRod = x; }
}