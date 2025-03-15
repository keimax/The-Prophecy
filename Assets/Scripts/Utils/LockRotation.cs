using UnityEngine;

public class LockRotation : MonoBehaviour
{
    [SerializeField] private bool lockX = false;
    [SerializeField] private bool lockY = false;
    [SerializeField] private bool lockZ = true; // Typically you'd lock Z for 2D

    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        Quaternion lockedRotation = transform.rotation;

        if (lockX)
        {
            lockedRotation.eulerAngles = new Vector3(initialRotation.eulerAngles.x, lockedRotation.eulerAngles.y, lockedRotation.eulerAngles.z);
        }

        if (lockY)
        {
            lockedRotation.eulerAngles = new Vector3(lockedRotation.eulerAngles.x, initialRotation.eulerAngles.y, lockedRotation.eulerAngles.z);
        }

        if (lockZ)
        {
            lockedRotation.eulerAngles = new Vector3(lockedRotation.eulerAngles.x, lockedRotation.eulerAngles.y, initialRotation.eulerAngles.z);
        }

        transform.rotation = lockedRotation;
    }
}