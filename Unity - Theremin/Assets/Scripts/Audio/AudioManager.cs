using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System.Runtime.InteropServices;
using System;
using FMOD;
using Leap.Unity;
using UnityEditor.Experimental.GraphView;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    public GameObject handManagement;

    private DSP fftDsp;
    private ChannelGroup channelGroup;
    private float yieldFreq = 3f;
    private Coroutine thread = null;
    private bool wasPlaying = false;
    private HandManager script;

    private float currMagnitude = 0f;

    private void Awake()
    {
        if (instance != null)
        {
            UnityEngine.Debug.LogError("Multiple Audio Testers in Scene.");
            Destroy(gameObject);
            return;
        }
        instance = this;

        // set script
        script = handManagement.GetComponent<HandManager>();

        // start fft dsp.
        RuntimeManager.CoreSystem.getMasterChannelGroup(out channelGroup);
        RuntimeManager.CoreSystem.createDSPByType(DSP_TYPE.FFT, out fftDsp);
        fftDsp.setParameterInt((int)DSP_FFT.WINDOWTYPE, (int)DSP_FFT_WINDOW.HANNING);
        fftDsp.setParameterInt((int)DSP_FFT.WINDOWSIZE, 2048); // Window size for FFT
        channelGroup.addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, fftDsp);

        fftDsp.setActive(true);
    }

    void Update()
    {
        if (script.isPlaying && !wasPlaying)
        {
            if (thread == null)
            {
                thread = StartCoroutine(FFTAnalysisCoroutine());
            }
            wasPlaying= true;
        }
        else if (!script.isPlaying && wasPlaying)
        {
            if (thread != null)
            {
                UnityEngine.Debug.Log("Stopping Thread");
                StopCoroutine(thread);
                thread = null;

                currMagnitude = 0f; // set Mag to zero.
            }
            wasPlaying = false;
        }
    }


    private IEnumerator FFTAnalysisCoroutine()
    {
        while (script.isPlaying)
        {
            FFTAnalysis();
            yield return new WaitForSeconds(1f / yieldFreq);
        }
    }

    private Tuple<string, int> getNote(float freq)
    {
        if (freq == 0f) { return new Tuple<string, int>("?", 0); }
        string[] NOTES = { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#" };

        double noteNumber = 12 * Math.Log(freq / 440, 2) + 49;
        noteNumber = Math.Round(noteNumber);
        string note = NOTES[(int)(noteNumber) % NOTES.Length];
        int octave = (int)((noteNumber + 8) / NOTES.Length);

        return new Tuple<string, int>(note, octave);
    }

    void FFTAnalysis()
    {
        //fft dataa
        IntPtr unmanagedData; 
        uint length;
        fftDsp.getParameterData((int)DSP_FFT.SPECTRUMDATA, out unmanagedData, out length);
        DSP_PARAMETER_FFT fft = (DSP_PARAMETER_FFT)Marshal.PtrToStructure(unmanagedData, typeof(DSP_PARAMETER_FFT));

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

            float frequency;
            if (maxIndex > 0 && maxIndex < fft.length - 1)
            {
                float leftMagnitude = fft.spectrum[0][maxIndex - 1];
                float rightMagnitude = fft.spectrum[0][maxIndex + 1];
                float peakMagnitude = maxMagnitude;

                float interpolatedIndex = maxIndex + (leftMagnitude - rightMagnitude) / (2 * (2 * peakMagnitude - leftMagnitude - rightMagnitude));
                frequency = interpolatedIndex * sampleRate / (float)fft.length;
            }
            else
            {
                // handle the case where maxIndex is at the array boundary
                frequency = Mathf.Abs(maxIndex * sampleRate / (float)fft.length);
            }

            UnityEngine.Debug.Log("Detected Frequency: " + frequency + " Hz");

            // convert frequency to musical note
            var (note, octave) = getNote(frequency);
            UnityEngine.Debug.Log("Note: " + note + " Octave : " + octave);

            // for speakers.
            currMagnitude = maxMagnitude;
        }
    }

    // public getters
    public float getMagnitude()
    {
        return currMagnitude;
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