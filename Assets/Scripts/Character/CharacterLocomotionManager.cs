using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotionManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Ground check and jumping")]
    [SerializeField] protected float gravityForce = -40;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckSphereRadius = 0.3f;
    [SerializeField] protected Vector3 yVelocity;
    [SerializeField] protected float groundedVelocity = -20;
    [SerializeField] protected float fallStartVelocity = -5;
    protected bool fallingVelocityHasBeenSet = false;
    protected float inAirTimer = 0;

    [Header("Flags")]
    public bool isDodging = false;
    public bool isGrounded = true;
    public bool canRotate = true;
    // public bool canJump = true;
    public bool canMove = true;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Update()
    {
        HandleGroundCheck();

        if (isGrounded)
        {
            if (yVelocity.y < 0)
            {
                inAirTimer = 0;
                fallingVelocityHasBeenSet = false;
                yVelocity.y = groundedVelocity;
            }
        }
        else
        {
            if (!character.characterNetworkManager.isJumping.Value && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                yVelocity.y = fallStartVelocity;
            }

            inAirTimer += Time.deltaTime;
            character.animator.SetFloat("InAirTimer", inAirTimer);

            yVelocity.y += gravityForce * Time.deltaTime;
        }
        character.characterController.Move(yVelocity * Time.deltaTime);
    }

    protected void HandleGroundCheck ()
    {
        isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
    }

    /*
    protected void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
    }
    */
     
    /*
    public void EnableCanMove()
    {
        canMove = true;
    }
    public void DisableCanMove()
    {
        canMove = false;
    }
    */

    public void EnableCanRotate()
    {
        canRotate = true;
    }

    public void DisableCanRotate()
    {
        canRotate = false;
    }
}
