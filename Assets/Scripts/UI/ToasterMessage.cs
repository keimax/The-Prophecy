using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ToasterMessage : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float moveSpeed = 500f;

    public void ShowMessage(Sprite itemSprite, string itemName)
    {
        itemImage.sprite = itemSprite;
        itemNameText.text = itemName;
        gameObject.SetActive(true); // Ensure the toaster is active
        StartCoroutine(DisplayMessage());
    }

    private IEnumerator DisplayMessage()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 startPosition = rectTransform.anchoredPosition; // Use the prefab's initial position
        Vector2 endPosition = new Vector2(startPosition.x, startPosition.y - rectTransform.rect.height - 10); // Move into the viewport

        // Move in
        float elapsedTime = 0f;
        while (elapsedTime < displayDuration / 2)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / (displayDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = endPosition;

        // Wait
        yield return new WaitForSeconds(displayDuration / 2);

        // Move out
        elapsedTime = 0f;
        while (elapsedTime < displayDuration / 2)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(endPosition, startPosition, elapsedTime / (displayDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = startPosition;

        gameObject.SetActive(false); // Deactivate the toaster after use
    }

    private void OnRectTransformDimensionsChange()
    {
        // Ensure the toaster message is visible when the screen size changes
        if (gameObject.activeSelf)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            Vector2 startPosition = rectTransform.anchoredPosition; // Use the prefab's initial position
            Vector2 endPosition = new Vector2(startPosition.x, startPosition.y - rectTransform.rect.height - 10); // Move into the viewport
            rectTransform.anchoredPosition = endPosition;
        }
    }

    public float GetDisplayDuration()
    {
        return displayDuration;
    }
}

