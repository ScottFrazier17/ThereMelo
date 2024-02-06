using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using static UnityEngine.Rendering.DebugUI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Multiple Audio Testers in Scene.");
        }
        instance = this;
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        // plays audio once
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public FMOD.Studio.EventInstance PlayConstaint(EventReference sound, Vector3 worldPos)
    {
        // Create instance
        FMOD.Studio.EventInstance eventInstance = RuntimeManager.CreateInstance(sound);

        // add to space.
        FMOD.ATTRIBUTES_3D attributes = new FMOD.ATTRIBUTES_3D();
        attributes.position = RuntimeUtils.ToFMODVector(worldPos);
        eventInstance.set3DAttributes(attributes);

        // start
        eventInstance.start();

        return eventInstance;
    }
}