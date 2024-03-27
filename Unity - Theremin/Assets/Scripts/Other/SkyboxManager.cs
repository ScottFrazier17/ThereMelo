using FMOD;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Rendering;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    private HandManager script;

    private float speed = 0f;
    private float angleOffset = 10f;

    private float maxSpeed = 1f, minSpeed = 0.0f;

    [SerializeField] private GameObject handManagement;
    [SerializeField] private Material sky;

    private void Awake()
    {
        // set script
        script = handManagement.GetComponent<HandManager>();
    }

    void Update()
    {
        if (script.isPlaying && (speed < maxSpeed))
        {
            speed += Time.deltaTime * 0.1f;
            speed = Mathf.Min(speed, maxSpeed);

           
        }
        else if (!script.isPlaying)
        {
            speed -= Time.deltaTime * 0.2f;
            speed = Mathf.Max(speed, minSpeed);
        }

        if (speed > 0)
        {
            angleOffset += Time.deltaTime * speed;
        }
        sky.SetFloat("_AngleOffset", angleOffset);
    }
}
