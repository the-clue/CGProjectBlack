using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI/States/Combat Stance")]
public class CombatStanceState : AIState
{
    [Header("Attacks")]
    public List<AICharacterAttackAction> aiCharacterAttacks; // all attacks
    public List<AICharacterAttackAction> potentialAttacks; // all currently possible attacks
    public AICharacterAttackAction chosenAttack;
    public AICharacterAttackAction previousAttack;
    protected bool hasAttack = false;

    [Header("Combo")]
    [SerializeField] protected bool canPerformCombo = false; // If the character can perform a combo attack, after the first attack
    [SerializeField] protected int chanceToPerformCombo = 25; // Chance in percent to perform a combo

    // bool hasRolledForComboChance = false; // If the character has rolled for the chance during this state

    [Header("Engagement Distance")]
    [SerializeField] public float maximumEngagementDistance = 5; // Maximum distance before going to pursue state

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)
        {
            return this;
        }

        if (!aiCharacter.navMeshAgent.enabled)
        {
            aiCharacter.navMeshAgent.enabled = true;
        }

        if (aiCharacter.aiCharacterCombatManager.enablePivot)
        {
            // If you want the AI character to face and turn towards it's target when outside it's fov
            if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                if (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 || aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
                {
                    aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
                }
            }
        }

        aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

        if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
        {
            return SwitchState(aiCharacter, aiCharacter.idleState);
        }

        if (!hasAttack)
        {
            GetNewAttack(aiCharacter);
        }
        else
        {
            aiCharacter.attackState.currentAttack = chosenAttack;

            // Roll for combo chance
            if (canPerformCombo)
            {
                aiCharacter.attackState.willPerformCombo = RollForOutcomeChance(chanceToPerformCombo);
            }

            return SwitchState(aiCharacter, aiCharacter.attackState);
        }

        if (aiCharacter.aiCharacterCombatManager.distanceFromTarget > maximumEngagementDistance)
        {
            return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
        }

        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);

        return this;
    }

    protected virtual void GetNewAttack(AICharacterManager aiCharacter)
    {
        potentialAttacks = new List<AICharacterAttackAction>();

        foreach (var potentialAttack in aiCharacterAttacks)
        {
            if (potentialAttack == previousAttack) 
            {
                continue; // this makes repeating attacks impossible

                // Uncomment this to make repeating attacks still possible, but less probable
                // var randomInt = Random.Range(1, 4); // Gives a random number between 1, 2 and 3
                // if (randomInt == 1) // There is a 33% chance that the previous attack becomes a potential attack
                // {
                //     continue;
                // }
            }

            // Too close
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget < potentialAttack.minimumAttackDistance)
            {
                continue;
            } // Too far
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget > potentialAttack.maximumAttackDistance)
            {
                continue;
            }

            // Outside minimum field of view
            if (aiCharacter.aiCharacterCombatManager.viewableAngle < potentialAttack.minimumAttackAngle)
            {
                continue;
            } // Outside maximum field of view
            if (aiCharacter.aiCharacterCombatManager.viewableAngle > potentialAttack.maximumAttackAngle)
            {
                continue;
            }

            potentialAttacks.Add(potentialAttack);
        }

        if (potentialAttacks.Count <= 0)
        {
            Debug.Log("No potential attacks");

            if (aiCharacter.aiCharacterCombatManager.enablePivot)
            {
                // If you want the AI character to face and turn towards it's target when outside it's fov
                if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value)
                {
                    if (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 || aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
                    {
                        aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
                    }
                }
            }

            aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

            return;
        }

        var totalWeight = 0;

        foreach (var attack in potentialAttacks)
        {
            totalWeight += attack.attackWeight;
        }

        var randomWeightValue = Random.Range(1, totalWeight + 1);
        var processedWeight = 0;

        foreach (var attack in potentialAttacks)
        {
            processedWeight += attack.attackWeight;

            if (randomWeightValue <= processedWeight)
            {
                chosenAttack = attack;
                previousAttack = chosenAttack;
                hasAttack = true;
                return;
            }
        }
    }

    protected virtual bool RollForOutcomeChance(int outcomeChance)
    {
        bool outcomeWillBePerformed = false;

        int randomPercentage = Random.Range(0, 100);

        if (randomPercentage < outcomeChance)
        {
            outcomeWillBePerformed = true;
        }

        return outcomeWillBePerformed;
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);

        hasAttack = false;
        // hasRolledForComboChance = false;
    }
}
