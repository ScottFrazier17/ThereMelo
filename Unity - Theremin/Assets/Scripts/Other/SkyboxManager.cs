using FMOD;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Rendering;
using UnityEngine;

/**
 * @brief Manages the rotation of the skybox in response to the music playing status from HandManager.
 */
public class SkyboxManager : MonoBehaviour
{
    /**
     * @brief Reference to the HandManager script to check if music is playing.
     */
    private HandManager script;

    private float speed = 0f;
    private float angleOffset = 10f;

    private float maxSpeed = 1f, minSpeed = 0.0f;

    /**
     * @brief GameObject that manages the hand interactions.
     */
    [SerializeField] private GameObject handManagement;

    /**
     * @brief The skybox material to be rotated.
     */
    [SerializeField] private Material sky;

    /**
     * @brief Initializes the SkyboxManager by obtaining the HandManager component from the specified GameObject.
     */
    private void Awake()
    {
        // set script
        script = handManagement.GetComponent<HandManager>();
    }

    /**
     * @brief Updates the skybox's rotation speed and applies the rotation based on the music playing status.
     */
    void Update()
    {
        if (script.isPlaying && (speed < maxSpeed))
        {
            speed += Time.deltaTime * 0.5f;
            speed = Mathf.Min(speed, maxSpeed);
        }
        
        else if (!script.isPlaying)
        {
            speed -= Time.deltaTime * 1f;
            speed = Mathf.Max(speed, minSpeed);
        }

        if (speed > 0)
        {
            angleOffset += Time.deltaTime * speed;
        }
        sky.SetFloat("_AngleOffset", angleOffset);
    }
}
