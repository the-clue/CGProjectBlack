using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;

    public PlayerManager player;

    PlayerControls playerControls;

    [Header("Player Movement Input")]
    [SerializeField] Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

    [Header("Camera Movement Input")]
    [SerializeField] Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;

    [Header("Lock-On Input")]
    [SerializeField] bool lockOnInput;
    [SerializeField] bool lockOnLeftInput;
    [SerializeField] bool lockOnRightInput;
    private Coroutine lockOnCoroutine;

    [Header("Player Action Input")]
    [SerializeField] bool dodgeInput = false;
    [SerializeField] bool sprintInput = false;
    [SerializeField] bool jumpInput = false;
    [SerializeField] bool RB_Input = false;
    [SerializeField] bool RT_Input = false;
    [SerializeField] bool RT_HoldInput = false;
    [SerializeField] bool switchWeaponRightInput = false;
    [SerializeField] bool switchWeaponLeftInput = false;
    [SerializeField] bool interactionInput = false;

    [Header("Qued Input")]
    private bool inputQueIsActive = false;
    [SerializeField] float queInputTimer = 0f;
    [SerializeField] float defaultQueInputTimer = 0.35f;
    [SerializeField] bool quedRB_Input = false;
    [SerializeField] bool quedRT_Input = false;
    [SerializeField] bool quedDodge_Input = false;
    [SerializeField] bool quedJump_Input = false;

    [Header("UI Input")]
    [SerializeField] bool openMenuInput = false;
    [SerializeField] bool closeMenuInput = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        // When the scene changes, run this logic
        SceneManager.activeSceneChanged += OnSceneChange;

        instance.enabled = false;

        if (playerControls != null)
        {
            playerControls.Disable();
        }
    }

    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        // If we are loading into our world scene, enable our player's controls
        if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            instance.enabled = true;

            if (playerControls != null)
            {
                playerControls.Enable();
            }
        }
        // Otherwise we must be at the main menu, disable our player's controls
        else
        {
            instance.enabled = false;

            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            // Movement
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();

            // Actions
            playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            playerControls.PlayerActions.Interact.performed += i => interactionInput = true;

            playerControls.PlayerActions.SwitchRightWeapon.performed += i => switchWeaponRightInput = true;
            playerControls.PlayerActions.SwitchLeftWeapon.performed += i => switchWeaponLeftInput = true;

            playerControls.PlayerActions.RB.performed += i => RB_Input = true;

            // Triggers
            playerControls.PlayerActions.RT.performed += i => RT_Input = true;
            playerControls.PlayerActions.RTHold.performed += i => RT_HoldInput = true;
            playerControls.PlayerActions.RTHold.canceled += i => RT_HoldInput = false;

            // Lock-On
            playerControls.PlayerActions.LockOn.performed += i => lockOnInput = true;
            playerControls.PlayerActions.LockOnLeft.performed += i => lockOnLeftInput = true;
            playerControls.PlayerActions.LockOnRight.performed += i => lockOnRightInput = true;

            // Qued Inputs
            playerControls.PlayerActions.QuedRB.performed += i => QueInput(ref quedRB_Input);
            playerControls.PlayerActions.QuedRT.performed += i => QueInput(ref quedRT_Input);
            playerControls.PlayerActions.QuedDodge.performed += i => QueInput(ref quedDodge_Input);
            playerControls.PlayerActions.QuedJump.performed += i => QueInput(ref quedJump_Input);

            // UI Inputs
            playerControls.PlayerActions.Dodge.performed += i => closeMenuInput = true;
            playerControls.UI.Menu.performed += i => openMenuInput = true;
        }
        playerControls.Enable();
    }

    private void OnDestroy()
    {
        // If we destroy this object, unsubscribe from this event
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    // If we minimize or lower the window, stop adjusting inputs
    private void OnApplicationFocus(bool focus)
    {
        if (enabled)
        {
            if (focus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }

    private void Update() // Handles every input
    {
        HandleLockOnInput();
        HandleLockOnSwitchTargetInput();
        HandlePlayerMovementInput();
        HandleCameraMovementInput();
        HandleDodgeInput();
        HandleSprintingInput();
        HandleJumpInput();
        HandleRBInput();
        HandleRTInput();
        HandleRTChargedInput();
        HandleSwitchWeaponRightInput();
        HandleSwitchWeaponLeftInput();
        HandleQuedInputs();
        HandleInteractionInput();
        HandleOpenCharacterMenuInput();
        HandleCloseUIInput();
    }

    // Menu
    //private void HandleMenuInput()
    //{
    //    if (startInput)
    //    {
    //        startInput = false;
    //        NetworkManager.Singleton.Shutdown();
    //        WorldSoundFXManager.instance.StopBossMusic();
    //        Destroy(FindAnyObjectByType(typeof(UI_Boss_HP_Bar)));
    //        SceneManager.LoadScene(0);
    //    }
    //}

    // Movement
    private void HandlePlayerMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        // Returns the absolute number
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        // We clamp the values, so they are 0, 0.5 or 1 (optional)
        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = 1;
        }

        if (player == null)
        {
            return;
        }

        if (moveAmount != 0)
        {
            player.playerNetworkManager.isMoving.Value = true;
        }
        else
        {
            player.playerNetworkManager.isMoving.Value = false;
        }

        // We pass 0 because we are not locked-on
        if (!player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value)
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
        } // If locked-on, pass horizontal and vertical params
        else
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalInput, verticalInput, player.playerNetworkManager.isSprinting.Value);
        }
    }

    private void HandleCameraMovementInput()
    {
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;
    }

    // Actions
    private void HandleDodgeInput()
    {
        if (dodgeInput)
        {
            dodgeInput = false;
            // Perform a dodge

            player.playerLocomotionManager.AttemptToPerformDodge();
        }
    }

    private void HandleSprintingInput()
    {
        if (sprintInput)
        {
            player.playerLocomotionManager.HandleSprinting();
        }
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }
    }

    private void HandleJumpInput()
    {
        if (jumpInput)
        {
            jumpInput = false;

            player.playerLocomotionManager.AttemptToPerformJump();
        }
    }

    private void HandleLockOnInput()
    {
        if (player.playerNetworkManager.isLockedOn.Value)
        {
            if (player.playerCombatManager.currentTarget == null)
            {
                return;
            }

            if (player.playerCombatManager.currentTarget.isDead.Value)
            {
                player.playerNetworkManager.isLockedOn.Value = false;
                // so that the coroutine doesn't run multiple times in parallel
                if (lockOnCoroutine != null)
                {
                    StopCoroutine(lockOnCoroutine);
                }
                lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
            }
        }

        if (lockOnInput && player.playerNetworkManager.isLockedOn.Value)
        {
            lockOnInput = false;
            PlayerCamera.instance.ClearLockedOnTargets();
            player.playerNetworkManager.isLockedOn.Value = false;

            return;
        }

        if (lockOnInput && !player.playerNetworkManager.isLockedOn.Value)
        {
            lockOnInput = false;

            PlayerCamera.instance.HandleLocatingLockOnTargets();

            if (PlayerCamera.instance.nearestLockOnTarget != null)
            {
                player.playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                player.playerNetworkManager.isLockedOn.Value = true;
            }
        }
    }

    private void HandleLockOnSwitchTargetInput()
    {
        if (lockOnLeftInput)
        {
            lockOnLeftInput = false;

            if (player.playerNetworkManager.isLockedOn.Value)
            {
                PlayerCamera.instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.instance.leftLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                }
            }
        }

        if (lockOnRightInput)
        {
            lockOnRightInput = false;

            if (player.playerNetworkManager.isLockedOn.Value)
            {
                PlayerCamera.instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.instance.rightLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
                }
            }
        }
    }

    private void HandleRBInput()
    {
        if (RB_Input)
        {
            RB_Input = false;

            player.playerNetworkManager.SetCharacterActionHand(true);

            player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.rightHandAction,
                player.playerInventoryManager.currentRightHandWeapon);
        }
    }

    private void HandleRTInput()
    {
        if (RT_Input)
        {
            RT_Input = false;

            player.playerNetworkManager.SetCharacterActionHand(true);

            player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.rightHandHeavyAction,
                player.playerInventoryManager.currentRightHandWeapon);
        }
    }

    private void HandleRTChargedInput()
    {
        // we only check this only if we are already in an action
        if (player.isPerformingAction)
        {
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                player.playerNetworkManager.isChargingAttack.Value = RT_HoldInput;
            }
        }
    }

    private void HandleSwitchWeaponRightInput()
    {
        if (switchWeaponRightInput)
        {
            switchWeaponRightInput = false;

            if (PlayerUIManager.instance.menuWindowIsOpen)
            {
                return;
            }

            player.playerEquipmentManager.SwitchRightWeapon();
        }
    }

    private void HandleSwitchWeaponLeftInput()
    {
        if (switchWeaponLeftInput)
        {
            switchWeaponLeftInput = false;

            if (PlayerUIManager.instance.menuWindowIsOpen)
            {
                return;
            }

            player.playerEquipmentManager.SwitchLeftWeapon();
        }
    }

    private void HandleInteractionInput()
    {
        if (interactionInput)
        {
            interactionInput = false;

            player.playerInteractionManager.Interact();
        }
    }

    private void QueInput(ref bool quedInput) // ref allows the passing of the bool instead of its value
    {
        // Reset all qued inputs so only one can be qued at a time
        quedRB_Input = false;
        quedRT_Input = false;
        quedDodge_Input = false;
        quedJump_Input = false;

        // To implement: check if UI windows are open

        if (player.isPerformingAction || player.playerNetworkManager.isJumping.Value)
        {
            quedInput = true;

            // Attempt qued input for x amount of time
            queInputTimer = defaultQueInputTimer;
            inputQueIsActive = true;
        }
    }

    private void ProcessQuedInputs()
    {
        if (player.isDead.Value)
        {
            return;
        }

        if (quedRB_Input)
        {
            RB_Input = true;
        }

        if (quedRT_Input)
        {
            RT_Input = true;
        }

        if (quedDodge_Input)
        {
            dodgeInput = true;
        }

        if (quedJump_Input)
        {
            jumpInput = true;
        }
    }

    private void HandleQuedInputs()
    {
        if (inputQueIsActive)
        {
            // While timer is positive, keep attempting the qued input
            if (queInputTimer > 0)
            {
                queInputTimer -= Time.deltaTime;
                ProcessQuedInputs();
            }
            else // Reset qued inputs
            {
                quedRB_Input = false;
                quedRT_Input = false;
                quedDodge_Input = false;
                quedJump_Input = false;
                inputQueIsActive = false;
                queInputTimer = 0;
            }
        }
    }

    private void HandleOpenCharacterMenuInput()
    {
        if (openMenuInput)
        {
            openMenuInput = false;

            PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
            PlayerUIManager.instance.CloseAllMenuWindows();
            PlayerUIManager.instance.playerUICharacterMenuManager.OpenCharacterMenu();
        }
    }

    private void HandleCloseUIInput()
    {
        if (closeMenuInput)
        {
            closeMenuInput = false;

            if (PlayerUIManager.instance.menuWindowIsOpen)
            {
                PlayerUIManager.instance.CloseAllMenuWindows();
            }
        }
    }
}
