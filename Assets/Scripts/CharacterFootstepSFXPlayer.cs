using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterFootstepSFXPlayer : MonoBehaviour
{
    CharacterManager character;

    private float lastFootStep;

    private void Awake()
    {
        character = GetComponentInParent<CharacterManager>();
    }

    private void Update()
    {
        var footStep = character.animator.GetFloat("Footstep");

        if (Mathf.Abs(footStep) < 0.00001f) footStep = 0; // to solve a floating point error

        if (lastFootStep > 0 && footStep < 0 || lastFootStep < 0 && footStep > 0)
        {
            character.characterSoundFXManager.PlayFootStep();
        }

        lastFootStep = footStep;
    }
}
