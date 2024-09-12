using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AIDeathGodCombatManager : AICharacterCombatManager
{
    AIDeathGodCharacterManager aiDeathGodManager;

    [Header("Damage Colliders and Spawn Points")]
    [SerializeField] ImpactDamageCollider impactDamageCollider;
    [SerializeField] Transform rangeMagicSpawnPoint;
    [SerializeField] Transform flyRangeMagicSpawnPoint;
    [SerializeField] Transform phaseShiftMagicSpawnPoint;

    [Header("Damage")]
    [SerializeField] int baseDamage = 100;
    [SerializeField] int basePoiseDamage = 50;
    [SerializeField] float attack01DamageModifier = 1.0f; // Fly then Smash
    [SerializeField] float attack02DamageModifier = 1.25f; // Smash for Three
    [SerializeField] float attack03DamageModifier = 1.5f; // Fire for Three
    [SerializeField] float attack04DamageModifier = 2.0f; // Fly then Fire
    [SerializeField] float attack05DamageModifier = 2.25f; // Charge Slash
    [SerializeField] float attackPSDamageModifier = 4.0f; // Phase Shift Aura

    /*
    [SerializeField] float attack06DamageModifier = 2.5f;
    */

    [Header("Attack Parameters")]
    [SerializeField] float attack01ImpactSize = 5.6f;
    [SerializeField] float attack01ImpactHeight = 0.2f;
    [SerializeField] float attack02ImpactSize = 2.5f;
    [SerializeField] float attack02ImpactHeight = 0.2f;
    [SerializeField] float attack03RangeMagicSpeed = 5f;
    [SerializeField] float attack03RangeMagicAngleSpeed = 5f;
    [SerializeField] int attack03RangeMagicMode = 1;
    [SerializeField] float attack04RangeMagicSpeed = 15f;
    [SerializeField] float attack04RangeMagicAngleSpeed = 2f;
    [SerializeField] int attack04RangeMagicMode = 1;
    [SerializeField] float attack05RangeMagicSpeed = 15f;
    [SerializeField] float attack05RangeMagicAngleSpeed = 2f;
    [SerializeField] int attack05RangeMagicMode = 1;
    [SerializeField] float attackPSRangeMagicSpeed = 7.5f;
    [SerializeField] float attackPSRangeMagicAngleSpeed = 7.5f;
    [SerializeField] int attackPSRangeMagicMode = 1;
    [SerializeField] float attackPSRangeMagicDelay = 10;
    [SerializeField] float attackPSRangeMagicHomingDuration = 10;

    [Header("FXs")]
    [SerializeField] GameObject weapon;
    [SerializeField] GameObject attack01VFX;
    [SerializeField] AudioClip attack01SFX;
    [SerializeField] GameObject attack02VFX;
    [SerializeField] AudioClip attack02SFX;
    [SerializeField] GameObject attack03VFX;
    [SerializeField] AudioClip attack03SFX;
    [SerializeField] GameObject attack04VFX;
    [SerializeField] AudioClip attack04SFX;
    [SerializeField] GameObject attack05VFX;
    [SerializeField] AudioClip attack05SFX;
    [SerializeField] GameObject attackPSVFX;
    [SerializeField] AudioClip attackPSSFX;
    [SerializeField] GameObject teleportVFX;

    protected override void Awake()
    {
        base.Awake();

        aiDeathGodManager = GetComponent<AIDeathGodCharacterManager>();
        impactDamageCollider.GetComponent<ImpactDamageCollider>();
    }

    public void SetAttack01()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        float impactDamage = baseDamage * attack01DamageModifier;
        float impactPoiseDamage = basePoiseDamage * attack01DamageModifier;
        impactDamageCollider.SetImpactDamageCollider(aiDeathGodManager, impactDamage, impactPoiseDamage,
            attack01VFX, attack01SFX, attack01ImpactSize, attack01ImpactHeight, 53.5f);
    }

    public void SetAttack02()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        float impactDamage = baseDamage * attack02DamageModifier;
        float impactPoiseDamage = basePoiseDamage * attack02DamageModifier;
        impactDamageCollider.SetImpactDamageCollider(aiDeathGodManager, impactDamage, impactPoiseDamage,
            attack02VFX, attack02SFX, attack02ImpactSize, attack02ImpactHeight, 53.5f);
    }
    public void SetAttack03()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
    }

    public void CastAttack03()
    {
        float magicDamage = baseDamage * attack03DamageModifier;
        float magicPoiseDamage = basePoiseDamage * attack03DamageModifier;
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        eulerRotation.x = 0; // Set the x axis rotation to 0
        GameObject rangeMagicGameObject = Instantiate(attack03VFX, rangeMagicSpawnPoint.position, Quaternion.Euler(eulerRotation));
        RangeMagicDamageCollider rangeMagicDamageCollider = rangeMagicGameObject.GetComponent<RangeMagicDamageCollider>();
        rangeMagicDamageCollider.SetRangeMagicDamageCollider(aiDeathGodManager, currentTarget, magicDamage, magicPoiseDamage,
            attack03RangeMagicSpeed, attack03RangeMagicAngleSpeed, attack03RangeMagicMode, 0, 5, false);
        aiDeathGodManager.deathGodSoundFXManager.PlaySoundFX(attack03SFX);
    }

    public void SetAttack04()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
    }

    public void CastAttack04()
    {
        float magicDamage = baseDamage * attack04DamageModifier;
        float magicPoiseDamage = basePoiseDamage * attack04DamageModifier;
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        eulerRotation.x = 20;
        GameObject rangeMagicGameObject = Instantiate(attack04VFX, flyRangeMagicSpawnPoint.position, Quaternion.Euler(eulerRotation));
        RangeMagicDamageCollider rangeMagicDamageCollider = rangeMagicGameObject.GetComponent<RangeMagicDamageCollider>();
        rangeMagicDamageCollider.SetRangeMagicDamageCollider(aiDeathGodManager, currentTarget, magicDamage, magicPoiseDamage,
            attack04RangeMagicSpeed, attack04RangeMagicAngleSpeed, attack04RangeMagicMode, 0, attack04RangeMagicAngleSpeed, false);
        aiDeathGodManager.deathGodSoundFXManager.PlaySoundFX(attack04SFX);
    }

    public void SetAttack05()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
    }

    public void CastAttack05()
    {
        float magicDamage = baseDamage * attack05DamageModifier;
        float magicPoiseDamage = basePoiseDamage * attack05DamageModifier;
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        eulerRotation.x = 0;
        GameObject rangeMagicGameObject = Instantiate(attack05VFX, rangeMagicSpawnPoint.position, Quaternion.Euler(eulerRotation));
        RangeMagicDamageCollider rangeMagicDamageCollider = rangeMagicGameObject.GetComponent<RangeMagicDamageCollider>();
        rangeMagicDamageCollider.SetRangeMagicDamageCollider(aiDeathGodManager, currentTarget, magicDamage, magicPoiseDamage,
            attack05RangeMagicSpeed, attack05RangeMagicAngleSpeed, attack05RangeMagicMode, 0, 5, false);
        aiDeathGodManager.deathGodSoundFXManager.PlaySoundFX(attack05SFX);
    }

    public void CastAttack06()
    {
        float magicDamage = baseDamage * attack04DamageModifier;
        float magicPoiseDamage = basePoiseDamage * attack04DamageModifier;
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        eulerRotation.x = 20;
        Vector3 newPosition = new Vector3(flyRangeMagicSpawnPoint.position.x, flyRangeMagicSpawnPoint.position.y + 1, flyRangeMagicSpawnPoint.position.z);
        GameObject rangeMagicGameObject = Instantiate(attack04VFX, newPosition, Quaternion.Euler(eulerRotation));
        RangeMagicDamageCollider rangeMagicDamageCollider = rangeMagicGameObject.GetComponent<RangeMagicDamageCollider>();
        rangeMagicDamageCollider.SetRangeMagicDamageCollider(aiDeathGodManager, currentTarget, magicDamage, magicPoiseDamage,
            attack04RangeMagicSpeed, attack04RangeMagicAngleSpeed, attack04RangeMagicMode, 0, attack04RangeMagicAngleSpeed, false);
        aiDeathGodManager.deathGodSoundFXManager.PlaySoundFX(attack04SFX);
    }

    public void CastAttack09()
    {
        float magicDamage = baseDamage * attack04DamageModifier;
        float magicPoiseDamage = basePoiseDamage * attack04DamageModifier;
        GameObject rangeMagicGameObject = Instantiate(attack04VFX, flyRangeMagicSpawnPoint.position, flyRangeMagicSpawnPoint.rotation);
        RangeMagicDamageCollider rangeMagicDamageCollider = rangeMagicGameObject.GetComponent<RangeMagicDamageCollider>();
        rangeMagicDamageCollider.SetRangeMagicDamageCollider(aiDeathGodManager, currentTarget, magicDamage, magicPoiseDamage,
            attack04RangeMagicSpeed, attack04RangeMagicAngleSpeed, attack04RangeMagicMode, 0, attack04RangeMagicAngleSpeed, false);
        aiDeathGodManager.deathGodSoundFXManager.PlaySoundFX(attack04SFX);
    }

    public void CastPhaseShiftAttack()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();

        float magicDamage = baseDamage * attackPSDamageModifier;
        float magicPoiseDamage = basePoiseDamage * attackPSDamageModifier;
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        eulerRotation.x = 0; // Set the x axis rotation to 0
        GameObject rangeMagicGameObject = Instantiate(attackPSVFX, phaseShiftMagicSpawnPoint.position, Quaternion.Euler(eulerRotation));
        RangeMagicDamageCollider rangeMagicDamageCollider = rangeMagicGameObject.GetComponent<RangeMagicDamageCollider>();
        rangeMagicDamageCollider.SetRangeMagicDamageCollider(aiDeathGodManager, currentTarget, magicDamage, magicPoiseDamage,
            attackPSRangeMagicSpeed, attackPSRangeMagicAngleSpeed, attackPSRangeMagicMode, attackPSRangeMagicDelay, attackPSRangeMagicHomingDuration, true);
        aiDeathGodManager.deathGodSoundFXManager.PlaySoundFX(attackPSSFX);
    }

    public void ActivateImpactAttack()
    {
        impactDamageCollider.ActivateImpact();
    }

    public void Teleport()
    {
        if (distanceFromTarget > 3f)
        {
            _ = Instantiate(teleportVFX, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.LookRotation(transform.up));
            // _ = Instantiate(teleportVFX, aiDeathGodManager.transform);

            Vector3 targetPosition = currentTarget.transform.position;
            Vector3 directionToTarget = (aiDeathGodManager.transform.position - targetPosition).normalized;
            targetPosition += directionToTarget * 1f;
            targetPosition.y = targetPosition.y + 1.5f;

            aiDeathGodManager.transform.position = targetPosition;

            _ = Instantiate(teleportVFX, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.LookRotation(transform.up));
        }
    }

    public void MakeWeaponAppear()
    {
        weapon.SetActive(true);
    }

    public override void PivotTowardsTarget(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)
        {
            return;
        }

        if (viewableAngle >= 75 && viewableAngle <= 105)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_90", true, true, true);
        }
        else if (viewableAngle >= 150 && viewableAngle <= 180)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_180", true, true, true);
        }
        else if (viewableAngle <= -75 && viewableAngle >= -105)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_90", true, true, true);
        }
        else if (viewableAngle <= -150 && viewableAngle >= -180)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_180", true, true, true);
        }
    }
}