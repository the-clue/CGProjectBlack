using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PlayerManager : CharacterManager
{
    [Header("DEBUG MENU")]
    [SerializeField] bool revivePlayer = false;
    [SerializeField] bool switchRightWeapon = false;

    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    [HideInInspector] public PlayerInventoryManager playerInventoryManager;
    [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
    [HideInInspector] public PlayerCombatManager playerCombatManager;
    [HideInInspector] public PlayerInteractionManager playerInteractionManager;

    private bool isRespawning = false;

    protected override void Awake()
    {
        base.Awake();

        // Do more stuff, only for the player

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerInteractionManager = GetComponent<PlayerInteractionManager>();
    }

    protected override void Update()
    {
        base.Update();

        // If we do not own this gameobject, we do not control or edit it
        if (!IsOwner)
        {
            return;
        }

        if (isDead.Value) // If character is dead, no more movement nor regen
        {
            if (!isRespawning)
            {
                isRespawning = true;
                StartCoroutine(Respawn());
            }

            return;
        }

        // Handle movement
        playerLocomotionManager.HandleAllMovement();

        // Regen
        playerStatsManager.RegenerateHealth();
        playerStatsManager.RegenerateStamina();

        DebugMenu();
    }

    protected override void LateUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraActions();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

        if (IsOwner)
        {
            PlayerCamera.instance.player = this;
            PlayerInputManager.instance.player = this;
            WorldSaveGameManager.instance.player = this;

            // Updates UI when health or stamina changes
            playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
            playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;

            playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
            playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
            playerNetworkManager.currentHealth.OnValueChanged += playerStatsManager.ResetHealthRegenTimer;
            playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;
        }

        if (!IsOwner) // this makes others' health bars visible to you but not your own
        {
            characterNetworkManager.currentHealth.OnValueChanged += characterUIManager.OnHealthChanged;
        }

        playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHealth;

        // lock-on
        playerNetworkManager.isLockedOn.OnValueChanged += playerNetworkManager.OnIsLockedOnChanged;
        playerNetworkManager.currentTargetNetworkID.OnValueChanged += playerNetworkManager.OnLockOnTargetIDChange;

        // EQUIPMENT
        playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange;
        playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
        playerNetworkManager.currentWeaponBeingUsed.OnValueChanged += playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;

        playerNetworkManager.isChargingAttack.OnValueChanged += playerNetworkManager.OnIsChargingAttackChanged;

        // Upon connecting, if we are the owner of this character, but we are not the server, reload our character data
        // to this newly instantiated character; we don't run this if we are the server as we would already load our character
        if (IsOwner && !IsServer)
        {
            LoadGameDataFromCurrentCharacterData(ref WorldSaveGameManager.instance.currentCharacterData);
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;

        if (IsOwner)
        {
            // Updates UI when health or stamina changes
            playerNetworkManager.vitality.OnValueChanged -= playerNetworkManager.SetNewMaxHealthValue;
            playerNetworkManager.endurance.OnValueChanged -= playerNetworkManager.SetNewMaxStaminaValue;

            playerNetworkManager.currentHealth.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
            playerNetworkManager.currentStamina.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
            playerNetworkManager.currentStamina.OnValueChanged -= playerStatsManager.ResetStaminaRegenTimer;
        }

        if (!IsOwner)
        {
            characterNetworkManager.currentHealth.OnValueChanged += characterUIManager.OnHealthChanged;
        }

        playerNetworkManager.currentHealth.OnValueChanged -= playerNetworkManager.CheckHealth;

        // lock-on
        playerNetworkManager.isLockedOn.OnValueChanged -= playerNetworkManager.OnIsLockedOnChanged;
        playerNetworkManager.currentTargetNetworkID.OnValueChanged -= playerNetworkManager.OnLockOnTargetIDChange;

        // EQUIPMENT
        playerNetworkManager.currentRightHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentRightHandWeaponIDChange;
        playerNetworkManager.currentLeftHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
        playerNetworkManager.currentWeaponBeingUsed.OnValueChanged -= playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;

        playerNetworkManager.isChargingAttack.OnValueChanged -= playerNetworkManager.OnIsChargingAttackChanged;
    }

    private void OnClientConnectedCallback(ulong clientID)
    {
        // keep list of active players in game
        WorldGameSessionManager.instance.AddPlayerToActivePlayersList(this);

        // if host, no need to load players to sync them
        // if client, load other players' gear to sync them if joining a game that has already been active
        if (!IsServer && IsOwner)
        {
            foreach (var player in WorldGameSessionManager.instance.players)
            {
                if (player != this)
                {
                    player.LoadOtherPlayerWhenJoiningServer();
                }
            }
        }
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if (IsOwner)
        {
            PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();
        }

        // award player with runes
        // disable character
        return base.ProcessDeathEvent(manuallySelectDeathAnimation);
    }

    public override void ReviveCharacter()
    {
        base.ReviveCharacter();

        if (IsOwner)
        {
            isDead.Value = false;
            playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
            playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;

            playerAnimatorManager.PlayTargetActionAnimation("Empty", false);
        }
    }

    public void SaveGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        // Location Saving
        currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        currentCharacterData.xPosition = transform.position.x;
        currentCharacterData.yPosition = transform.position.y;
        currentCharacterData.zPosition = transform.position.z;

        // Status Saving
        currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
        currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;
        currentCharacterData.currentStamina = playerNetworkManager.currentStamina.Value;
        currentCharacterData.vitality = playerNetworkManager.vitality.Value;
        currentCharacterData.endurance = playerNetworkManager.endurance.Value;

        // Inventory Saving
        currentCharacterData.itemsInInventory.Clear();
        foreach (Item item in playerInventoryManager.itemsInInventory)
        {
            if (currentCharacterData.itemsInInventory.ContainsKey(item.itemID))
            {
                currentCharacterData.itemsInInventory[item.itemID] += 1;
            }
            else
            {
                currentCharacterData.itemsInInventory[item.itemID] = 1;
            }
        }

        // Equipment Saving
        for (int i = 0; i < playerInventoryManager.weaponsInRightHandSlots.Length; i++)
        {
            currentCharacterData.weaponsInWeaponSlots[i] = playerInventoryManager.weaponsInRightHandSlots[i].itemID;
        }
    }

    public void SaveGameDataFromCurrentCharacterDataWithoutPosition(ref CharacterSaveData currentCharacterData)
    {
        // Status Saving
        currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
        currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;
        currentCharacterData.currentStamina = playerNetworkManager.currentStamina.Value;
        currentCharacterData.vitality = playerNetworkManager.vitality.Value;
        currentCharacterData.endurance = playerNetworkManager.endurance.Value;

        // Inventory Saving
        currentCharacterData.itemsInInventory.Clear();
        foreach (Item item in playerInventoryManager.itemsInInventory)
        {
            if (currentCharacterData.itemsInInventory.ContainsKey(item.itemID))
            {
                currentCharacterData.itemsInInventory[item.itemID] += 1;
            }
            else
            {
                currentCharacterData.itemsInInventory[item.itemID] = 1;
            }
        }

        // Equipment Saving
        for (int i = 0; i < playerInventoryManager.weaponsInRightHandSlots.Length; i++)
        {
            currentCharacterData.weaponsInWeaponSlots[i] = playerInventoryManager.weaponsInRightHandSlots[i].itemID;
        }
    }

    public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        playerNetworkManager.characterName.Value = currentCharacterData.characterName;
        Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition,
            currentCharacterData.zPosition);
        transform.position = myPosition;

        playerNetworkManager.vitality.Value = currentCharacterData.vitality;
        playerNetworkManager.endurance.Value = currentCharacterData.endurance;

        playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealthBasedOnVitalityLevel(playerNetworkManager.vitality.Value);
        PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(playerNetworkManager.maxHealth.Value);
        playerNetworkManager.currentHealth.Value = currentCharacterData.currentHealth;
        PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue(0, currentCharacterData.currentHealth);

        playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
        PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
        playerNetworkManager.currentStamina.Value = currentCharacterData.currentStamina;
        PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue(0, currentCharacterData.currentStamina);

        // Inventory Loading
        foreach (var entry in currentCharacterData.itemsInInventory)
        {
            int itemID = entry.Key;
            int quantity = entry.Value;

            for (int i = 0; i < quantity; i++)
            {
                Item item = WorldItemDatabase.instance.GetWeaponByID(itemID);
                playerInventoryManager.AddItemToInventory(item);
            }
        }

        // Equipment Loading
        for (int i = 0; i < playerInventoryManager.weaponsInRightHandSlots.Length; i++)
        {
            playerInventoryManager.weaponsInRightHandSlots[i] = WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.weaponsInWeaponSlots[i]);
            
            // to load first weapon, can also implement precise slot position in the future
            playerNetworkManager.currentRightHandWeaponID.Value = playerInventoryManager.weaponsInRightHandSlots[0].itemID;
        }
    }

    public void LightlyLoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        playerNetworkManager.characterName.Value = currentCharacterData.characterName;
        Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition,
            currentCharacterData.zPosition);
        transform.position = myPosition;

        playerNetworkManager.vitality.Value = currentCharacterData.vitality;
        playerNetworkManager.endurance.Value = currentCharacterData.endurance;

        playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealthBasedOnVitalityLevel(playerNetworkManager.vitality.Value);
        PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(playerNetworkManager.maxHealth.Value);
        playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
        PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue(0, playerNetworkManager.currentHealth.Value);

        playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
        PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
        playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;
        PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue(0, playerNetworkManager.currentStamina.Value);
    }

    public void LoadOtherPlayerWhenJoiningServer()
    {
        // sync weapons
        playerNetworkManager.OnCurrentRightHandWeaponIDChange(0, playerNetworkManager.currentRightHandWeaponID.Value);
        playerNetworkManager.OnCurrentLeftHandWeaponIDChange(0, playerNetworkManager.currentLeftHandWeaponID.Value);

        if (playerNetworkManager.isLockedOn.Value)
        {
            playerNetworkManager.OnLockOnTargetIDChange(0, playerNetworkManager.currentTargetNetworkID.Value);
        }
    }

    IEnumerator Respawn()
    {
        int respawnDelay = 3; // can be put global

        yield return new WaitForSeconds(respawnDelay);

        playerNetworkManager.isLockedOn.Value = false;
        WorldAIManager.instance.ResetAllCharacters();
        ReviveCharacter();

        LightlyLoadGameDataFromCurrentCharacterData(ref WorldSaveGameManager.instance.currentCharacterData);

        isRespawning = false;
    }

    public void AddStatusPoints(int statusType, int statusPoints)
    {
        if (statusType == 0)
        {
            playerNetworkManager.vitality.Value += statusPoints;
        }
        else if (statusType == 1)
        {
            playerNetworkManager.endurance.Value += statusPoints;
        }
        else
        {
            Debug.Log("Status <" + statusType + "> does not exist!");
        }
    }

    private void DebugMenu()
    {
        if (revivePlayer)
        {
            revivePlayer = false;
            ReviveCharacter();
        }
        if (switchRightWeapon)
        {
            switchRightWeapon = false;
            playerEquipmentManager.SwitchRightWeapon();
        }
    }
}
