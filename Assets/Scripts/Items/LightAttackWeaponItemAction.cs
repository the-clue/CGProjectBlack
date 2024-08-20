using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
public class LightAttackWeaponItemAction : WeaponItemAction
{
    [SerializeField] string light_attack_01 = "Main_Light_Attack_01";
    [SerializeField] string light_attack_02 = "Main_Light_Attack_02";
    [SerializeField] string light_attack_03 = "Main_Light_Attack_03";
    [SerializeField] string run_attack_01 = "Main_Run_Attack_01";
    [SerializeField] string dodge_attack_01 = "Main_Dodge_Attack_01";
    [SerializeField] string duck_attack_01 = "Main_Duck_Attack_01";
    [SerializeField] string jump_attack_01 = "Main_Jump_Attack_01";

    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

        if (!playerPerformingAction.IsOwner)
        {
            return;
        }

        if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0)
        {
            return;
        }

        if (!playerPerformingAction.characterLocomotionManager.isGrounded)
        {
            // If jumping
            if (playerPerformingAction.characterCombatManager.canDoJumpAttack)
            {
                if (playerPerformingAction.animator.GetBool("IsHeavyAttacking")) // rename to IsHeavyJumpAttacking?
                {
                    return;
                }
                PerformJumpAttack(playerPerformingAction, weaponPerformingAction);
            }
            return;
        }

        // If running
        if (playerPerformingAction.characterNetworkManager.isSprinting.Value)
        {
            if (playerPerformingAction.isPerformingAction)
            {
                return;
            }
            PerformRunningAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }

        // If dodging
        if (playerPerformingAction.characterCombatManager.canDoDodgeAttack)
        {
            PerformDodgeAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }

        // If ducking
        if (playerPerformingAction.characterCombatManager.canDoDuckAttack)
        {
            PerformDuckAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }

        PerformLightAttack(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        // If we are attacking and able to perform a combo
        if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

            if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == light_attack_02)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightAttack03, light_attack_03, true);
            }
            else if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == light_attack_01)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightAttack02, light_attack_02, true);
            }
            else
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightAttack01, light_attack_01, true);

            }
        }
        else if (!playerPerformingAction.isPerformingAction) // Else, if we are not performing another action, do regular attack
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightAttack01, light_attack_01, true);
        }
        

        /*
        if (playerPerformingAction.playerNetworkManager.isUsingRightHand.Value)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_attack_01, true);
        }
        else if (playerPerformingAction.playerNetworkManager.isUsingLeftHand.Value)
        {

        }
        */
    }

    private void PerformRunningAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.RunningAttack01, run_attack_01, true);
    }

    private void PerformDodgeAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        playerPerformingAction.playerCombatManager.canDoDodgeAttack = false;
        playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.DodgeAttack01, dodge_attack_01, true);
    }

    private void PerformDuckAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        playerPerformingAction.playerCombatManager.canDoDuckAttack = false;
        playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.DuckAttack01, duck_attack_01, true);
    }

    private void PerformJumpAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        playerPerformingAction.playerCombatManager.canDoJumpAttack = false;
        playerPerformingAction.playerCombatManager.isDoingJumpattack = true;
        // playerPerformingAction.playerLocomotionManager.moveAmount = 0;
        playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.JumpAttack01, jump_attack_01, true);
    }
}
