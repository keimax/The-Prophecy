// ToasterMessage.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class ToasterMessage : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private float displayDuration = 2f;

    public event Action OnToasterComplete;

    public void ShowMessage(Sprite itemSprite, string itemName)
    {
        itemImage.sprite = itemSprite;
        itemNameText.text = itemName;
        gameObject.SetActive(true);
        StartCoroutine(DisplayMessage());
    }

    private IEnumerator DisplayMessage()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 endPosition = new Vector2(startPosition.x, startPosition.y - rectTransform.rect.height - 25f);

        float halfDuration = displayDuration / 2f;
        float elapsed = 0f;

        // Move in
        while (elapsed < halfDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = endPosition;

        // Wait
        yield return new WaitForSeconds(halfDuration);

        // Move out
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(endPosition, startPosition, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = startPosition;

        gameObject.SetActive(false);
        OnToasterComplete?.Invoke();
        Destroy(gameObject);
    }

    private void OnRectTransformDimensionsChange()
    {
        if (gameObject.activeSelf)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            Vector2 startPosition = rectTransform.anchoredPosition;
            Vector2 endPosition = new Vector2(startPosition.x, startPosition.y - rectTransform.rect.height - 10f);
            rectTransform.anchoredPosition = endPosition;
        }
    }
}