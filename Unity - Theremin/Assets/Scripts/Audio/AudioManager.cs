using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System.Runtime.InteropServices;
using System;
using FMOD;
using Leap.Unity;
using UnityEditor.Experimental.GraphView;
using FMOD.Studio;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEngine.UI;
using System.Drawing;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    public GameObject handManagement;

    private ChannelGroup channelGroup;
    private DSP fftDsp;

    private float yieldFreq = 3f;
    private Coroutine thread = null;
    private bool wasPlaying = false;
    private HandManager script;

    private float currMagnitude = 0f;
    private float[] spectrum = new float[512];

    private float frequency;
    private float curFreq;
    private string note;

    private LineRenderer lineRenderer;

    private int points = 25;
    private float length = 5f;
    private float lWidth = 0.025f;

    private float phase;
    private float speed = 1f;

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

        // get channel
        RuntimeManager.CoreSystem.getMasterChannelGroup(out channelGroup);

        // start fft dsp.
        RuntimeManager.CoreSystem.createDSPByType(DSP_TYPE.FFT, out fftDsp);
        fftDsp.setParameterInt((int)DSP_FFT.WINDOWTYPE, (int)DSP_FFT_WINDOW.HANNING);
        fftDsp.setParameterInt((int)DSP_FFT.WINDOWSIZE, 2048); // Window size for FFT
        channelGroup.addDSP(CHANNELCONTROL_DSP_INDEX.TAIL, fftDsp);

        // activate the fft and event, disable oscillator
        fftDsp.setActive(true);


        // visualizer stuff
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lWidth;
        lineRenderer.endWidth = lWidth;
    }

    void Update()
    {
        if (script.isPlaying && !wasPlaying)
        {
            if (thread == null)
            {
                thread = StartCoroutine(FFTAnalysisCoroutine());
            }
            wasPlaying = true;
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
            frequency = 0f;
            GetComponentInChildren<Text>().text = "";
        }

        // sine wave visualizer decor
        lineRenderer.positionCount = points;
        float halfLength = length / 2f;
        float amp = HandManager.instance.getVolume();
        for (int i = 0; i < points; i++)
        {
            float x = (i * (length / points)) - halfLength;
            float y = frequency > 0 ? amp * Mathf.Sin((x) + phase) : 0f;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
        phase += speed * Time.deltaTime;
    }

    private IEnumerator FFTAnalysisCoroutine()
    {
        while (script.isPlaying)
        {
            FFTAnalysis();
            yield return new WaitForSeconds(1f / yieldFreq);
        }
    }

    private string[] NOTES = { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#" };

    private UnityEngine.Color ConvertColor((int, int, int) colorTuple) { 
        return new UnityEngine.Color(colorTuple.Item1 / 255.0f, colorTuple.Item2 / 255.0f, colorTuple.Item3 / 255.0f); 
    }

    private string getNote(float freq)
    {
        if (freq == 0f) {
            lineRenderer.material.color = UnityEngine.Color.black;
            return "?" + 0.ToString(); 
        }
        
        double noteNumber = 12 * Math.Log(freq / 440, 2) + 49;
        noteNumber = Math.Round(noteNumber);
        int noteIndex = (int)(noteNumber) % NOTES.Length;
        if (noteIndex >= 0 && noteIndex < NOTES.Length)
        {
            string noteFound = NOTES[noteIndex];
            int octave = (int)((noteNumber + 8) / NOTES.Length);

            // set wave speed by notes
            speed = (float)noteNumber / 2;

            // set color and text
            GetComponentInChildren<Text>().text = noteFound + octave.ToString();
            ColorLerp colorLerp = GetComponent<ColorLerp>();
            if (colorLerp != null) { colorLerp.startLerp(noteIndex); }

            return noteFound + octave.ToString();
        }
        else 
        {
            lineRenderer.material.color = UnityEngine.Color.black;
            return "?" + 0.ToString();
        }

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

            // convert frequency to musical note and put into variables.
            note = getNote(frequency);

            // for speakers.
            var mag = Mathf.Abs(maxMagnitude);
            currMagnitude = mag < 0.1f ? 0 : mag;
        }
    }


    // public getters
    public float getMagnitude()
    {
        return currMagnitude;
    }

    public float getFreq() 
    {
        return frequency;
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        // plays audio once
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance PlayConstaint(EventReference sound, Vector3 worldPos)
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