using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Fade : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] float initialAlphaValue = 0f;
    [SerializeField] float finalAlphaValue = 1f;

    private void Awake()
    {
        StartCoroutine(FadeToAppear());
    }

    private IEnumerator FadeToAppear()
    {
        float elapsedTime = 0f;

        // Loop until the specified fade duration has elapsed
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float alphaValue = Mathf.Lerp(initialAlphaValue, finalAlphaValue, elapsedTime / fadeDuration);

            Color newColor = new Color(image.color.r, image.color.g, image.color.b, alphaValue);
            image.color = newColor;

            yield return null; // Wait until the next frame
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, finalAlphaValue);
    }
}
