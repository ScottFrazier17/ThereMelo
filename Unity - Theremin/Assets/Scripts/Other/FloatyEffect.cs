using UnityEngine;

/**
 * @brief Creates a floating effect on the GameObject to which it's attached by periodically adjusting its vertical position.
 */
public class FloatyEffect : MonoBehaviour
{
    /**
     * @brief Amplitude of the floating effect, determining the maximum height the GameObject moves up and down.
     */
    public float amplitude = 0.5f;

    /**
     * @brief Frequency of the floating effect, determining how fast the GameObject moves up and down.
     */
    public float frequency = 1f;

    /**
     * @brief The vertical offset from the starting position to the target position.
     */
    public float targetYOffset = 1f;

    /**
     * @brief The base position of the GameObject, set at Start.
     */
    private Vector3 basePosition;

    /**
     * @brief The target vertical position of the GameObject.
     */
    private float targetY;

    /**
     * @brief Initializes the effect by recording the base position and calculating the target vertical position.
     */
    void Start()
    {
        basePosition = transform.position;
        targetY = basePosition.y + targetYOffset;
    }

    /**
     * @brief Updates the vertical position of the GameObject to create a floating effect.
     */
    void Update()
    {
        float newY = Mathf.Sin(Time.time * frequency) * amplitude + targetY;
        transform.position = new Vector3(basePosition.x, newY, basePosition.z);
    }
}
