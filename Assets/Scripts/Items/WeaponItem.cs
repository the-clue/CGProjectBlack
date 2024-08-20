using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponItem : Item
{
    [Header("Animations")]
    public AnimatorOverrideController weaponAnimator;

    [Header("Weapon Model")]
    public GameObject weaponModel;

    [Header("Weapon Base Damage")]
    public int physicalDamage = 0;
    public int magicalDamage = 0;

    [Header("Weapon Poise Damage")]
    public float poiseDamage = 0;

    [Header("Attack Modifiers")]
    public float light_Attack_01_Modifier = 0.9f;
    public float light_Attack_02_Modifier = 1f;
    public float light_Attack_03_Modifier = 1.1f;
    public float running_Attack_01_Modifier = 1.2f;
    public float dodge_Attack_01_Modifier = 1.2f;
    public float duck_Attack_01_Modifier = 1.1f;
    public float jump_Attack_01_Modifier = 0.9f;
    public float heavy_Attack_01_Modifier = 1.5f;
    public float heavy_Charged_Attack_01_Modifier = 2.5f;
    public float heavy_Running_Attack_01_Modifier = 1.5f;
    public float heavy_Jump_Attack_01_Modifier = 2f;

    [Header("Stamina Costs")]
    public int baseStaminaCost = 20;
    public float lightAttackStaminaCostMultiplier = 1f;
    public float runningAttackStaminaCostMultiplier = 1.5f;
    public float dodgeAttackStaminaCostMultiplier = 1.2f;
    public float duckAttackStaminaCostMultiplier = 0.7f;
    public float jumpAttackStaminaCostMultiplier = 0.9f;
    public float heavyAttackStaminaCostMultiplier = 1.5f;
    public float heavyChargedAttackStaminaCostMultiplier = 2.5f;
    public float heavyRunningAttackStaminaCostMultiplier = 1.5f;
    public float heavyJumpAttackStaminaCostMultiplier = 2f;

    [Header("Actions")]
    public WeaponItemAction rightHandAction;
    public WeaponItemAction rightHandHeavyAction;

    [Header("Sounds")]
    public AudioClip[] swingSounds;
}
