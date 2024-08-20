using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MeleeWeaponDamageCollider : DamageCollider
{
    [Header("Attacking Character")]
    public CharacterManager characterCausingDamage;

    [Header("Weapon Trail")] // Added for VFX, maybe make it default?
    public GameObject weaponTrailVFX;

    [Header("Weapon Attack Modifiers")]
    public float light_Attack_01_Modifier;
    public float light_Attack_02_Modifier;
    public float light_Attack_03_Modifier;
    public float running_Attack_01_Modifier;
    public float dodge_Attack_01_Modifier;
    public float duck_Attack_01_Modifier;
    public float jump_Attack_01_Modifier;
    public float heavy_Attack_01_Modifier;
    public float heavy_Charged_Attack_01_Modifier;
    public float heavy_Running_Attack_01_Modifier;
    public float heavy_Jump_Attack_01_Modifier;

    protected override void Awake()
    {
        base.Awake();

        if (damageCollider == null)
        {
            damageCollider = GetComponent<Collider>();
        }

        damageCollider.enabled = false; // melee weapon colliders should be disabled at start, only enabled when animation allows

        if (weaponTrailVFX != null)
        {
            weaponTrailVFX.SetActive(false);
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

        if (damageTarget != null)
        {
            // to not damage ourself
            if (damageTarget == characterCausingDamage)
            {
                return;
            }

            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            DamageTarget(damageTarget);
        }
    }

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        if (charactersDamaged.Contains(damageTarget))
        {
            return;
        }

        charactersDamaged.Add(damageTarget);

        TakeHealthDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.contactPoint = contactPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);

        switch (characterCausingDamage.characterCombatManager.currentAttackType)
        {
            case AttackType.LightAttack01:
                ApplyAttackDamageModifiers(light_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.LightAttack02:
                ApplyAttackDamageModifiers(light_Attack_02_Modifier, damageEffect);
                break;
            case AttackType.LightAttack03:
                ApplyAttackDamageModifiers(light_Attack_03_Modifier, damageEffect);
                break;
            case AttackType.HeavyAttack01:
                ApplyAttackDamageModifiers(heavy_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.HeavyChargedAttack01:
                ApplyAttackDamageModifiers(heavy_Charged_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.RunningAttack01:
                ApplyAttackDamageModifiers(running_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.DodgeAttack01:
                ApplyAttackDamageModifiers(dodge_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.DuckAttack01:
                ApplyAttackDamageModifiers(duck_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.JumpAttack01:
                ApplyAttackDamageModifiers(jump_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.HeavyRunningAttack01:
                ApplyAttackDamageModifiers(heavy_Running_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.HeavyJumpAttack01:
                ApplyAttackDamageModifiers(heavy_Jump_Attack_01_Modifier, damageEffect);
                break;
            default:
                break;
        }

        if (characterCausingDamage.IsOwner)
        {
            damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(damageTarget.NetworkObjectId,
                characterCausingDamage.NetworkObjectId, damageEffect.physicalDamage, damageEffect.magicDamage, damageEffect.poiseDamage,
                damageEffect.angleHitFrom, damageEffect.contactPoint.x, damageEffect.contactPoint.y, damageEffect.contactPoint.z);
        }

        // damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);

    }

    private void ApplyAttackDamageModifiers(float modifier, TakeHealthDamageEffect damage)
    {
        damage.physicalDamage *= modifier;
        damage.magicDamage *= modifier;

        // if attack is fully charged, multiply with full modifier
    }

    public override void EnableDamageCollider()
    {
        base.EnableDamageCollider();
        if (weaponTrailVFX != null)
        {
            weaponTrailVFX.SetActive(true);
        }
    }
    public override void DisableDamageCollider()
    {
        base.DisableDamageCollider();
        if (weaponTrailVFX != null)
        {
            weaponTrailVFX.SetActive(false);
        }
    }
}
