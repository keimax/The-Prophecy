using UnityEngine;

public class Follow2DObject : MonoBehaviour
{
    public Transform target; // The 2D player object
    public Vector3 offset;   // Offset from the target

    private RectTransform rectTransform; // For UI elements

    void Start()
    {
        // Get the RectTransform if this is a UI element
        rectTransform = GetComponent<RectTransform>();

        if (target == null)
        {
            Debug.LogError("Target not assigned!");
            enabled = false; // Disable the script if no target is assigned
            return;
        }
    }

    void LateUpdate()
    {
        if (target == null) return; // Safeguard if target is destroyed

        if (rectTransform != null)
        {
            // For UI elements (using screen space)
            Vector2 viewportPoint = Camera.main.WorldToViewportPoint(target.position);
            Vector2 screenPoint = Camera.main.ViewportToScreenPoint(viewportPoint);

            // Apply offset in screen space
            screenPoint += (Vector2)offset;

            rectTransform.position = screenPoint;
            rectTransform.rotation = Quaternion.identity; // Prevent rotation
        }
        else
        {
            // For regular GameObjects (using world space)
            transform.position = target.position + offset;
            transform.rotation = Quaternion.identity; // Prevent rotation
        }
    }
}