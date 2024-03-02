using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SoundAnimate : MonoBehaviour
{
    public AudioManager audioManager;

    private Vector3 defaultVec;
    public float speed = 0.0f;
    private float zMovement;
    private bool movingOutwards = true;

    private void Start()
    {
        defaultVec = transform.localPosition;
    }

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
