using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageFader : MonoBehaviour
{
    public enum FadeType { FadeIn, FadeOut }

    [SerializeField] private Image imageToFade;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private FadeType fadeType = FadeType.FadeIn;
    [SerializeField] private bool startFadedOut = false;
    [SerializeField] private bool startOnAwake = false;

    private Coroutine currentFadeCoroutine;
    private bool hasOriginalSprite;
    private Sprite originalSprite;

    private void Awake()
    {
        if (imageToFade != null)
        {
            hasOriginalSprite = imageToFade.sprite != null;
            originalSprite = imageToFade.sprite;

            if (startFadedOut)
            {
                SetAlpha(0f);
            }
        }
    }

    private void Start()
    {
        Debug.Log("Fade start function");
        if (startOnAwake)
            StartFade();
    }

    public void StartFade()
    {
        Debug.Log("StartFade() called");

        if (imageToFade == null)
        {
            Debug.LogError("Image to fade is not assigned!");
            return;
        }

        if (!hasOriginalSprite && imageToFade.sprite == null)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            imageToFade.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            imageToFade.type = Image.Type.Simple;
        }

        float startAlpha = imageToFade.color.a;
        float targetAlpha = (fadeType == FadeType.FadeIn) ? 1f : 0f;
        Fade(startAlpha, targetAlpha);
    }

    public void ForceSetAlpha(float alpha)
    {
        SetAlpha(alpha);
    }

    private void SetAlpha(float alpha)
    {
        if (imageToFade == null) return;
        Color c = imageToFade.color;
        c.a = alpha;
        imageToFade.color = c;
    }

    public void SetFadeType(FadeType newFadeType)
    {
        fadeType = newFadeType;
    }

    private void Fade(float startAlpha, float targetAlpha)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(FadeCoroutine(startAlpha, targetAlpha));
    }

    private IEnumerator FadeCoroutine(float startAlpha, float targetAlpha)
    {
        Debug.Log("FadeCoroutine() started with startAlpha: " + startAlpha + ", targetAlpha: " + targetAlpha);

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            SetAlpha(currentAlpha);
            yield return null;
        }

        SetAlpha(targetAlpha);
        currentFadeCoroutine = null;
    }

    private void OnDisable()
    {
        if (!hasOriginalSprite && imageToFade != null)
        {
            imageToFade.sprite = null;
        }
    }
}