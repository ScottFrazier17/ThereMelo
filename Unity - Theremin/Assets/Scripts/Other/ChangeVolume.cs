using FMODUnity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ChangeVolume : MonoBehaviour
{
    public void ChangeMasterVolValue()
    {
        InteractionSlider slider = this.GetComponent<InteractionSlider>();
        RuntimeManager.StudioSystem.setParameterByName("MasterVolume", slider.HorizontalSliderValue);
    }
}
