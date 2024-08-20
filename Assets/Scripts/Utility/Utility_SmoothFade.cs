using System.Collections;
using UnityEngine;

public class Utility_SmoothFade : MonoBehaviour
{
    [SerializeField] float fadeInDuration = 1f;
    [SerializeField] float duration = 1f;
    [SerializeField] float fadeOutDuration = 1f;

    private Material material;
    [SerializeField] private Color initialColor;

    void OnEnable()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer == null)
        {
            Debug.LogError("Renderer component missing!");
            return;
        }

        material = renderer.material;
        // initialColor = material.color;

        // Make the gameObject transparent at start
        material.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        Color transparentColor = initialColor;
        transparentColor.a = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeInDuration;
            material.color = Color.Lerp(transparentColor, initialColor, t);
            yield return null; // Wait for the next frame
        }

        // Ensure the GameObject is fully opaque after the duration
        material.color = initialColor;

        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        Color transparentColor = initialColor;
        transparentColor.a = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeOutDuration;
            material.color = Color.Lerp(initialColor, transparentColor, t);
            yield return null; // Wait for the next frame
        }

        // Ensure the GameObject is fully opaque after the duration
        material.color = transparentColor;

        // Optionally, disable the GameObject after fading out
        gameObject.SetActive(false);
    }
}