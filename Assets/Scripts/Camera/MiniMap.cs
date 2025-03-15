using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform miniMapRect; // The RectTransform of the mini-map UI image
    [SerializeField] private Image miniMapImage; // The Image component that displays the mini-map
    [SerializeField] private GameObject player; // Reference to the player GameObject
    [SerializeField] private GameObject enemyIconPrefab; // Prefab for enemy icons
    [SerializeField] private Transform enemyContainer; // Parent object for enemies

    [Header("MiniMap Settings")]
    [SerializeField] private float scale = 60f; // Adjustable scale for zoom level

    private List<GameObject> enemyIcons = new List<GameObject>();

    private void Start()
    {
        // Initialize mini-map size
        miniMapRect.sizeDelta = new Vector2(300, 300);
        UpdateMiniMap();
    }

    private void Update()
    {
        UpdateMiniMap();
    }

    private void UpdateMiniMap()
    {
        // Clear previous enemy icons
        foreach (var icon in enemyIcons)
        {
            Destroy(icon);
        }
        enemyIcons.Clear();

        // Get the player's position
        Vector2 playerPosition = new Vector2(player.transform.position.x, player.transform.position.y);

        // Update mini-map image
        foreach (Transform enemy in enemyContainer)
        {
            if (enemy.gameObject.activeSelf)
            {
                // Create an enemy icon
                GameObject enemyIcon = Instantiate(enemyIconPrefab, miniMapImage.transform);
                RectTransform enemyIconRect = enemyIcon.GetComponent<RectTransform>();

                // Calculate enemy position relative to the player
                Vector2 enemyPosition = new Vector2(enemy.position.x, enemy.position.y);
                Vector2 direction = enemyPosition - playerPosition;

                // Set the position of the enemy icon on the mini-map
                Vector2 miniMapPosition = direction * scale;

                // Clamp the position to the mini-map boundaries
                miniMapPosition.x = Mathf.Clamp(miniMapPosition.x, -150, 150);
                miniMapPosition.y = Mathf.Clamp(miniMapPosition.y, -150, 150);

                enemyIconRect.anchoredPosition = miniMapPosition; // Position the enemy icon
                enemyIconRect.sizeDelta = new Vector2(2, 2); // Size of the enemy icon

                // Add to the list of icons
                enemyIcons.Add(enemyIcon);
            }
        }
    }
}