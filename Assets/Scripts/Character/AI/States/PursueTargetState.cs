using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI/States/Pursue Target")]
public class PursueTargetState : AIState
{
    [SerializeField] bool pursueToStoppingDistance = false;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)
            return this;

        if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
            return SwitchState(aiCharacter, aiCharacter.idleState);

        if (!aiCharacter.navMeshAgent.enabled)
            aiCharacter.navMeshAgent.enabled = true;

        if (aiCharacter.aiCharacterCombatManager.enablePivot)
        {
            // If target goes outside FOV
            if (aiCharacter.aiCharacterCombatManager.viewableAngle < aiCharacter.aiCharacterCombatManager.minimumFOV ||
                aiCharacter.aiCharacterCombatManager.viewableAngle > aiCharacter.aiCharacterCombatManager.maximumFOV)
            {
                aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
            }
        }

        aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

        if (!pursueToStoppingDistance)
        {
            // option 1
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.combatStanceState.maximumEngagementDistance)
            {
                return SwitchState(aiCharacter, aiCharacter.combatStanceState);
            }
        }
        else
        {
            // option 2
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
            {
                return SwitchState(aiCharacter, aiCharacter.combatStanceState);
            }
        }

        // PURSUE TARGET
        // option 1 - more efficient, less effective?
        // aiCharacter.navMeshAgent.SetDestination(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position);

        // option 2
        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);

        return this;
    }
}
