using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterLocomotionManager : CharacterLocomotionManager
{
    public void RotateTowardsAgent(AICharacterManager aiCharacter)
    {
        if (aiCharacter.aiCharacterNetworkManager.isMoving.Value)
        {
            aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;

            /*
            Vector3 targetDirection = aiCharacter.aiCharacterCombatManager.currentTarget.transform.position - aiCharacter.transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();

            if (targetDirection == Vector3.zero)
            {
                targetDirection = aiCharacter.transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, 10 * Time.deltaTime);
            */
        }
    }
}
