using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;

    int vertical;
    int horizontal;

    [Header("Flags")]
    public bool applyRootMotion = false;

    [Header("Damage Animations")]
    public string hit_Forward_Medium_01 = "Hit_Forward_Medium_01";
    public string hit_Backward_Medium_01 = "Hit_Backward_Medium_01";
    public string hit_Left_Medium_01 = "Hit_Left_Medium_01";
    public string hit_Right_Medium_01 = "Hit_Right_Medium_01";
    public string hit_Forward_Ping_01 = "Hit_Forward_Ping_01";
    public string hit_Backward_Ping_01 = "Hit_Backward_Ping_01";
    public string hit_Left_Ping_01 = "Hit_Left_Ping_01";
    public string hit_Right_Ping_01 = "Hit_Right_Ping_01";

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue, bool isSprinting)
    {
        if (character == null)
        {
            return;
        }

        float snappedHorizontalMovement;
        float snappedVerticalMovement;
        
        if (horizontalValue > 0 && horizontalValue <= 0.5f)
        {
            snappedHorizontalMovement = 0.5f;
        }
        else if (horizontalValue > 0.5f && horizontalValue <= 1)
        {
            snappedHorizontalMovement = 1;
        }
        else if (horizontalValue < 0 && horizontalValue >= -0.5f)
        {
            snappedHorizontalMovement = -0.5f;
        }
        else if (horizontalValue < -0.5f && horizontalValue >= -1)
        {
            snappedHorizontalMovement = -1;
        }
        else
        {
            snappedHorizontalMovement = 0;
        }

        if (verticalValue > 0 && verticalValue <= 0.5f)
        {
            snappedVerticalMovement = 0.5f;
        }
        else if (verticalValue > 0.5f && verticalValue <= 1)
        {
            snappedVerticalMovement = 1;
        }
        else if (verticalValue < 0 && verticalValue >= -0.5f)
        {
            snappedVerticalMovement = -0.5f;
        }
        else if (verticalValue < -0.5f && verticalValue >= -1)
        {
            snappedVerticalMovement = -1;
        }
        else
        {
            snappedVerticalMovement = 0;
        }

        if (isSprinting)
        {
            snappedVerticalMovement = 2;
        }

        character.animator.SetFloat(vertical, snappedVerticalMovement, 0.1f, Time.deltaTime);
        character.animator.SetFloat(horizontal, snappedHorizontalMovement, 0.1f, Time.deltaTime);
    }

    public virtual void PlayTargetActionAnimation(string targetAnimation, bool isPerformingAction, bool applyRootMotion = true,
        bool canRotate = false, bool canMove = false)
    {
        Debug.Log("Playing animation: " + targetAnimation);
        this.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);
        // Can be used to stop character from attempting new actions
        // Example: if you get damaged, and begin performing a damage animation
        // This flag will turn true if you are stunned
        character.isPerformingAction = isPerformingAction;
        character.characterLocomotionManager.canRotate = canRotate;
        character.characterLocomotionManager.canMove = canMove;

        // Tell the server we played an animation
        character.characterNetworkManager.NotifyTheServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId,
            targetAnimation, applyRootMotion);
    }

    public virtual void PlayTargetAttackActionAnimation(WeaponItem weapon, AttackType attackType,
        string targetAnimation, bool isPerformingAction, bool applyRootMotion = true,
        bool canRotate = false, bool canMove = false)
    {
        // keep track of last attack performed for combos
        character.characterCombatManager.currentAttackType = attackType;
        character.characterCombatManager.lastAttackAnimationPerformed = targetAnimation;

        UpdateAnimatorController(weapon.weaponAnimator);

        //character.characterAnimatorManager.applyRootMotion = applyRootMotion;
        this.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);

        character.isPerformingAction = isPerformingAction;
        character.characterLocomotionManager.canRotate = canRotate;
        character.characterLocomotionManager.canMove = canMove;

        character.characterNetworkManager.NotifyTheServerOfAttackActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }

    public void UpdateAnimatorController(AnimatorOverrideController weaponController)
    {
        character.animator.runtimeAnimatorController = weaponController;
    }
}
