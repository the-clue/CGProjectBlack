using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundFXManager : MonoBehaviour
{
    private CharacterManager character;
    private AudioSource audioSource;

    [Header("Sounds")]
    [SerializeField] protected AudioClip[] damageGrunts;
    [SerializeField] protected AudioClip[] attackGrunts;
    [SerializeField] protected AudioClip[] footSteps;
    [SerializeField] protected AudioClip[] whooshes;

    protected void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        character = GetComponent<CharacterManager>();
    }

    public void PlaySoundFX(AudioClip soundFX, float volume = 1, bool randomizePitch = true, float pitchRandom = 0.1f)
    {
        audioSource.PlayOneShot(soundFX, volume);
        // resets pitch
        audioSource.pitch = 1;

        // randomizes pitch by pitchRandom percent
        if (randomizePitch)
        {
            audioSource.pitch += Random.Range(-pitchRandom, pitchRandom);
        }
    }

    public void PlayRollSoundFX()
    {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.rollSFX);
    }

    public void PlayDamageGrunt()
    {
        if (damageGrunts.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(damageGrunts));
        }
    }

    public void PlayAttackGrunt()
    {
        if (attackGrunts.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(attackGrunts));
        }
    }

    public virtual void PlayFootStep()
    {
        if (!character.characterNetworkManager.isMoving.Value)
        {
            return;
        }

        if (footSteps.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(footSteps), 1f, true, 0.2f);
        }
    }
    public virtual void PlayWhooshes()
    {
        if (whooshes.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(whooshes));
        }
    }
}
