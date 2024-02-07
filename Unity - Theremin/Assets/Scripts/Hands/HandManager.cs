using Leap.Unity;
using UnityEngine;
using Leap;
using FMODUnity;
using FMOD.Studio;

public class HandManager : MonoBehaviour
{
    public LeapProvider leapProvider;
    public GameObject volumeObj;
    public GameObject pitchObj;

    private bool isPlaying = false;
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
        if (_leftHand != null && _rightHand != null) {
            if (!isPlaying) {
                isPlaying = true;
                audioManagerEmitter.Play();
                Debug.Log("playing");
            }
            float v = Vector3.Distance(_leftHand.PalmPosition, volumeObj.transform.position);
            float p = Vector3.Distance(_rightHand.PalmPosition, pitchObj.transform.position);
           
            audioManagerEmitter.EventInstance.setParameterByName("Volume", v);
            audioManagerEmitter.EventInstance.setParameterByName("Pitch", p*2);

            audioManagerEmitter.EventInstance.getParameterByName("Volume", out float a);
            audioManagerEmitter.EventInstance.getParameterByName("Pitch", out float t);
            Debug.Log("Vol: " + v + " | " + a);
            Debug.Log("Pitch: " + p + " | " + t);
        }
        else if(isPlaying)
        {
            // stop theremin sound.
            isPlaying = false;
            audioManagerEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Debug.Log("stopping");
        }
    }

    // get hand.
    public float getDistance(GameObject obj1, GameObject obj2) {
        return Vector3.Distance(obj1.transform.position, obj2.transform.position);
    }
}