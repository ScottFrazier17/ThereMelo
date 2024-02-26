using FMODUnity;
using Leap.Unity.Interaction;
using UnityEngine;

public class UserPrefs : MonoBehaviour
{
    private string userHand;
    private float userVolume;

    public GameObject UIAttachment;
    public GameObject VolumeBar;

    void Start() {
        // on launch, load prefs. if no prefs are found, load defaults
        userHand = PlayerPrefs.GetString("UserHand", "Left");
        userVolume = PlayerPrefs.GetFloat("UserVolume", 0.5F);

        // set prefs to action
        VolumeBar.GetComponent<InteractionSlider>().HorizontalSliderValue = userVolume;
        RuntimeManager.StudioSystem.setParameterByName("MasterVolume", userVolume);

        // TODO : change hands later.
    }
    
    public void updateValues() {
        float newVal = VolumeBar.GetComponent<InteractionSlider>().HorizontalSliderValue;
        PlayerPrefs.SetFloat("UserVolume", newVal);
        Debug.Log("Updated : " + newVal);
    }
}
