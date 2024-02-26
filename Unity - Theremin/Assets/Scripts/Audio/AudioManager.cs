using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System.Runtime.InteropServices;
using System;
using FMOD;
using Leap.Unity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    public GameObject handManagement;

    private DSP fftDsp;
    private ChannelGroup channelGroup;
    private float freq = 3f;
    private Coroutine thread = null;
    private HandManager script;

    private void Awake()
    {
        if (instance != null)
        {
            UnityEngine.Debug.LogError("Multiple Audio Testers in Scene.");
        }
        instance = this;

        // start fft dsp. TODO : make it toggle on hand appearance.
        RuntimeManager.CoreSystem.getMasterChannelGroup(out channelGroup);
        RuntimeManager.CoreSystem.createDSPByType(DSP_TYPE.FFT, out fftDsp);
        fftDsp.setParameterInt((int)DSP_FFT.WINDOWTYPE, (int)DSP_FFT_WINDOW.HANNING);
        fftDsp.setParameterInt((int)DSP_FFT.WINDOWSIZE, 1024 * 2); // Window size for FFT
        channelGroup.addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, fftDsp);

        fftDsp.setActive(true);
        StartCoroutine(FFTAnalysisCoroutine());
    }

    void Update()
    {
        script = handManagement.GetComponent<HandManager>();
        if (script.handsEnabled && thread != null)
        {
            thread = StartCoroutine(FFTAnalysisCoroutine());
        }
        else if (!script.handsEnabled && thread != null)
        {
            UnityEngine.Debug.Log("Stopping Thread");
            StopCoroutine(thread);
            thread = null;
        }
    }


    private IEnumerator FFTAnalysisCoroutine()
    {
        HandManager script = handManagement.GetComponent<HandManager>();
        while (true)
        {
            if (script.handsEnabled)
            {
                FFTAnalysis();
            }
            yield return new WaitForSeconds(1f / freq);
        }
    }
    void FFTAnalysis()
    {
        //fft dataa
        IntPtr unmanagedData;
        uint length;
        fftDsp.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out unmanagedData, out length);
        FMOD.DSP_PARAMETER_FFT fft = (FMOD.DSP_PARAMETER_FFT)System.Runtime.InteropServices.Marshal.PtrToStructure(unmanagedData, typeof(FMOD.DSP_PARAMETER_FFT));

        if (fft.numchannels > 0)
        {
            // analyze channel.
            float maxMagnitude = 0f;
            int maxIndex = 0;
            for (int i = 0; i < fft.length; i++)
            {
                float magnitude = fft.spectrum[0][i];
                if (magnitude > maxMagnitude)
                {
                    maxMagnitude = magnitude;
                    maxIndex = i;
                }
            }
            int sampleRate;
            RuntimeManager.CoreSystem.getSoftwareFormat(out sampleRate, out _, out _);
            float frequency = maxIndex * sampleRate / (float)fft.length;
            UnityEngine.Debug.Log("Detected Frequency: " + frequency + " Hz");

            //convert frequency to musical note later
        }
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