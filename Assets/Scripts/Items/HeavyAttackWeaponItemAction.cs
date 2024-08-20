using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
public class HeavyAttackWeaponItemAction : WeaponItemAction
{
    [SerializeField] string heavy_attack_01 = "Main_Heavy_Attack_01";
    [SerializeField] string heavy_run_attack_01 = "Main_Heavy_Run_Attack_01";
    [SerializeField] string heavy_jump_attack_01 = "Main_Heavy_Jump_Attack_01";

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
            if (playerPerformingAction.characterCombatManager.canDoJumpAttack) // If jumping
            {
                if (playerPerformingAction.characterCombatManager.isDoingJumpattack)
                {
                    return;
                }
                if (playerPerformingAction.animator.GetBool("IsHeavyAttacking"))
                {
                    return;
                }
                PerformHeavyJumpAttack(playerPerformingAction, weaponPerformingAction);
                return;
            }

            return;
        }

        if (playerPerformingAction.characterNetworkManager.isSprinting.Value) // If running
        {
            if (playerPerformingAction.isPerformingAction)
            {
                return;
            }
            PerformHeavyRunningAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }

        PerformHeavyAttack(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        if (!playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack01, heavy_attack_01, true);
        }

        /*
        // If you want to give Heavy Attack Combos, use this
        if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

            if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == heavy_attack_01)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack01, heavy_attack_01, true);
            }
            else
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack01, heavy_attack_01, true);

            }
        }
        else if (!playerPerformingAction.isPerformingAction) // Else, if we are not performing another action, do regular attack
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack01, heavy_attack_01, true);
        }
        */
    }

    private void PerformHeavyRunningAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyRunningAttack01, heavy_run_attack_01, true);
    }

    private void PerformHeavyJumpAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        playerPerformingAction.playerCombatManager.canDoJumpAttack = false;
        // playerPerformingAction.playerCombatManager.isDoingJumpattack = true;
        playerPerformingAction.animator.SetBool("IsHeavyAttacking", true);
        // playerPerformingAction.playerLocomotionManager.moveAmount = 0;
        playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyJumpAttack01, heavy_jump_attack_01, true);
    }
}
