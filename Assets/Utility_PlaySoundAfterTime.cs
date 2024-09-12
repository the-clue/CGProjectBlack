using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility_PlaySoundAfterTime : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    [SerializeField] float delay = 1f;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        audioSource.PlayDelayed(delay);
        audioSource.pitch = 1;
        audioSource.pitch += Random.Range(-0.2f, 0.2f);
    }
}
