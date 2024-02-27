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

    private bool movingObject => movingRod || movingRod;
    private StudioEventEmitter audioManagerEmitter;
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

    private void OnEnable()
    {
        leapProvider.OnUpdateFrame += OnUpdateFrame;
    }
    private void OnDisable()
    {
        leapProvider.OnUpdateFrame -= OnUpdateFrame;
    }

    void OnUpdateFrame(Frame frame) {
        //Use a helpful utility function to get the first hand that matches the Chirality
        Hand _leftHand = frame.GetHand(Chirality.Left);
        Hand _rightHand = frame.GetHand(Chirality.Right);
        if ((_leftHand != null && _rightHand != null) && !(menuEnabled || movingObject)){
            if (!isPlaying) {
                isPlaying = true;
                audioManagerEmitter.Play();
                Debug.Log("playing");
            }
            float pDis = Vector3.Distance(_rightHand.PalmPosition, pitchObj.transform.position);
            float vDis = Vector3.Distance(_leftHand.PalmPosition, volumeObj.transform.position);

            // get fingers
            foreach (Finger finger in _rightHand.Fingers) {
                float currentPos = Vector3.Distance(finger.TipPosition, pitchObj.transform.position);
                if (currentPos < pDis)
                {
                    pDis = currentPos;
                }
            }

            float volumeValue = vDis;
            float pitchValue = (1 / (pDis * 2)) - 1;

            audioManagerEmitter.EventInstance.setParameterByName("Volume", volumeValue);
            audioManagerEmitter.EventInstance.setParameterByName("Pitch", pitchValue);

        }
        else if (isPlaying)
        {
            // stop theremin sound. TODO : seperate into its seperate function.
            isPlaying = false;
            audioManagerEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Debug.Log("stopping");
        }
    }

    // setters.
    public void setMenuBoolean(bool x) { menuEnabled = x; }

    public void setMovePad(bool x) { movingPad = x; }

    public void setMoveRod(bool x) { movingRod = x; }
}