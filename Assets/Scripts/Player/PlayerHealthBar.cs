using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheProphecy.Player;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("Health Bar Settings")]
    [SerializeField] private Image[] healthImages; // Array to hold the health bar images
    [SerializeField] private Color healthyColor = Color.green; // Color for healthy segments
    [SerializeField] private Color yellowColor = Color.yellow; // Color for warning segments
    [SerializeField] private Color damagedColor = Color.red; // Color for critical segments
    [SerializeField] private Color emptyColor = Color.grey; // Color for empty segments

    [SerializeField] private TextMeshProUGUI healthText; // Reference to the TextMeshPro component

    private BasePlayer player; // Reference to the player script

    void Start()
    {
        player = FindObjectOfType<BasePlayer>(); // Find the player object in the scene
        if (player != null)
        {
            player.OnHealthChanged += UpdateHealthBar; // Subscribe to health change event
            UpdateHealthBar(); // Initialize health bar display
        }
        else
        {
            Debug.LogError("Player not found in the scene."); // Error log if player is not found
        }
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnHealthChanged -= UpdateHealthBar; // Unsubscribe to avoid memory leaks
        }
    }

    private void UpdateHealthBar()
    {
        if (player == null) return; // Exit if player is not found

        int maxHealth = player.MaxHealth; // Get maximum health from the property
        int currentHealth = player.health; // Get current health from the property

        Debug.Log($"Updating Health Bar: {currentHealth}/{maxHealth}"); // Debug log

        // Update the health text
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}"; // Display current health
        }

        // Calculate the number of segments to fill based on current health
        int segmentsToFill = Mathf.FloorToInt((float)currentHealth / maxHealth * healthImages.Length);

        for (int i = 0; i < healthImages.Length; i++)
        {
            // Determine the color based on health percentage
            Color segmentColor;
            float healthPercentage = (float)currentHealth / maxHealth;

            if (healthPercentage > 0.69f) // Above 69%
            {
                segmentColor = healthyColor; // Green
            }
            else if (healthPercentage > 0.29f) // Between 29% and 69%
            {
                segmentColor = yellowColor; // Yellow
            }
            else // 29% or below
            {
                segmentColor = damagedColor; // Red
            }

            healthImages[i].color = i < segmentsToFill ? segmentColor : emptyColor; // Fill color or empty color if not filled
        }
    }
}