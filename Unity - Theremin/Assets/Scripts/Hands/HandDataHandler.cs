using Leap;
using Leap.Unity;
using UnityEngine;

public class HandDataHandler : MonoBehaviour
{
    public LeapProvider leapProvider;
    public GameObject volumeObj;
    public GameObject pitchObj;

    private void OnEnable(){
        leapProvider.OnUpdateFrame += OnUpdateFrame;
    }
    private void OnDisable(){
        leapProvider.OnUpdateFrame -= OnUpdateFrame;
    }

    void OnUpdateFrame(Frame frame){
        //Get a list of all the hands in the frame and loop through
        //to find the first hand that matches the Chirality
        foreach (var hand in frame.Hands)
        {
            if (hand.IsLeft)
            {
                //We found a left hand
            }
        }

        //Use a helpful utility function to get the first hand that matches the Chirality
        Hand _leftHand = frame.GetHand(Chirality.Left);
        Hand _rightHand = frame.GetHand(Chirality.Right);

        // print(Vector3.Distance(_rightHand.PalmPosition, );
    }
}
