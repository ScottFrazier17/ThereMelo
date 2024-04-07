using Leap;
using Leap.Unity;
using Leap.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Represents a squishy pointer that visualizes hand pinching actions using Leap Motion.
 * 
 * This class is responsible for adjusting the position, scale, and color of a pointer based on the user's hand pinch strength.
 */
public class SquishyPointer : MonoBehaviour
{
    /**
     * @brief Reference to the UIInputModule, used to obtain the Leap data provider.
     */
    [SerializeField] private UIInputModule module;

    /**
     * @brief PointerElement that the squishy pointer will point towards.
     */
    [SerializeField] PointerElement pointerElement;

    /**
     * @brief Specifies the hand chirality (left or right) to track with the squishy pointer.
     */
    [SerializeField] Chirality chirality;
    [SerializeField] MeshRenderer meshRenderer;

    LeapProvider leapDataProvider;

    public float lerpSpeed = 10;

    /**
     * @brief Initializes the squishy pointer by setting up Leap Motion data provider and initializing the pointer material.
     */
    private void Awake()
    {
        leapDataProvider = module.LeapDataProvider;
        meshRenderer.material = new Material(Shader.Find("Unlit/Color"));
    }

    /** 
     * The pointer interpolates towards the hand's pinch position, changes scale based on pinch strength,
     * and updates its color from white to green based on the pinch strength.
     */
    private void Update()
    {
        var hand = leapDataProvider.CurrentFrame.GetHand(chirality);

        if (hand == null) return;

        transform.position = Vector3.Lerp(meshRenderer.transform.position, hand.GetPinchPosition(), Time.deltaTime * lerpSpeed);
        transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1, 0.5f, 0.5f), hand.PinchStrength);
        transform.LookAt(pointerElement.transform);
        meshRenderer.material.color = Color.Lerp(Color.white, Color.green, hand.PinchStrength);
    }
}