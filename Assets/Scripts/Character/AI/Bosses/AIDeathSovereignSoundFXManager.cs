using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDeathSovereignSoundFXManager : CharacterSoundFXManager
{
    [Header("Skill Sounds")]
    public AudioClip[] weakMagicSounds;
    public AudioClip[] mediumMagicSounds;
    public AudioClip[] strongMagicSounds;

    public virtual void PlayWeakMagicSFX()
    {
        if (weakMagicSounds.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(weakMagicSounds));
        }
    }

    public virtual void PlayMediumMagicSFX()
    {
        if (mediumMagicSounds.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(mediumMagicSounds));
        }
    }

    public virtual void PlayStrongMagicSFX()
    {
        if (strongMagicSounds.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(strongMagicSounds));
        }
    }
}