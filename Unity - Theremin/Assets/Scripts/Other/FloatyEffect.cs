using UnityEngine;

public class FloatyEffect : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float frequency = 1f;
    public float targetYOffset = 1f;

    private Vector3 basePosition;
    private float targetY;

    void Start()
    {
        basePosition = transform.position;
        targetY = basePosition.y + targetYOffset;
    }

    void Update()
    {
        float newY = Mathf.Sin(Time.time * frequency) * amplitude + targetY;
        transform.position = new Vector3(basePosition.x, newY, basePosition.z);
    }
}
