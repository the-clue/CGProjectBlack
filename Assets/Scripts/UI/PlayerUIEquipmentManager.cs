using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerUIEquipmentManager : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] GameObject menu;

    [Header("Weapon Slots")]
    [SerializeField] Image rightHandSlot01;
    [SerializeField] Image rightHandSlot02;
    [SerializeField] Image rightHandSlot03;

    [Header("Equipment Inventory")]
    public EquipmentType currentSelectedEquipmentSlot;
    [SerializeField] GameObject equipmentInventoryWindow;
    [SerializeField] GameObject equipmentInventorySlotPrefab;
    [SerializeField] Transform equipmentInventoryContentWindow;
    [SerializeField] Item currentSelectedItem;

    public void OpenEquipmentMenu()
    {
        // menu.SetActive(false);
        PlayerUIManager.instance.menuWindowIsOpen = true;
        menu.SetActive(true);
        equipmentInventoryWindow.SetActive(false);
        RefreshMenu();
    }

    public void RefreshMenu()
    {
        ClearEquipmentInventory();
        RefreshWeaponSlotIcons();
    }

    public void SelectLastSelectedEquipmentSlot() // go back to last equipment slot after equipping item
    {
        Button lastSelectedButton = null;

        switch (currentSelectedEquipmentSlot)
        {
            case EquipmentType.RightWeapon01:
                lastSelectedButton = rightHandSlot01.GetComponentInParent<Button>();
                break;
            case EquipmentType.RightWeapon02:
                lastSelectedButton = rightHandSlot02.GetComponentInParent<Button>();
                break;
            case EquipmentType.RightWeapon03:
                lastSelectedButton = rightHandSlot03.GetComponentInParent<Button>();
                break;
            default:
                break;
        }

        if (lastSelectedButton != null)
        {
            lastSelectedButton.Select();
            lastSelectedButton.OnSelect(null);
        }
    }

    public void CloseEquipmentMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = false;
        menu.SetActive(false);
    }

    private void RefreshWeaponSlotIcons()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

        WeaponItem rightHandWeapon01 = player.playerInventoryManager.weaponsInRightHandSlots[0];
        WeaponItem rightHandWeapon02 = player.playerInventoryManager.weaponsInRightHandSlots[1];
        WeaponItem rightHandWeapon03 = player.playerInventoryManager.weaponsInRightHandSlots[2];

        if (rightHandWeapon01.itemIcon != null)
        {
            rightHandSlot01.enabled = true;
            rightHandSlot01.sprite = rightHandWeapon01.itemIcon;
        }
        else
        {
            rightHandSlot01.enabled = false;
        }
        if (rightHandWeapon02.itemIcon != null)
        {
            rightHandSlot02.enabled = true;
            rightHandSlot02.sprite = rightHandWeapon02.itemIcon;
        }
        else
        {
            rightHandSlot02.enabled = false;
        }
        if (rightHandWeapon03.itemIcon != null)
        {
            rightHandSlot03.enabled = true;
            rightHandSlot03.sprite = rightHandWeapon03.itemIcon;
        }
        else
        {
            rightHandSlot03.enabled = false;
        }
    }

    private void ClearEquipmentInventory()
    {
        foreach (Transform item in equipmentInventoryContentWindow)
        {
            Destroy(item.gameObject);
        }
    }

    public void LoadEquipmentInventory()
    {
        // if (equipmentInventoryWindow.activeSelf)
        // {
        //    return;
        // }

        equipmentInventoryWindow.SetActive(true);

        switch (currentSelectedEquipmentSlot)
        {
            case EquipmentType.RightWeapon01:
                LoadWeaponInventory();
                break;
            case EquipmentType.RightWeapon02:
                LoadWeaponInventory();
                break;
            case EquipmentType.RightWeapon03:
                LoadWeaponInventory();
                break;
            default:
                break;
        }
    }

    private void LoadWeaponInventory()
    {
        List<WeaponItem> weaponsInInventory = new List<WeaponItem>();

        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

        for (int i = 0; i < player.playerInventoryManager.itemsInInventory.Count; i++)
        {
            WeaponItem weapon = player.playerInventoryManager.itemsInInventory[i] as WeaponItem;

            if (weapon != null)
            {
                weaponsInInventory.Add(weapon);
            }
        }

        if (weaponsInInventory.Count <= 0)
        {
            // equipmentInventoryWindow.SetActive(false); // also valid
            RefreshMenu();
            return;
        }

        bool hasSelectedFirstInventorySlot = false;

        for (int i = 0; i < weaponsInInventory.Count; i++)
        {
            GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, equipmentInventoryContentWindow);
            UI_Equipment_Inventory_Slot equipmentInventorySlot = inventorySlotGameObject.GetComponent<UI_Equipment_Inventory_Slot>();
            equipmentInventorySlot.AddItem(weaponsInInventory[i]);

            if (!hasSelectedFirstInventorySlot)
            {
                hasSelectedFirstInventorySlot = true;
                Button inventorySlotButton = inventorySlotGameObject.GetComponent<Button>();
                inventorySlotButton.Select();
                inventorySlotButton.OnSelect(null);
            }
        }
    }

    public void SelectEquipmentSlot(int equipmentSlot)
    {
        currentSelectedEquipmentSlot = (EquipmentType) equipmentSlot;
    }

    public void UnEquipSelectedItem()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

        Item unequippedItem;

        switch (currentSelectedEquipmentSlot)
        {
            case EquipmentType.RightWeapon01:
                unequippedItem = player.playerInventoryManager.weaponsInRightHandSlots[0];

                if (unequippedItem != null)
                {
                    player.playerInventoryManager.weaponsInRightHandSlots[0] = Instantiate(WorldItemDatabase.instance.unarmedWeapon);
                    
                    if (unequippedItem.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID) // do not add to inventory if unarmed weapon
                    {
                        player.playerInventoryManager.AddItemToInventory(unequippedItem);
                    }
                }

                if (player.playerInventoryManager.rightHandWeaponIndex == 0) // if you were holding the weapon you just unequipped
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDatabase.instance.unarmedWeapon.itemID;
                }

                break;
            case EquipmentType.RightWeapon02:
                unequippedItem = player.playerInventoryManager.weaponsInRightHandSlots[1];

                if (unequippedItem != null)
                {
                    player.playerInventoryManager.weaponsInRightHandSlots[1] = Instantiate(WorldItemDatabase.instance.unarmedWeapon);

                    if (unequippedItem.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(unequippedItem);
                    }
                }

                if (player.playerInventoryManager.rightHandWeaponIndex == 1)
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDatabase.instance.unarmedWeapon.itemID;
                }

                break;
            case EquipmentType.RightWeapon03:
                unequippedItem = player.playerInventoryManager.weaponsInRightHandSlots[2];

                if (unequippedItem != null)
                {
                    player.playerInventoryManager.weaponsInRightHandSlots[2] = Instantiate(WorldItemDatabase.instance.unarmedWeapon);

                    if (unequippedItem.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(unequippedItem);
                    }
                }

                if (player.playerInventoryManager.rightHandWeaponIndex == 2)
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDatabase.instance.unarmedWeapon.itemID;
                }

                break;
            default:
                break;
        }

        RefreshMenu(); // to refresh
    }
}
