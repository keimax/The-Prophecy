using UnityEngine;

public class MoveTowardsClick : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 5f;
    public float rotationSmoothTime = 0.2f;
    public float moveSmoothTime = 0.1f;
    public float rotationOffset = 0f;
    public float decelerationRate = 0.95f; // Adjust this for deceleration (0-1, lower = faster deceleration)
    public float minSpeed = 0.5f;        // Minimum speed to prevent complete stop
    public Camera mainCamera;
    public bool useScreenSpaceCamera = false;

    private Vector3 moveDirection;
    private bool isMoving = false;
    private float targetAngle;
    private float currentRotationVelocity;
    private Vector3 currentMoveVelocity;
    private float currentSpeed; // Track the current speed

    void Start()
    {
        currentSpeed = speed; // Initialize current speed
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("No Main Camera found. Please assign it in the Inspector or ensure a Camera with the 'MainCamera' tag exists.");
                enabled = false;
                return;
            }
        }
    }

    void Update()
    {
        // Input handling
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleInput(Input.GetTouch(0).position);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            HandleInput(Input.mousePosition);
        }

        if (isMoving)
        {
            // Smooth rotation
            float currentAngle = transform.eulerAngles.z;
            float newAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref currentRotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, 0f, newAngle);

            // Smooth movement with deceleration
            currentSpeed *= decelerationRate; // Apply deceleration
            currentSpeed = Mathf.Max(currentSpeed, minSpeed); // Clamp to minimum speed

            transform.position += transform.up * currentSpeed * Time.deltaTime;
        }
    }

    private void HandleInput(Vector2 inputPosition)
    {
        if (useScreenSpaceCamera)
        {
            moveDirection = mainCamera.ScreenToWorldPoint(inputPosition) - transform.position;
        }
        else
        {
            moveDirection = mainCamera.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, transform.position.z - mainCamera.transform.position.z)) - transform.position;
        }
        moveDirection.Normalize();
        targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg + rotationOffset;
        isMoving = true;
        currentSpeed = speed; // Reset speed on new input
    }
}