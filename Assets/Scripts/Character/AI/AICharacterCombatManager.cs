using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterCombatManager : CharacterCombatManager
{
    protected AICharacterManager aiCharacter;

    [Header("Recovery Timer")]
    public float actionRecoveryTime = 0;

    [Header("Pivot")]
    public bool enablePivot = true;

    [Header("Target Information")]
    public float distanceFromTarget;
    public float viewableAngle;
    public Vector3 targetDirection;

    [Header("Detection")]
    [SerializeField] float detectionRadius = 15;
    public float minimumFOV = -35;
    public float maximumFOV = 35;

    [Header("Attack Rotation Speed")]
    public float attackRotationSpeed = 3;

    protected override void Awake()
    {
        base.Awake();

        aiCharacter = GetComponent<AICharacterManager>();
        lockOnTransform = GetComponentInChildren<LockOnTransform>().transform; 
    }

    public void FindTargetViaLineOfSight(AICharacterManager aiCharacter)
    {
        if (currentTarget != null)
        {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtilityManager.instance.GetCharacterLayers());

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

            if (targetCharacter == null)
            {
                continue;
            }
            if (targetCharacter == aiCharacter)
            {
                continue;
            }
            if (targetCharacter.isDead.Value)
            {
                continue;
            }
            if (WorldUtilityManager.instance.IsTargetAttackable(character.characterGroup, targetCharacter.characterGroup))
            {
                Vector3 targetDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                float angleOfTarget = Vector3.Angle(targetDirection, aiCharacter.transform.forward);

                if (angleOfTarget > minimumFOV && angleOfTarget < maximumFOV)
                {
                    if (Physics.Linecast(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position, WorldUtilityManager.instance.GetEnvironmentLayers()))
                    {
                        Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position);
                        Debug.Log("Sight blocked");
                    }
                    else
                    {
                        targetDirection = targetCharacter.transform.position - transform.position;
                        viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, targetDirection);
                        aiCharacter.characterCombatManager.SetTarget(targetCharacter);

                        // You can put enabling pivot here or in a (pursue) state
                        if (enablePivot)
                        {
                            PivotTowardsTarget(aiCharacter);
                        }
                    }
                }
            }
        }
    }

    public virtual void PivotTowardsTarget(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)
        {
            return;
        }

        if (viewableAngle >= 20 && viewableAngle <= 60)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_45", true);
        }
        else if (viewableAngle >= 61 && viewableAngle <= 110)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_90", true);
        }
        else if (viewableAngle >= 146 && viewableAngle <= 180)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_180", true);
        }
        else if (viewableAngle <= -20 && viewableAngle >= -60)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_45", true);
        }
        else if (viewableAngle <= -61 && viewableAngle >= -110)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_90", true);
        }
        else if (viewableAngle <= -146 && viewableAngle >= -180)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_180", true);
        }
    }

    public void RotateTowardsAgent(AICharacterManager aiCharacter)
    {
        if (aiCharacter.aiCharacterNetworkManager.isMoving.Value)
        {
            aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
        }
    }

    public void RotateTowardsTargetWhileAttacking(AICharacterManager aiCharacter)
    {
        if (currentTarget == null)
        {
            return;
        }

        if (!aiCharacter.characterLocomotionManager.canRotate)
        {
            return;
        }

        if (!aiCharacter.isPerformingAction)
        {
            return;
        }

        Vector3 targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
        targetDirection.y = 0;
        targetDirection.Normalize();

        if (targetDirection == Vector3.zero)
        {
            targetDirection = aiCharacter.transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, attackRotationSpeed * Time.deltaTime);
    }

    public void HandleActionRecovery(AICharacterManager aiCharacter)
    {
        if (actionRecoveryTime > 0)
        {
            if (!aiCharacter.isPerformingAction)
            {
                actionRecoveryTime -= Time.deltaTime;
            }
        }
    }
}
