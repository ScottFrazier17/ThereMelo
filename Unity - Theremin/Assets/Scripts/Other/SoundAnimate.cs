using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/**
 * @brief Animates a GameObject's position in response to audio amplitude, creating a visual representation of sound intensity.
 */
public class SoundAnimate : MonoBehaviour
{
    /**
     * @brief Reference to the AudioManager, used to obtain the current amplitude of playing audio.
     */
    public AudioManager audioManager;

    /**
     * @brief The GameObject's original local position, used as a base for animation.
     */
    private Vector3 defaultVec;
    public float speed = 0.0f;
    private float zMovement;

    /**
     * @brief Flag indicating whether the GameObject is currently moving outwards or returning to its original position.
     */
    private bool movingOutwards = true;

    /**
     * @brief Initializes the SoundAnimate script by recording the GameObject's original local position.
     */
    private void Start()
    {
        defaultVec = transform.localPosition;
    }

    /**
     * @brief Updates the GameObject's position based on audio amplitude, creating a forward and backward animation effect.
     */
    void Update()
    {
        if (audioManager != null)
        {
            // get amp, then animate.
            float temp = 1/audioManager.getMagnitude();
            float amp = Mathf.Clamp(((float.IsNaN(temp) || float.IsInfinity(temp)) ? 0.0f : temp) * 0.05f, 0f, 3.5f);
            speed = Mathf.Lerp(speed, amp, Time.deltaTime);

            if (movingOutwards)
            {
                // Move outwards
                zMovement += Time.deltaTime * speed;
                if (zMovement > 0.05f)
                {
                    movingOutwards = false;
                }
            }
            else
            {
                // Glide back to initial position
                zMovement -= Time.deltaTime * (speed / 2); // slower return
                if (zMovement < 0)
                {
                    zMovement = 0;
                    movingOutwards = true; // reset
                }
            }

            // apply the movement
            transform.localPosition = new Vector3(defaultVec.x, defaultVec.y, defaultVec.z + zMovement);
        }
    }
}
