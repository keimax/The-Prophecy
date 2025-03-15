using System.Collections;
using UnityEngine;

public class LootItem : MonoBehaviour
{
    public enum ItemType
    {
        Health,
        Shield,
        Blaster,
        Thruster,
        XP,
        Credits
    }

    [SerializeField] public string itemName; // Name of the loot item
    [SerializeField] public int itemValue; // Value of the loot item
    [SerializeField] public ItemType itemType; // Changed from private to public
    [SerializeField] private float attractionSpeed = 5f;
    [SerializeField] private float attractionDelay = 0.5f; // Delay before attraction starts
    [SerializeField] private float attractionRadius = 5f; // Radius within which the item is attracted to the player
    [SerializeField] private float dampingFactor = 0.95f; // Factor to reduce velocity when out of range
    private Transform playerTransform;
    private Rigidbody2D rb;
    private bool canAttract = false;

    private void Awake()
    {
        // Start the scale at 300% of the initial scale set in the inspector
        transform.localScale *= 3f;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on loot item.");
        }

        Invoke(nameof(EnableAttraction), attractionDelay); // Enable attraction after delay

        // Start the scaling coroutine
        StartCoroutine(ShrinkToNormalScale());
    }

    private void EnableAttraction()
    {
        canAttract = true;
        Debug.Log("Attraction enabled");
    }

    private void FixedUpdate()
    {
        if (canAttract)
        {
            if (playerTransform == null)
            {
                // Find the player transform if not already set
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                    Debug.Log("Player transform found");
                }
            }

            if (playerTransform != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                if (distanceToPlayer <= attractionRadius)
                {
                    Vector2 direction = (playerTransform.position - transform.position).normalized;
                    Vector2 targetVelocity = direction * attractionSpeed;
                    rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * 2f); // Smoothly change velocity
                }
                else
                {
                    // Apply damping to gradually reduce velocity when out of range
                    rb.linearVelocity *= dampingFactor;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInventory playerInventory = collision.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
                playerInventory.AddItem(gameObject);
                gameObject.SetActive(false); // Deactivate the item after it has been added
                Debug.Log($"Loot collected by player: {itemName}, Value: {itemValue}, Type: {itemType}");
            }
            else
            {
                Debug.LogWarning("PlayerInventory component not found on player");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position to visualize the attraction range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attractionRadius);
    }

    private IEnumerator ShrinkToNormalScale()
    {
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = initialScale / 3f; // Shrink back to the initial scale set in the inspector
        float duration = 0.3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
}