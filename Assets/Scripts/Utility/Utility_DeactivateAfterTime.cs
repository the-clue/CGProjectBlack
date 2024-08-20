using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility_DeactivateAfterTime : MonoBehaviour
{
    [SerializeField] float timeUntilDeactivated = 5;
    private void Awake()
    {
        DeactivateAfterTime();
    }

    IEnumerator DeactivateAfterTime()
    {
        float elapsedTime = 0f;
        while (elapsedTime < timeUntilDeactivated)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
