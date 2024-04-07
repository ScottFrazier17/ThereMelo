using FMODUnity;
using Leap.Unity;
using Leap.Unity.Interaction;
using System;
using System.Net.Mail;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

/**
 * @brief Manages user preferences for a VR application, including hand dominance and volume settings.
 */
public class UserPrefs : MonoBehaviour
{
    /**
     * @brief Stores the user's hand preference.
     */
    private string userHand;

    /**
     * @brief Stores the user's preferred volume level.
     */
    private float userVolume;

    /**
     * @brief Delegate for handling preference change events.
     */
    public delegate void PreferenceChangedEventHandler(string key, object value);

    /**
     * @brief Event triggered when a preference changes.
     */
    public static event PreferenceChangedEventHandler OnPreferenceChanged;

    private GameObject rod, pad;

    /**
     * @brief Anchors for left and right hand positions.
     */
    private Anchor left, right;

    [SerializeField] private GameObject MenuUI, VolumeBar, leftHand, rightHand;
    [SerializeField] private HandModelBase lHandGeneric, rHandGeneric;
    [SerializeField] private GameObject menuAttachment, ThereminObject;

    /**
     * @brief Initializes user preferences, setting default values if none are found.
     * @details This sets by default the following settings to their resepects values
     * UserHand: Right
     * UserVolume: 0.5F
     * MasterVolume: userVolume
     */
    void Start() {
        // on launch, load prefs. if no prefs are found, load defaults
        userHand = PlayerPrefs.GetString("UserHand", "Right");
        userVolume = PlayerPrefs.GetFloat("UserVolume", 0.5F);

        // set prefs to action
        VolumeBar.GetComponent<InteractionSlider>().HorizontalSliderValue = userVolume;
        RuntimeManager.StudioSystem.setParameterByName("MasterVolume", userVolume);

        // set rod and pad
        rod = ThereminObject.transform.Find("Rod").gameObject;
        pad = ThereminObject.transform.Find("Pad").gameObject;

        // set anchors
        left = ThereminObject.transform.Find("Anchors/LeftAnchor").GetComponent<Anchor>();
        right = ThereminObject.transform.Find("Anchors/RightAnchor").GetComponent<Anchor>();

        // set hands
        setHand(userHand);
    }
    
    /**
     * @brief Updates the user volume preference based on interaction with the volume bar.
     */
    public void updateValues() {
        float newVal = VolumeBar.GetComponent<InteractionSlider>().HorizontalSliderValue;
        PlayerPrefs.SetFloat("UserVolume", newVal);
    }

    /**
     * @brief Swaps the positions of the rod and pad based on the user's hand dominance.
     * 
     * @param Hand The hand dominance to swap anchors for.
     */
    private void swapAnchors(string Hand)
    {
        // get anchorables
        AnchorableBehaviour rBehaviour = rod.GetComponent<AnchorableBehaviour>();
        AnchorableBehaviour pBehaviour = pad.GetComponent<AnchorableBehaviour>();

        // detach
        rBehaviour.Detach();
        pBehaviour.Detach();

        // assign rod and pad to positions
        Vector3 rPos = right.gameObject.transform.position, lPos = left.gameObject.transform.position;
        rod.transform.position = Hand == "Right" ? rPos : lPos;
        pad.transform.position = Hand == "Right" ? lPos : rPos;

        // try to attach
        rBehaviour.TryAttachToNearestAnchor();
        pBehaviour.TryAttachToNearestAnchor();
    }

    /**
     * @brief Sets the user's hand dominance, updating UI elements and interaction anchors accordingly.
     * 
     * @param Hand The hand dominance (either "Left" or "Right").
     */
    public void setHand(string Hand)
    {
        Material domMat = Resources.Load<Material>("Dominant");
        Material nonDomMat = Resources.Load<Material>("Non-Dominant");

        // assume swap
        if (Hand == "") { Hand = (userHand == "Left" ? "Right" : "Left"); }

        // swap anchors depending on hand.
        swapAnchors(Hand);

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

        // set palm detectors to non dominant hand.
        GetComponent<PalmDirectionDetector>().HandModel = nonDomiantHandBase;

        // get children of menu ui
        Transform Buttons = null, Volume = null, InstruList = null;
        Transform ui = MenuUI.gameObject.transform.GetChild(0);

        Buttons = ui.transform.Find("Buttons Panel");
        Volume = ui.transform.Find("Slider Panel");
        InstruList = ui.transform.Find("Instrument Panel");

        // disable instrument list
        InstruList.gameObject.SetActive(false);

        // set positions for menu
        MenuUI.transform.SetParent(nonDominantMenu.gameObject.transform.Find("Palm"), false);
        MenuUI.transform.localPosition = new Vector3(Hand == "Left" ? 0.25f :  - 0.007f, 0.0066f, 0.05f);

        Buttons.transform.localPosition = new Vector3(Hand == "Left" ? 0.025f : -0.025f, 0f, 0f);
        Volume.transform.localPosition = new Vector3(Hand == "Left" ? -0.025f : 0.025f, 0f, 0f);
        InstruList.transform.localPosition = new Vector3(Hand == "Left" ? -0.0525f : 0.0525f, 0f, 0f);

        // hand icon swap
        Buttons.transform.Find("HandBase/Hand/RoundButtonCircle/Icon").GetComponent<SpriteRenderer>().flipX = Hand == "Left" ? false : true;

        // set materials
        dominantHand.GetComponent<Renderer>().material = domMat;
        nonDominantHand.GetComponent<Renderer>().material = nonDomMat;

        // save preferences
        userHand = Hand;
        PlayerPrefs.SetString("UserHand", Hand);
        OnPreferenceChanged?.Invoke("UserHand", Hand);
    }
}
