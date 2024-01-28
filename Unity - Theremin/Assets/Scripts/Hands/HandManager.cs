using Leap.Unity;
using UnityEngine;
using Leap;
using Unity.VisualScripting;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class HandManager : MonoBehaviour
{
    public LeapProvider leapProvider;
    public GameObject volumeObj;
    public GameObject pitchObj;

    // Singleton instance
    public static HandManager instance { get; private set; }

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        }
        else{
            instance = this;
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
        if (_leftHand != null) {
            Debug.Log("Vol: " + Vector3.Distance(_leftHand.PalmPosition, volumeObj.transform.position));
        }
        if (_rightHand != null)
        {
            Debug.Log("Pitch: " + Vector3.Distance(_rightHand.PalmPosition, pitchObj.transform.position));
        }
    }

    // get hand.
    public float getDistance(GameObject obj1, GameObject obj2) {
        return Vector3.Distance(obj1.transform.position, obj2.transform.position);
    }
}