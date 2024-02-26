using UnityEngine;
using FMODUnity;

public class SpinEffect : MonoBehaviour
{
    public float rotationSpeed = 50f;

    private StudioEventEmitter audioManagerEmitter;

    private void Start()
    {
        GameObject audioManager = GameObject.Find("Audio Manager");
        if (audioManager != null)
        {
            audioManagerEmitter = audioManager.GetComponent<StudioEventEmitter>();
        }
        else
        {
            Debug.Log("Audio Manager not found?");
        }
    }

    void Update()
    {
        audioManagerEmitter.EventInstance.getParameterByName("Pitch", out float t);
        transform.Rotate(0, 0, (rotationSpeed * t) * Time.deltaTime);
    }
}
