using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;
    // Values taken from PlayerInputManager
    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float moveAmount;

    [Header("Movement Settings")]
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float runningSpeed = 4;
    [SerializeField] float sprintingSpeed = 6;
    [SerializeField] float rotationSpeed = 15;
    [SerializeField] int sprintingStaminaCost = 5;
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;

    [Header("Dodge Settings")]
    [SerializeField] float dodgeStaminaCost = 25;
    [SerializeField] float duckStaminaCost = 10;
    // [SerializeField] float dodgeDistance = 5;
    private Vector3 rollDirection;

    [Header("Jump Settings")]
    [SerializeField] float jumpStaminaCost = 20;
    [SerializeField] float jumpHeight = 2;
    private Vector3 jumpDirection;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Update()
    {
        base.Update();

        if (player.IsOwner)
        {
            player.characterNetworkManager.verticalMovement.Value = verticalMovement;
            player.characterNetworkManager.horizontalMovement.Value = horizontalMovement;
            player.characterNetworkManager.moveAmount.Value = moveAmount;
        }
        else
        {
            verticalMovement = player.characterNetworkManager.verticalMovement.Value;
            horizontalMovement = player.characterNetworkManager.horizontalMovement.Value;
            moveAmount = player.characterNetworkManager.moveAmount.Value;

            // If not locked-on, pass move amount
            if (!player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value)
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
            } // If locked-on, pass horizontal and vertical params
            else
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalMovement, verticalMovement, player.playerNetworkManager.isSprinting.Value);
            }
        }
    }

    public void HandleAllMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
        HandleJumping();
        HandleFreeFallMovement();
    }

    private void GetMovementValues()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;

        // Clamp the movements for animations
    }

    private void HandleGroundedMovement()
    {
        if (player.characterLocomotionManager.canMove || player.playerLocomotionManager.canRotate)
        {
            GetMovementValues();
        }

        if (!player.characterLocomotionManager.canMove)
        {
            return;
        }

        // Movement direction based on camera perspective and movement inputs
        moveDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        moveDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        moveDirection.y = 0;
        moveDirection.Normalize();

        if (player.playerNetworkManager.isSprinting.Value)
        {
            player.characterController.Move(sprintingSpeed * Time.deltaTime * moveDirection);
        }
        else
        {
            if (PlayerInputManager.instance.moveAmount > 0.5f)
            {
                // Move at running speed
                player.characterController.Move(runningSpeed * Time.deltaTime * moveDirection);
            }
            else if (PlayerInputManager.instance.moveAmount <= 0.5f)
            {
                // Move at walking speed
                player.characterController.Move(walkingSpeed * Time.deltaTime * moveDirection);
            }
        }
    }

    private void HandleRotation()
    {
        if (player.isDead.Value)
        {
            return;
        }

        if (!canRotate)
        {
            return;
        }

        if (player.playerNetworkManager.isLockedOn.Value)
        {
            if (player.playerNetworkManager.isSprinting.Value || player.playerLocomotionManager.isDodging)
            {
                Vector3 targetDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
                targetDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
                targetDirection.Normalize();
                targetDirection.y = 0;

                if (targetDirection == Vector3.zero)
                {
                    targetDirection = transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                transform.rotation = finalRotation;
            }
            else
            {
                if (player.playerCombatManager.currentTarget == null)
                {
                    return;
                }

                Vector3 targetDirection;
                targetDirection = player.playerCombatManager.currentTarget.transform.position - transform.position;
                targetDirection.y = 0;
                targetDirection.Normalize();

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                transform.rotation = finalRotation;
            }
        }
        else
        {
            targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;

            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
    }

    private void HandleJumping()
    {
        if (player.playerNetworkManager.isJumping.Value)
        {
            player.characterController.Move(jumpDirection * runningSpeed * Time.deltaTime);
        }
    }

    private void HandleFreeFallMovement()
    {
        if (!player.characterLocomotionManager.isGrounded)
        {
            Vector3 freeFallDirection;

            freeFallDirection = PlayerCamera.instance.transform.forward * PlayerInputManager.instance.verticalInput;
            freeFallDirection = freeFallDirection + PlayerCamera.instance.transform.right * PlayerInputManager.instance.horizontalInput;
            freeFallDirection.y = 0;

            player.characterController.Move(freeFallDirection * (walkingSpeed / 2) * Time.deltaTime);
        }
    }

    /* Used in the past to make player's dash cover more distance
    private IEnumerator HandleDodging()
    {
        float startTime = Time.time;
        float dashTime = 0.3f;

        while(Time.time < startTime + dashTime)
        {
            player.characterController.Move(dodgeDistance * Time.deltaTime * rollDirection);
            yield return null;
        }
    }
    */

    public void AttemptToPerformDodge()
    {
        if (player.isPerformingAction)
        {
            return;
        }

        if (player.playerNetworkManager.currentStamina.Value <= 0)
        {
            return;
        }

        // If moving -> dodge
        if (moveAmount > 0)
        {
            rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
            rollDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;

            rollDirection.y = 0;
            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            player.transform.rotation = playerRotation;

            player.animator.SetBool("IsRolling", true);

            if (!player.playerNetworkManager.isLockedOn.Value)
            {
                player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true, true);
            }
            else
            {
                player.playerAnimatorManager.PlayTargetActionAnimation("Dodge_Forward_01", true, true);
            }

            player.playerLocomotionManager.isDodging = true;

            // _ = StartCoroutine(HandleDodging());
            // StartCoroutine(HandleDodging());
            player.playerNetworkManager.currentStamina.Value -= dodgeStaminaCost;
        }
        else // If stationary -> backstep or duck
        {
            // rollDirection = -player.transform.forward * 1;
            // player.playerAnimatorManager.PlayTargetActionAnimation("Dodge_Backward_01", true, true);
            player.playerAnimatorManager.PlayTargetActionAnimation("Dodge_Duck_01", true, true);

            player.playerNetworkManager.currentStamina.Value -= duckStaminaCost;
        }
    }

    public void AttemptToPerformJump()
    {
        if (player.isPerformingAction)
        {
            return;
        }

        if (player.playerNetworkManager.currentStamina.Value <= 0)
        {
            return;
        }

        if (player.playerNetworkManager.isJumping.Value)
        {
            return;
        }

        if (!player.characterLocomotionManager.isGrounded)
        {
            return;
        }

        player.playerAnimatorManager.PlayTargetActionAnimation("Jump", true, true);

        // player.characterLocomotionManager.canJump = false;
        player.playerNetworkManager.isJumping.Value = true;

        player.playerNetworkManager.currentStamina.Value -= jumpStaminaCost;

        jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
        jumpDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
        jumpDirection.y = 0;

        if (jumpDirection != Vector3.zero)
        {
            if (player.playerNetworkManager.isSprinting.Value)
            {
                jumpDirection *= 1.25f;
            }
            else if (PlayerInputManager.instance.moveAmount > 0.5)
            {
                jumpDirection *= 1f;
            }
            else if (PlayerInputManager.instance.moveAmount <= 0.5)
            {
                jumpDirection *= 0.75f;
            }
        }
    }

    public void ApplyJumpingVelocity()
    {
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
    }

    public void HandleSprinting()
    {
        if (player.isPerformingAction)
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }

        if (player.playerNetworkManager.currentStamina.Value <= 0)
        {
            player.playerNetworkManager.isSprinting.Value = false;
            return;
        }

        if (moveAmount >= 0.5) // Moving -> true
        {
            player.playerNetworkManager.isSprinting.Value = true;
        }
        else // Stationary or slowly moving -> false
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }

        if (player.playerNetworkManager.isSprinting.Value)
        {
            player.playerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
        }
    }
}
