using System.Collections;
using UnityEngine;

public class Utility_SmoothAppearance : MonoBehaviour
{
    [SerializeField] float scaleDuration = 1;
    [SerializeField] Vector3 targetScale = Vector3.one;

    void Start()
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(ScaleUpRoutine());
    }

    IEnumerator ScaleUpRoutine()
    {
        float elapsedTime = 0;
        Vector3 initialScale = Vector3.zero;

        while (elapsedTime < scaleDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / scaleDuration);
            transform.localScale = Vector3.Lerp(initialScale, targetScale, progress);
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
