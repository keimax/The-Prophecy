using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyShip : MonoBehaviour
{
    [Header("Basic Ship Stats")]
    public float movementSpeed = 75f;

    [Header("Detection Parameters")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Movement Parameters")]
    [SerializeField] private float pursueDistance = 7f;
    [SerializeField] private float circleDistance = 4f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float repulsionMultiplier = 2.5f;

    [Header("Idle Wandering")]
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float wanderChangeInterval = 5f;

    [Header("Rotation Parameters")]
    [SerializeField] private float maxRotationSpeed = 120f;  // Max degrees per second
    [SerializeField] private float rotationAcceleration = 90f;  // Rotation acceleration degrees per second squared

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 idleTargetPosition;
    private float wanderTimer;
    private bool isIdle = true;
    public  GameObject target;

    // Smooth rotation variables
    private float currentRotationSpeed = 0f;
    private Quaternion targetRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetRotation = transform.rotation;
    }

    private void Start()
    {
        rb.gravityScale = 0;
        rb.linearDamping = 0.5f;
        rb.freezeRotation = true;

        SetRandomIdleTarget();
    }

    private void Update()
    {
        // Smooth rotation in Update for framerate independence
        SmoothRotation();
    }

    private void SmoothRotation()
    {
        // Gradually adjust rotation speed
        float angleToTarget = Quaternion.Angle(transform.rotation, targetRotation);

        // Determine rotation direction
        float rotationDirection = Mathf.Sign(Quaternion.Dot(transform.rotation, targetRotation));

        // Accelerate or decelerate rotation
        if (angleToTarget > 0.1f)
        {
            currentRotationSpeed = Mathf.Min(
                currentRotationSpeed + rotationAcceleration * Time.deltaTime,
                maxRotationSpeed
            );
        }
        else
        {
            currentRotationSpeed = Mathf.Max(currentRotationSpeed - rotationAcceleration * Time.deltaTime, 0);
        }

        // Apply smooth rotation
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            currentRotationSpeed * Time.deltaTime
        );
    }

    private void FixedUpdate()
    {
        Collider2D playerInRange = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (playerInRange != null)
        {
            target = playerInRange.gameObject;
            isIdle = false;
        }
        else
        {
            target = null;
            isIdle = true;
        }

        if (isIdle)
        {
            HandleIdleMovement();
        }
        else if (target != null)
        {
            HandleTargetedMovement();
        }

        ConstrainMovementToForward();
    }

    private void ConstrainMovementToForward()
    {
        Vector2 forwardDirection = transform.up;
        Vector2 currentVelocity = rb.linearVelocity;

        float forwardSpeed = Vector2.Dot(currentVelocity, forwardDirection);

        rb.linearVelocity = forwardDirection * forwardSpeed;
    }

    private void HandleIdleMovement()
    {
        wanderTimer += Time.fixedDeltaTime;
        if (wanderTimer >= wanderChangeInterval)
        {
            SetRandomIdleTarget();
            wanderTimer = 0;
        }

        Vector2 direction = (idleTargetPosition - (Vector2)transform.position).normalized;

        // Set target rotation smoothly
        targetRotation = Quaternion.LookRotation(Vector3.forward, direction);

        rb.AddForce(transform.up * movementSpeed * 0.5f, ForceMode2D.Force);
    }

    private void HandleTargetedMovement()
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

        Vector2 direction = (target.transform.position - transform.position).normalized;

        // Set target rotation smoothly
        targetRotation = Quaternion.LookRotation(Vector3.forward, direction);

        if (distanceToTarget < minDistance)
        {
            Vector2 toTarget = (target.transform.position - transform.position).normalized;
            Vector2 perpendicular = new Vector2(-toTarget.y, toTarget.x);

            Vector2 avoidanceVector = (perpendicular * repulsionMultiplier + -toTarget).normalized;

            // Set avoidance rotation smoothly
            targetRotation = Quaternion.LookRotation(Vector3.forward, avoidanceVector);

            rb.AddForce(transform.up * movementSpeed * 2f, ForceMode2D.Force);
        }
        else
        {
            rb.AddForce(transform.up * movementSpeed, ForceMode2D.Force);
        }
    }

    private void SetRandomIdleTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        idleTargetPosition = (Vector2)transform.position + randomOffset;
    }

    private void OnCollisionEnter2D(Collision2D collision) { }
}