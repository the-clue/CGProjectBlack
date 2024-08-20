using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility_ActivateDeactivateCollider : MonoBehaviour
{
    [SerializeField] Collider damageCollider;
    [SerializeField] float timeUntilActivated = 1f;
    [SerializeField] float timeUntilDeactivated = 1.5f;

    private void Awake()
    {
        if (damageCollider == null)
        {
            damageCollider = GetComponent<Collider>();
        }

        StartCoroutine(ActivateDeactivateAfterTime());
    }

    IEnumerator ActivateDeactivateAfterTime()
    {
        float elapsedTime = 0f;

        while (elapsedTime < timeUntilActivated)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        damageCollider.enabled = true;

        while (elapsedTime < timeUntilDeactivated)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        damageCollider.enabled = false;
    }
}
