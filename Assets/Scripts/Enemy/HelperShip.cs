using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class HelperShip : MonoBehaviour
{
    [Header("Basic Ship Stats")]
    public float size = 1f;
    public float movementSpeed = 75f;
    public float maxLifetime = 20f;
    public GameObject target;
    public Sprite[] sprites;

    [Header("Movement Parameters")]
    [SerializeField] private float pursueDistance = 7f;
    [SerializeField] private float circleDistance = 4f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float repulsionMultiplier = 2.5f;
    [SerializeField] private float forwardBias = 1.5f;

    [Header("Rotation Parameters")]
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Advanced Movement")]
    [SerializeField] private float circleSpeed = 1.5f;
    [SerializeField] private float circleRadius = 5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Randomize sprite
        if (sprites.Length > 0)
            spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];

        // Physics setup
        rb.gravityScale = 0;
        rb.linearDamping = 0.5f;  // Updated from .drag
        rb.mass = size;
        rb.freezeRotation = true;

        // Self-destruct timer
        Destroy(gameObject, maxLifetime);
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

        // Advanced Collision Avoidance
        if (distanceToTarget < minDistance)
        {
            Vector2 toTarget = (target.transform.position - transform.position).normalized;
            Vector2 perpendicular = new Vector2(-toTarget.y, toTarget.x);

            // Create multi-directional evasion vector
            Vector2 avoidanceVector = (perpendicular * repulsionMultiplier + -toTarget).normalized;

            rb.AddForce(avoidanceVector * movementSpeed * 2f, ForceMode2D.Force);
        }
        else if (distanceToTarget > pursueDistance)
        {
            // Fast pursuit
            PursueTarget(1f);
        }
        else if (distanceToTarget > circleDistance)
        {
            // Slower approach
            PursueTarget(0.5f);
        }
        else
        {
            // Evasive circling
            CircleTarget();
        }

        // Always face movement direction
        RotateToMovementDirection();
    }

    private void PursueTarget(float speedMultiplier)
    {
        Vector2 direction = (target.transform.position - transform.position).normalized;
        rb.AddForce(direction * movementSpeed * speedMultiplier, ForceMode2D.Force);
    }

    private void CircleTarget()
    {
        Vector2 toTarget = (target.transform.position - transform.position).normalized;
        Vector2 perpendicular = new Vector2(-toTarget.y, toTarget.x);

        rb.AddForce(perpendicular * movementSpeed, ForceMode2D.Force);
    }

    private void RotateToMovementDirection()
    {
        if (rb.linearVelocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    // Prevent any automatic destruction
    private void OnCollisionEnter2D(Collision2D collision) { }
}