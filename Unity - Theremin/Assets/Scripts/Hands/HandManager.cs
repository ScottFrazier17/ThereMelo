using Leap.Unity;
using UnityEngine;
using Leap;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;

public class HandManager : MonoBehaviour
{
    public LeapProvider leapProvider;
    public GameObject volumeObj;
    public GameObject pitchObj;

    public bool movingPad = false;
    public bool movingRod = false;
    public bool menuEnabled = false;
    public bool isPlaying;

    private bool movingObject => movingPad || movingRod;
    private StudioEventEmitter audioManagerEmitter;

    private string userHand;

    // Singleton instance
    public static HandManager instance { get; private set; }

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        }
        else{
            instance = this;
            GameObject audioManager = GameObject.Find("Audio Manager");
            if (audioManager != null)
            {
                audioManagerEmitter = audioManager.GetComponent<StudioEventEmitter>();
            }
            else {
                Debug.Log("Audio Manager not found?");
            }
        }
    }

    private void updateHand(string key, object value)
    {
        if (key == "UserHand")
        {
            userHand = value.ToString();
        }
    }

    private void OnEnable()
    {
        leapProvider.OnUpdateFrame += OnUpdateFrame;
        UserPrefs.OnPreferenceChanged += updateHand;
    }
    private void OnDisable()
    {
        leapProvider.OnUpdateFrame -= OnUpdateFrame;
        UserPrefs.OnPreferenceChanged -= updateHand;
    }

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
            if (userHand == "Right")
            {
                pDis = calcClosest(_rightHand, pitchObj);
                vDis = Vector3.Distance(_leftHand.PalmPosition, volumeObj.transform.position);
            }
            else
            {
                pDis = calcClosest(_leftHand, pitchObj);
                vDis = Vector3.Distance(_rightHand.PalmPosition, volumeObj.transform.position);
            }

            float volumeValue = vDis;
            float pitchValue = (1 / (pDis * 2)) - 1;

            audioManagerEmitter.EventInstance.setParameterByName("Volume", volumeValue);
            audioManagerEmitter.EventInstance.setParameterByName("Pitch", pitchValue);

        }
        else if (isPlaying)
        {
            // stop theremin sound.
            isPlaying = false;
            audioManagerEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    // setters.
    public void setMenuBoolean(bool x) { menuEnabled = x; }

    public void setMovePad(bool x) { movingPad = x; }

    public void setMoveRod(bool x) { movingRod = x; }
}