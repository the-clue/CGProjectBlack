using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/States/Attack")]
public class AttackState : AIState
{
    [Header("Current Attack")]
    [HideInInspector] public AICharacterAttackAction currentAttack;
    [HideInInspector] public bool willPerformCombo = false;

    [Header("State Flags")]
    protected bool hasPerformedAttack = false;
    // protected bool hasPerformedCombo = false;

    [Header("Pivot After Attack")]
    [SerializeField] protected bool pivotAfterAttack = false;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
        {
            return SwitchState(aiCharacter, aiCharacter.idleState);
        }

        if (aiCharacter.aiCharacterCombatManager.currentTarget.isDead.Value)
        {
            return SwitchState(aiCharacter, aiCharacter.idleState);
        }

        // Rotate towards target while attacking
        aiCharacter.aiCharacterCombatManager.RotateTowardsTargetWhileAttacking(aiCharacter);

        // aiCharacter.characterNetworkManager.isMoving.Value = false;
        // aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

        // Set movement values to 0
        aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 0, false);
        // the values are being set to 0, but the character still moves for some reason!
        // aiCharacter.characterNetworkManager.isMoving.Value = false; // this doesn't work

        // Perform combo
        // if (willPerformCombo && !hasPerformedCombo)
        if (willPerformCombo)
        {
            if (currentAttack.comboAction != null)
            {
                // If we can combo
                // hasPerformedCombo = true;
                // currentAttack.comboAction.AttemptToPerformAction(aiCharacter);
            }
        }

        if (aiCharacter.isPerformingAction)
        {
            return this;
        }

        if (!hasPerformedAttack)
        {
            // If recovering, wait before another attack
            if (aiCharacter.aiCharacterCombatManager.actionRecoveryTime > 0)
            {
                // if (pivotAfterAttack)
                // {
                //     aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
                // }
                return this;
            }

            PerformAttack(aiCharacter);

            // Return to the top, so if we have a combo, we process that when we are able
            return this;
        }

        if (aiCharacter.aiCharacterCombatManager.enablePivot && pivotAfterAttack)
        {
            aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
        }

        return SwitchState(aiCharacter, aiCharacter.combatStanceState);
    }

    protected void PerformAttack(AICharacterManager aiCharacter)
    {
        hasPerformedAttack = true;
        currentAttack.AttemptToPerformAction(aiCharacter);
        aiCharacter.aiCharacterCombatManager.actionRecoveryTime = currentAttack.actionRecoveryTime;
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);

        hasPerformedAttack = false;
        // hasPerformedCombo = false;
        willPerformCombo = false;
    }
}
