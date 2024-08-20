using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/States/Idle")]
public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.characterCombatManager.currentTarget != null)
        {
            return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
        }
        else
        {
            aiCharacter.aiCharacterCombatManager.FindTargetViaLineOfSight(aiCharacter);
            return this;
        }
    }
}
