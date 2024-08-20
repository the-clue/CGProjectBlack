using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFootstepSFXMaker : MonoBehaviour
{
    CharacterManager character;
    AudioSource audioSource;
    GameObject steppedOnObject;

    private bool hasTouchedGround = false;
    private bool hasPlayedFootStepSFX = false;
    [SerializeField] float distanceToGround = 0.05f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        character = GetComponentInParent<CharacterManager>();
    }

    private void FixedUpdate()
    {
        CheckForFootSteps();
    }

    private void CheckForFootSteps()
    {
        if (character == null)
        {
            return;
        }

        if (!character.characterNetworkManager.isMoving.Value)
        {
            return;
        }

        RaycastHit hit;

        if (Physics.Raycast(transform.position, character.transform.TransformDirection(Vector3.down), out hit, distanceToGround, WorldUtilityManager.instance.GetEnvironmentLayers()))
        {
            hasTouchedGround = true;

            if (!hasPlayedFootStepSFX)
            {
                steppedOnObject = hit.transform.gameObject;
            }
            else
            {
                hasTouchedGround = false;
                hasPlayedFootStepSFX = false;
                steppedOnObject = null;
            }

            if (hasTouchedGround && !hasPlayedFootStepSFX)
            {
                hasPlayedFootStepSFX = true;
                PlayFootStepSFX();
            }
        }
    }

    private void PlayFootStepSFX()
    {
        // Method 1 (simple, no tags, footsteps are the same for every material stepped on)
        character.characterSoundFXManager.PlayFootStep();

        // Method 2 (add tags for every material)
        // audioSource.PlayOneShot(WorldSoundFXManager.instance.ChooseRandomFootStepSoundBaseOnGround(steppedOnObject, character));
    }
}
