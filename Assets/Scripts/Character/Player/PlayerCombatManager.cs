using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;

    public WeaponItem currentWeaponBeingUsed;

    [Header("Flags")]
    public bool canComboWithMainHandWeapon = false;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
    {
        if (player.IsOwner)
        {
            weaponAction.AttemptToPerformAction(player, weaponPerformingAction);

            player.playerNetworkManager.NotifyTheServerOfWeaponActionServerRpc(NetworkManager.Singleton.LocalClientId, weaponAction.actionID, weaponPerformingAction.itemID);
        }
    }

    public virtual void DrainStaminaBasedOnAttack()
    {
        Debug.Log("DrainStaminaBasedOnAttack called");

        if (!player.IsOwner)
        {
            Debug.Log("Player is not owner");
            return;
        }

        if (currentWeaponBeingUsed == null)
        {
            Debug.Log("Weapon used is null");
            return;
        }

        float staminaDeducted = 0;

        switch (currentAttackType)
        {
            case AttackType.LightAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                break;
            case AttackType.LightAttack02:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                break;
            case AttackType.LightAttack03:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                break;
            case AttackType.RunningAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.runningAttackStaminaCostMultiplier;
                break;
            case AttackType.DodgeAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.dodgeAttackStaminaCostMultiplier;
                break;
            case AttackType.DuckAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.duckAttackStaminaCostMultiplier;
                break;
            case AttackType.JumpAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.jumpAttackStaminaCostMultiplier;
                break;
            case AttackType.HeavyAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostMultiplier;
                break;
            case AttackType.HeavyChargedAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyChargedAttackStaminaCostMultiplier;
                break;
            case AttackType.HeavyRunningAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyRunningAttackStaminaCostMultiplier;
                break;
            case AttackType.HeavyJumpAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyJumpAttackStaminaCostMultiplier;
                break;
            default:
                break;
        }

        Debug.Log("Stamina deducted: " + staminaDeducted);
        player.playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
    }

    public override void SetTarget(CharacterManager newTarget)
    {
        base.SetTarget(newTarget);

        if (player.IsOwner)
        {
            PlayerCamera.instance.SetLockCameraHeight();
        }
    }

    // Can be put in PLayerAnimatorManager
    public override void EnableCanDoCombo()
    {
        if (player.playerNetworkManager.isUsingRightHand.Value)
        {
            canComboWithMainHandWeapon = true;
        }
    }
    public override void DisableCanDoCombo()
    {
        canComboWithMainHandWeapon = false;
    }
}
