using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetActionFlagJump : StateMachineBehaviour
{
    CharacterManager character;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (character == null)
        {
            character = animator.GetComponent<CharacterManager>();
        }

        if (character.characterCombatManager.isDoingJumpattack)
        {
            return;
        }

        // Called when action ends
        character.isPerformingAction = false;
        character.characterAnimatorManager.applyRootMotion = false;
        // character.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 0, false);
        character.characterLocomotionManager.canRotate = true;
        character.characterLocomotionManager.canMove = true;

        character.characterLocomotionManager.isDodging = false;
        character.characterCombatManager.DisableCanDoCombo();
        character.characterCombatManager.DisableCanDoDodgeAttack();
        character.characterCombatManager.DisableCanDoDuckAttack();
        character.characterCombatManager.DisableCanDoJumpAttack();

        character.characterEquipmentManager.CloseDamageCollider();

        if (character.IsOwner)
        {
            // Added, to test
            // character.characterNetworkManager.verticalMovement.Value = 0;
            // character.characterNetworkManager.horizontalMovement.Value = 0;
            // character.characterNetworkManager.moveAmount.Value = 0;
            // character.characterNetworkManager.isMoving.Value = false;

            character.characterNetworkManager.isJumping.Value = false;
            character.characterNetworkManager.isInvulnerable.Value = false;
        }
        // character.animator.applyRootMotion = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
