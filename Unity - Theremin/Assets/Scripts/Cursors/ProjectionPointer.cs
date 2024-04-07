using Leap;
using Leap.Unity;
using Leap.Unity.InputModule;
using UnityEditor;
using UnityEngine;

/**
 * @brief Manages the projection pointer for Leap Motion interaction, rendering a line between the hand and a UI cursor.
 */
public class ProjectionPointer : MonoBehaviour
{
    /**
     * @brief Reference to the UIInputModule, used to obtain Leap data provider and interaction mode.
     */
    [SerializeField] private UIInputModule module;

    /**
     * @brief Specifies the hand chirality (left or right) to track with the projection pointer.
     */
    [SerializeField] Chirality chirality;

    /**
     * @brief LineRenderer component used to draw the line between the hand and the cursor.
     */
    [SerializeField] private LineRenderer line;

     /**
     * @brief Reference to the UIInputCursor, marking the target position for the projection pointer.
     */
    [SerializeField] private UIInputCursor cursor;

    LeapProvider leapDataProvider;

    /**
     * @brief Stores the current hand data from Leap Motion.
     */
    private Hand hand;

    public float lerpSpeed = 10;

    /**
     * @brief Initializes the projection pointer by obtaining the Leap data provider from the UIInputModule.
     * See UIInputModule for more information on this.
     */
    private void Awake()
    {
        leapDataProvider = module.LeapDataProvider;
    }

    /**
     * @brief Updates the projection pointer, drawing a line from the hand to the UI cursor based on current Leap Motion data.
     */
    private void Update()
    {
        hand = leapDataProvider.CurrentFrame.GetHand(chirality);
        if (module.InteractionMode == InteractionCapability.Direct)
        {
            return;
        }

        if (hand == null)
        {
            return;
        }

        var projection = module.ProjectionOriginProvider;
        if (projection == null)
        {
            return;
        }

        var source = projection.ProjectionOriginForHand(hand);
        var target = cursor.transform.position;
        line.SetPosition(0, source);
        line.SetPosition(1, target);
    }
}