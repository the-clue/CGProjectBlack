using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class CharacterManager : NetworkBehaviour
{
    [Header("Status")]
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;

    [HideInInspector] public CharacterNetworkManager characterNetworkManager;
    [HideInInspector] public CharacterEffectsManager characterEffectsManager;
    [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
    [HideInInspector] public CharacterEquipmentManager characterEquipmentManager;
    [HideInInspector] public CharacterCombatManager characterCombatManager;
    [HideInInspector] public CharacterStatsManager characterStatsManager;
    [HideInInspector] public CharacterSoundFXManager characterSoundFXManager;
    [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;
    [HideInInspector] public CharacterUIManager characterUIManager;


    [Header("Character Group")]
    public CharacterGroup characterGroup;

    [Header("Flags")]
    public bool isPerformingAction = false;
    // public bool applyRootMotion = false;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        characterEffectsManager = GetComponent<CharacterEffectsManager>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
        characterEquipmentManager = GetComponent<CharacterEquipmentManager>();
        characterCombatManager = GetComponent<CharacterCombatManager>();
        characterStatsManager = GetComponent<CharacterStatsManager>();
        characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
        characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
        characterUIManager = GetComponent<CharacterUIManager>();
    }

    protected virtual void Start()
    {
        IgnoreMyOwnColliders();
    }

    protected virtual void Update()
    {
        animator.SetBool("IsGrounded", characterLocomotionManager.isGrounded);

        // If this character is being controlled from our side, then assign its network position to the position of our transform
        if (IsOwner)
        {
            characterNetworkManager.networkPosition.Value = transform.position;
            characterNetworkManager.networkRotation.Value = transform.rotation;
        }
        // If this character is being controlled elsewhere, then assign its position here locally by the position of its network transform
        else
        {
            // Position
            transform.position = Vector3.SmoothDamp(transform.position, characterNetworkManager.networkPosition.Value,
                ref characterNetworkManager.networkPositionVelocity, characterNetworkManager.networkPositionSmoothTime);
            // Rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, characterNetworkManager.networkRotation.Value,
                characterNetworkManager.networkRotationSmoothTime);
        }
    }

    protected virtual void FixedUpdate()
    {

    }

    protected virtual void LateUpdate()
    {

    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        animator.SetBool("IsMoving", characterNetworkManager.isMoving.Value);
        // alternative
        // characterNetworkManager.OnIsMovingChanged(false, characterNetworkManager.isMoving.Value);

        characterNetworkManager.OnIsActiveChanged(false, characterNetworkManager.isActive.Value);

        characterNetworkManager.isMoving.OnValueChanged += characterNetworkManager.OnIsMovingChanged;
        characterNetworkManager.isActive.OnValueChanged += characterNetworkManager.OnIsActiveChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        characterNetworkManager.isMoving.OnValueChanged -= characterNetworkManager.OnIsMovingChanged;
        characterNetworkManager.isActive.OnValueChanged -= characterNetworkManager.OnIsActiveChanged;
    }

    public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if (IsOwner)
        {
            characterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;

            // reset necessary flags

            //if not grounded, play air death animation

            if (!manuallySelectDeathAnimation)
            {
                characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
            }
        }

        // death sfx

        yield return new WaitForSeconds(5);
        // award player with runes
        // disable character
    }

    public virtual void ReviveCharacter()
    {

    }

    // To avoid collisions between own colliders
    protected virtual void IgnoreMyOwnColliders()
    {
        Collider characterControllerCollider = GetComponent<Collider>();
        Collider[] damageableColliders = GetComponentsInChildren<Collider>();
        
        List<Collider> ignoreColliders = new List<Collider>();

        foreach(var collider in damageableColliders)
        {
            ignoreColliders.Add(collider);
        }

        ignoreColliders.Add(characterControllerCollider);

        foreach(var collider in ignoreColliders)
        {
            foreach(var otherCollider in ignoreColliders)
            {
                Physics.IgnoreCollision(collider, otherCollider, true);
            }
        }
    }
}
