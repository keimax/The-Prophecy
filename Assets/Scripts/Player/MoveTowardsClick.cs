using UnityEngine;
using UnityEngine.InputSystem;

public class RotateTowardsInput : MonoBehaviour
{
    public float rotationSpeed = 200f; // Degrees per second
    public float rotationSmoothTime = 0.1f; // Smoothing time for rotation
    public float rotationOffset = 270f; // Offset in degrees
    public Camera mainCamera; // Assign in the Inspector
    public bool useScreenSpaceCamera = true; // Match your Inspector setting

    private PlayerInput playerInput;
    private InputAction moveAction; // Action for mouse/touch position
    private float currentRotationVelocity;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found on this GameObject.");
            enabled = false;
            return;
        }

        moveAction = playerInput.actions.FindAction("Move"); // Assuming "Move" is your position action
        if (moveAction == null)
        {
            Debug.LogError("Move action not found in Input Action Asset.");
            enabled = false;
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Main Camera not found. Please assign it in the Inspector or ensure a Camera with the 'MainCamera' tag exists.");
                enabled = false;
                return;
            }
        }
    }

    void Update()
    {
        // Get input position (mouse or touch)
        Vector2 inputPosition = moveAction.ReadValue<Vector2>();

        // Convert input position to world position
        Vector3 targetWorldPosition;
        if (useScreenSpaceCamera)
        {
            targetWorldPosition = mainCamera.ScreenToWorldPoint(inputPosition);
        }
        else
        {
            targetWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, transform.position.z - mainCamera.transform.position.z));
        }

        // Calculate target rotation
        Vector3 direction = targetWorldPosition - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;

        // Smoothly rotate towards target angle
        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref currentRotationVelocity, rotationSmoothTime, rotationSpeed);
        transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }
}