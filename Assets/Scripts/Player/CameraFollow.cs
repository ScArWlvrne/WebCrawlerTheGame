using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The target the camera will follow

    [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -8f); // Offset from the target

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            5f * Time.deltaTime
        );
    }
}
