using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : CharacterEquipmentManager
{
    PlayerManager player;

    public WeaponInstantiationSlot rightHandSlot;
    public WeaponInstantiationSlot leftHandSlot;

    [SerializeField] WeaponManager rightWeaponManager;
    [SerializeField] WeaponManager leftWeaponManager;

    public GameObject rightHandWeaponModel;
    public GameObject leftHandWeaponModel;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();

        InitializeWeaponSlots();
    }

    protected override void Start()
    {
        base.Start();

        LoadWeaponsOnBothHands();
    }

    private void InitializeWeaponSlots()
    {
        WeaponInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponInstantiationSlot>();
        
        foreach(var weaponSlot in weaponSlots)
        {
            if(weaponSlot.weaponSlot == WeaponSlot.RightHand)
            {
                rightHandSlot = weaponSlot;
            }
            else if (weaponSlot.weaponSlot == WeaponSlot.LefHand)
            {
                leftHandSlot = weaponSlot;
            }
        }
    }

    public void LoadWeaponsOnBothHands()
    {
        LoadRightWeapon();
        LoadLeftWeapon();
    }

    public void LoadRightWeapon()
    {
        if (player.playerInventoryManager.currentRightHandWeapon != null)
        {
            // Remove old weapon
            rightHandSlot.UnloadWeapon();

            // Bring in new weapon
            rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightHandWeapon.weaponModel);
            rightHandSlot.LoadWeapon(rightHandWeaponModel);
            rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
            rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon);
            player.playerAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentRightHandWeapon.weaponAnimator);
        }
    }

    public void SwitchRightWeapon()
    {
        if (!player.IsOwner)
        {
            return;
        }
        if (player.isPerformingAction)
        {
            return;
        }

        player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Right_Weapon_01", false, false, true, true);

        WeaponItem selectedWeapon = null;

        player.playerInventoryManager.rightHandWeaponIndex += 1;

        // if (player.playerInventoryManager.rightHandWeaponIndex < 0 || player.playerInventoryManager.rightHandWeaponIndex > player.playerInventoryManager.weaponsInRightHandSlots.Length - 1)
        if (player.playerInventoryManager.rightHandWeaponIndex < 0 || player.playerInventoryManager.rightHandWeaponIndex > 2)
        {
            player.playerInventoryManager.rightHandWeaponIndex = 0;

            float weaponCount = 0;
            WeaponItem firstWeapon = null;
            int firstWeaponPosition = 0;

            for (int i = 0; i < player.playerInventoryManager.weaponsInRightHandSlots.Length; i++)
            {
                if (player.playerInventoryManager.weaponsInRightHandSlots[i].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    weaponCount += 1;
                    if (firstWeapon == null)
                    {
                        firstWeapon = player.playerInventoryManager.weaponsInRightHandSlots[i];
                        firstWeaponPosition = i;
                    }
                }
            }

            if (weaponCount <= 1)
            {
                player.playerInventoryManager.rightHandWeaponIndex = -1;
                selectedWeapon = WorldItemDatabase.instance.unarmedWeapon;
                player.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;
            }
            else
            {
                player.playerInventoryManager.rightHandWeaponIndex = firstWeaponPosition;
                player.playerNetworkManager.currentRightHandWeaponID.Value = firstWeapon.itemID;
            }

            return;
        }

        foreach (WeaponItem weapon in player.playerInventoryManager.weaponsInRightHandSlots)
        {
            if (player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
            {
                selectedWeapon = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex];
                player.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;
                // player.playerNetworkManager.currentRightHandWeaponID.Value = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex.itemID];
                return;
            }
        }
        // selectedWeapon = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex];
        // player.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;

        if (selectedWeapon == null && player.playerInventoryManager.rightHandWeaponIndex <= 2)
        {
            SwitchRightWeapon();
        }
    }

    public void LoadLeftWeapon()
    {
        if (player.playerInventoryManager.currentLeftHandWeapon != null)
        {
            leftHandSlot.UnloadWeapon();

            leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);
            leftHandSlot.LoadWeapon(leftHandWeaponModel);
            leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
            leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);
        }
    }

    public void SwitchLeftWeapon()
    {
        if (!player.IsOwner)
        {
            return;
        }

        player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Left_Weapon_01", false, false, true, true);

        WeaponItem selectedWeapon;

        player.playerInventoryManager.leftHandWeaponIndex += 1;

        if (player.playerInventoryManager.leftHandWeaponIndex < 0 || player.playerInventoryManager.leftHandWeaponIndex > 1)
        {
            player.playerInventoryManager.leftHandWeaponIndex = 0;
        }

        foreach (WeaponItem weapon in player.playerInventoryManager.weaponsInLeftHandSlots)
        {
            selectedWeapon = player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex];

            player.playerNetworkManager.currentLeftHandWeaponID.Value = selectedWeapon.itemID;
        }
    }

    // Damage Colliders
    public override void OpenDamageCollider()
    {
        if (player.playerNetworkManager.isUsingRightHand.Value)
        {
            rightWeaponManager.meleeWeaponDamageCollider.EnableDamageCollider();
            player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(player.playerInventoryManager.currentRightHandWeapon.swingSounds));
        }
        else if (player.playerNetworkManager.isUsingLeftHand.Value)
        {
            leftWeaponManager.meleeWeaponDamageCollider.EnableDamageCollider();
            player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(player.playerInventoryManager.currentLeftHandWeapon.swingSounds));
        }
    }

    public override void CloseDamageCollider()
    {
        if (player.playerNetworkManager.isUsingRightHand.Value)
        {
            rightWeaponManager.meleeWeaponDamageCollider.DisableDamageCollider();
        }
        else if (player.playerNetworkManager.isUsingLeftHand.Value)
        {
            leftWeaponManager.meleeWeaponDamageCollider.DisableDamageCollider();
        }
    }
}
