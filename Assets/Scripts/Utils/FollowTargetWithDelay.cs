using UnityEngine;

public class FollowTargetWithDelay : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The target object to follow.")]
    public Transform target;

    [Header("Following Parameters")]
    [Tooltip("The delay before this object starts following the target.")]
    public float followDelay = 0.5f;

    [Tooltip("The minimum distance to maintain between this object and the target.")]
    public float minimumDistance = 1.0f;

    [Tooltip("The maximum movement speed of this object.")]
    public float maxSpeed = 5.0f;

    [Tooltip("The distance at which the object starts to move slower.")]
    public float closeDistance = 2.0f;

    [Tooltip("The minimum stopping distance from the target.")]
    public float stoppingDistance = 0.5f;

    [Tooltip("The rotation speed of this object.")]
    public float rotationSpeed = 10.0f;

    [Tooltip("Rotation offset to adjust facing direction.")]
    public float rotationOffset = -90f;

    private Rigidbody2D rb;
    private Vector2 desiredPosition;
    private bool isMoving = false;
    private bool canFollow = false;
    private float followTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = maxSpeed / stoppingDistance; // Set the linear drag based on the maxSpeed and stoppingDistance
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate the direction towards the target
        Vector2 direction = (Vector2)(target.position - (Vector3)rb.position);
        float distanceToTarget = direction.magnitude;

        // If the distance is greater than the minimum stopping distance, move towards the target
        if (distanceToTarget > stoppingDistance)
        {
            isMoving = true;

            // Calculate the target speed based on distance to the target
            float targetSpeed = maxSpeed;
            if (distanceToTarget < closeDistance)
            {
                float t = distanceToTarget / closeDistance;
                targetSpeed = Mathf.Lerp(0f, maxSpeed, t * t); // Quadratic interpolation for smoother slowing down
            }

            // Move towards the target with the target speed
            Vector2 desiredVelocity = direction.normalized * targetSpeed;
            rb.linearVelocity = desiredVelocity;

            // Rotate towards the target
            RotateTowardsTarget(direction);

            // If the distance is greater than the minimum distance, start the follow delay timer
            if (distanceToTarget > minimumDistance)
            {
                followTimer += Time.deltaTime;
                if (followTimer >= followDelay)
                {
                    // Calculate the desired position with a delay
                    desiredPosition = target.position;
                    canFollow = true;
                }
            }
            else
            {
                // Reset the follow delay timer if the distance is less than the minimum distance
                followTimer = 0f;
                canFollow = false;
            }
        }
        else if (isMoving) // If the distance is less than the minimum and it was previously moving, stop moving
        {
            rb.linearVelocity = Vector2.zero;
            isMoving = false;
            followTimer = 0f; // Reset the follow delay timer when stopping
            canFollow = false;
        }

        // Apply the follow delay by interpolating the position when following is allowed
        if (canFollow)
        {
            desiredPosition = Vector2.Lerp(desiredPosition, target.position, Time.deltaTime / followDelay);
            rb.MovePosition(desiredPosition);
        }
    }

    private void RotateTowardsTarget(Vector2 direction)
    {
        if (direction.magnitude > 0.01f) // Ensure there is some direction to rotate
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            // Calculate the rotation step using rotationSpeed
            float step = rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
        }
    }
}
