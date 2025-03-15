using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Camera _camera; // Reference to the Camera component
    public Vector3 _offset;
    private Vector3 _velocity;
    public float smoothTime = 0.5f; // Increased smoothTime for slower follow

    // Zoom settings
    [Header("Zoom Settings")]
    public float maxZoom = 15f; // Maximum orthographic size
    public float minZoom = 5f; // Minimum orthographic size
    public float zoomOutSpeed = 2f; // Speed of zooming out
    public float zoomInSpeed = 2f; // Speed of zooming in
    public float speedForMaxZoom = 15f; // Speed at which max zoom is applied

    private void Start()
    {
        _velocity = Vector3.zero;
        _offset = Vector3.forward * -10; // Adjust if needed
        if (_target == null)
        {
            Debug.LogError("No target assigned to CameraFollow script!");
            enabled = false;
        }

        if (_camera == null)
        {
            _camera = GetComponent<Camera>();
        }
    }

    private void LateUpdate()
    {
        if (_target != null)
        {
            Vector3 desiredPosition = _target.position + _offset;

            // Smoothly move the camera to the desired position
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, smoothTime);

            // Calculate speed based on the target's Rigidbody2D if it exists
            float speed = _target.GetComponent<Rigidbody2D>().linearVelocity.magnitude;

            // Calculate the desired orthographic size based on speed
            float targetZoom = Mathf.Lerp(minZoom, maxZoom, speed / speedForMaxZoom);

            // Apply clamping to ensure the orthographic size stays within bounds
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

            // Determine the appropriate zoom speed based on whether zooming in or out
            float currentZoomSpeed = targetZoom < _camera.orthographicSize ? zoomInSpeed : zoomOutSpeed;

            // Apply the zoom
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, targetZoom, currentZoomSpeed * Time.deltaTime);
        }
    }
}
