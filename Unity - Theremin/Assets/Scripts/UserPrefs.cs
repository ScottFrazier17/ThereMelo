using FMODUnity;
using Leap.Unity;
using Leap.Unity.Interaction;
using System;
using System.Net.Mail;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class UserPrefs : MonoBehaviour
{
    private string userHand;
    private float userVolume;

    public GameObject UIAttachment;
    public GameObject VolumeBar;
    public GameObject leftHand;
    public GameObject rightHand;

    public HandModelBase lHandGeneric;
    public HandModelBase rHandGeneric;
    public GameObject menuAttachment;

    public delegate void PreferenceChangedEventHandler(string key, object value);
    public static event PreferenceChangedEventHandler OnPreferenceChanged;

    void Start() {
        // on launch, load prefs. if no prefs are found, load defaults
        userHand = PlayerPrefs.GetString("UserHand", "Right");
        userVolume = PlayerPrefs.GetFloat("UserVolume", 0.5F);

        // set prefs to action
        VolumeBar.GetComponent<InteractionSlider>().HorizontalSliderValue = userVolume;
        RuntimeManager.StudioSystem.setParameterByName("MasterVolume", userVolume);

        // set hands
        setHand(userHand);
    }
    
    public void updateValues() {
        float newVal = VolumeBar.GetComponent<InteractionSlider>().HorizontalSliderValue;
        PlayerPrefs.SetFloat("UserVolume", newVal);
    }

    public void setHand(string Hand)
    {
        Material domMat = Resources.Load<Material>("Dominant");
        Material nonDomMat = Resources.Load<Material>("Non-Dominant");

        // assume swap
        if (Hand == "") { Hand = (userHand == "Left" ? "Right" : "Left"); }

        // get children of ui
        Transform Buttons = null, Volume = null;
        Transform ui = UIAttachment.gameObject.transform.GetChild(0);

        for (int i = 0; i < ui.childCount; i++)
        {
            Transform child = ui.GetChild(i);
            if (child.name == "Buttons Panel")
            {
                Buttons = child;
            }
            else if (child.name == "Slider Panel")
            {
                Volume = child;
            }
        }

        Debug.Log($"{Hand} hand as Dominant Chirality");

        Transform leftMenu = null, rightMenu = null;
        // get left and right attachments for menu.
        foreach (Transform child in menuAttachment.transform)
        {
            leftMenu = child.name == "Attachment Hand (Left)" ? child : leftMenu;
            rightMenu = child.name == "Attachment Hand (Right)" ? child : rightMenu;
        }

        // set positions and parents based on the dominant hand
        (GameObject dominantHand, HandModelBase dominantHandBase, Transform dominantMenu) = Hand == "Left" ? 
            (leftHand, lHandGeneric, leftMenu) : (rightHand, rHandGeneric, rightMenu);

        (GameObject nonDominantHand, HandModelBase nonDomiantHandBase, Transform nonDominantMenu) = Hand == "Left" ?
            (rightHand, rHandGeneric, rightMenu) : (leftHand, lHandGeneric, leftMenu);

        // set palm detector to detect the non-dom hand.
        GetComponent<PalmDirectionDetector>().HandModel = nonDomiantHandBase;

        UIAttachment.transform.SetParent(nonDominantMenu.gameObject.transform.Find("Palm"), false);
        UIAttachment.transform.localPosition = new Vector3(Hand == "Left" ? 0.25f :  - 0.007f, 0.0066f, 0.05f);

        Buttons.transform.localPosition = new Vector3(Hand == "Left" ? 0.025f : -0.025f, 0f, 0f);
        Volume.transform.localPosition = new Vector3(Hand == "Left" ? -0.025f : 0.025f, 0f, 0f);

        // Set materials
        dominantHand.GetComponent<Renderer>().material = domMat;
        nonDominantHand.GetComponent<Renderer>().material = nonDomMat;

        // Save preferences
        userHand = Hand;
        PlayerPrefs.SetString("UserHand", Hand);
        OnPreferenceChanged?.Invoke("UserHand", Hand);

    }

}
