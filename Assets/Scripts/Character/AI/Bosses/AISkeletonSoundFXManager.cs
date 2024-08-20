using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISkeletonSoundFXManager : CharacterSoundFXManager
{
    [Header("Skill Sounds")]
    public AudioClip[] auraSounds; // played when opening Aura Damage Colliders
    public AudioClip[] impactSounds; // played from the jump attack
    public AudioClip[] roarSounds; // played with phase shift

    public virtual void PlayImpactSFX()
    {
        if (impactSounds.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(impactSounds));
        }
    }

    public virtual void PlayRoarSFX()
    {
        if (roarSounds.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(roarSounds));
        }
    }
}
