using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Health Damage")]
public class TakeHealthDamageEffect : InstantCharacterEffect
{
    [Header("Character causing Damage")]
    public CharacterManager characterCausingDamage;

    [Header("Damage")]
    public float physicalDamage = 0;
    public float magicDamage = 0;
    public int finalDamageDealt = 0; // damage after all calculations

    [Header("Poise")]
    public float poiseDamage = 0;
    public bool poiseIsBroken = false;

    [Header("Animation")]
    public bool playDamageAnimation = true;
    public bool manuallySelectDamageAnimation = false;
    public string damageAnimation;

    [Header("Sound FX")]
    public bool willPlayDamageFX = true;
    public AudioClip elementalDamageSoundFX;

    [Header("Direction damage is taken from")]
    public float angleHitFrom;
    public Vector3 contactPoint;

    public override void ProcessEffect(CharacterManager character)
    {
        if (character.characterNetworkManager.isInvulnerable.Value)
        {
            return;
        }

        base.ProcessEffect(character);

        if (character.isDead.Value)
        {
            return;
        }

        CalculateDamage(character);
        PlayDirectionalBasedDamageAnimation(character);
        PlayDamageVFX(character);
        PlayDamageSFX(character);
    }

    private void CalculateDamage(CharacterManager character)
    {
        if (!character.IsOwner)
        {
            return;
        }

        if (characterCausingDamage != null)
        {

        }

        finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage);

        if (finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }

        Debug.Log("Final damage dealt: " + finalDamageDealt);
        character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;

        character.characterStatsManager.totalPoiseDamage -= poiseDamage;

        float remainingPoise = character.characterStatsManager.basePoiseDefense
            // + character.characterStatsManager.offensivePoiseBonus 
            + character.characterStatsManager.totalPoiseDamage;

        if (remainingPoise <= 0)
        {
            poiseIsBroken = true;
        }

        character.characterStatsManager.poiseResetTimer = character.characterStatsManager.defaultPoiseResetTimer;
    }

    private void PlayDamageVFX(CharacterManager character)
    {
        character.characterEffectsManager.PlayBloodSplashVFX(contactPoint);
    }

    private void PlayDamageSFX(CharacterManager character)
    {
        AudioClip physicalDamageSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.physicalDamageSFX);

        character.characterSoundFXManager.PlaySoundFX(physicalDamageSFX);
        character.characterSoundFXManager.PlayDamageGrunt();
    }

    private void PlayDirectionalBasedDamageAnimation(CharacterManager character)
    {
        if (!character.IsOwner)
        {
            return;
        }

        if (character.isDead.Value)
        {
            return;
        }

        if (poiseIsBroken && !character.characterNetworkManager.isPoiseInvulnerable.Value) // staggering animations
        {
            if (angleHitFrom >= 145 && angleHitFrom <= 180) // front animation
            {
                damageAnimation = character.characterAnimatorManager.hit_Forward_Medium_01;
            }
            else if (angleHitFrom <= -145 && angleHitFrom >= -180) // front animation
            {
                damageAnimation = character.characterAnimatorManager.hit_Forward_Medium_01;
            }
            else if (angleHitFrom >= -45 && angleHitFrom <= 45) // back animation
            {
                damageAnimation = character.characterAnimatorManager.hit_Backward_Medium_01;
            }
            else if (angleHitFrom >= -144 && angleHitFrom <= -45) // left animation
            {
                damageAnimation = character.characterAnimatorManager.hit_Left_Medium_01;
            }
            else if (angleHitFrom >= 45 && angleHitFrom <= 144) // right animation
            {
                damageAnimation = character.characterAnimatorManager.hit_Right_Medium_01;
            }
        }
        else // flinching animations (which are just masked staggering animations in head and chest)
        {
            if (angleHitFrom >= 145 && angleHitFrom <= 180) // front animation
            {
                damageAnimation = character.characterAnimatorManager.hit_Forward_Ping_01;
            }
            else if (angleHitFrom <= -145 && angleHitFrom >= -180) // front animation
            {
                damageAnimation = character.characterAnimatorManager.hit_Forward_Ping_01;
            }
            else if (angleHitFrom >= -45 && angleHitFrom <= 45) // back animation
            {
                damageAnimation = character.characterAnimatorManager.hit_Backward_Ping_01;
            }
            else if (angleHitFrom >= -144 && angleHitFrom <= -45) // left animation
            {
                damageAnimation = character.characterAnimatorManager.hit_Left_Ping_01;
            }
            else if (angleHitFrom >= 45 && angleHitFrom <= 144) // right animation
            {
                damageAnimation = character.characterAnimatorManager.hit_Right_Ping_01;
            }
        }

        if (poiseIsBroken) // then you are stunned
        {
            character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
        }
        else // you flinch without getting stunned
        {
            // this code resets isPerforming to false, making AIs able to perform more animations contemporarily
            // character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, false, false, true, true);
            character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation,
                character.isPerformingAction,
                false,
                character.characterLocomotionManager.canRotate,
                character.characterLocomotionManager.canMove);
        }
    }
}
