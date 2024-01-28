using Leap.Unity.Interaction;
using UnityEngine;

public class HandHandler : MonoBehaviour
{
    public GameObject targetObject;

    private void Awake() {
       print(this.gameObject.GetComponent<HandDataMode>());
    }
    void Update() {
        if (targetObject != null) {
            float distance = HandManager.instance.getDistance(this.gameObject, targetObject);
            Debug.Log("Distance to Object: " + targetObject.name + " " + distance);
        }
    }
}