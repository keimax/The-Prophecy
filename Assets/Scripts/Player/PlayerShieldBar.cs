using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheProphecy.Player;

public class PlayerShieldBar : MonoBehaviour
{
    [Header("Shield Bar Settings")]
    [SerializeField] private Image[] shieldImages; // Array to hold the shield bar images
    [SerializeField] private Color healthyColor = Color.blue; // Color for healthy segments
    [SerializeField] private Color yellowColor = Color.yellow; // Color for warning segments
    [SerializeField] private Color damagedColor = Color.red; // Color for critical segments
    [SerializeField] private Color emptyColor = Color.grey; // Color for empty segments

    [SerializeField] private TextMeshProUGUI shieldText; // Reference to the TextMeshPro component

    private BasePlayer player; // Reference to the player script

    void Start()
    {
        player = FindObjectOfType<BasePlayer>(); // Find the player object in the scene
        if (player != null)
        {
            player.OnShieldChanged += UpdateShieldBar; // Subscribe to shield change event
            UpdateShieldBar(); // Initialize shield bar display
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
            player.OnShieldChanged -= UpdateShieldBar; // Unsubscribe to avoid memory leaks
        }
    }

    private void UpdateShieldBar()
    {
        if (player == null) return; // Exit if player is not found

        int maxShield = player.MaxShield; // Get maximum shield from the property
        int currentShield = player.shield; // Get current shield from the property

        Debug.Log($"Updating Shield Bar: {currentShield}/{maxShield}"); // Debug log

        // Update the shield text
        if (shieldText != null)
        {
            shieldText.text = $"{currentShield}/{maxShield}"; // Display current shield
        }

        // Calculate the number of segments to fill based on current shield
        int segmentsToFill = Mathf.FloorToInt((float)currentShield / maxShield * shieldImages.Length);

        for (int i = 0; i < shieldImages.Length; i++)
        {
            // Determine the color based on shield percentage
            Color segmentColor;
            float shieldPercentage = (float)currentShield / maxShield;

            if (shieldPercentage > 0.69f) // Above 69%
            {
                segmentColor = healthyColor; // Blue
            }
            else if (shieldPercentage > 0.29f) // Between 29% and 69%
            {
                segmentColor = yellowColor; // Yellow
            }
            else // 29% or below
            {
                segmentColor = damagedColor; // Red
            }

            shieldImages[i].color = i < segmentsToFill ? segmentColor : emptyColor; // Fill color or empty color if not filled
        }
    }
}