using UnityEngine;

public class RotateSelf : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z }
    public RotationAxis rotationAxis = RotationAxis.Y;
    public float rotationSpeedPerMinute = 30f;

    private void Update()
    {
        float rotationSpeed = rotationSpeedPerMinute * Time.deltaTime / 60f;

        switch (rotationAxis)
        {
            case RotationAxis.X:
                transform.Rotate(Vector3.right * rotationSpeed);
                break;
            case RotationAxis.Y:
                transform.Rotate(Vector3.up * rotationSpeed);
                break;
            case RotationAxis.Z:
                transform.Rotate(Vector3.forward * rotationSpeed);
                break;
        }
    }
}