using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Equipment_Inventory_Slot : MonoBehaviour
{
    public Image itemIcon;
    public Image highlightedIcon;

    [SerializeField] public Item currentItem;

    public void AddItem(Item item)
    {
        if (item == null)
        {
            itemIcon.enabled = false;
            return;
        }

        itemIcon.enabled = true;

        currentItem = item;

        itemIcon.sprite = item.itemIcon;
    }

    public void SelectSlot()
    {
        highlightedIcon.enabled = true;
    }

    public void DeselectSlot()
    {
        highlightedIcon.enabled = false;
    }

    public void EquipItem()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

        WeaponItem currentWeapon;

        switch (PlayerUIManager.instance.playerUIEquipmentManager.currentSelectedEquipmentSlot)
        {
            case EquipmentType.RightWeapon01:
                currentWeapon = player.playerInventoryManager.weaponsInRightHandSlots[0];
                if (currentWeapon.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    player.playerInventoryManager.AddItemToInventory(currentWeapon);
                }

                player.playerInventoryManager.weaponsInRightHandSlots[0] = currentItem as WeaponItem;
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                if (player.playerInventoryManager.rightHandWeaponIndex == 0) // re-equip weapon if holding weapon in this slot
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = currentItem.itemID;
                }

                PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu(); // refresh equipment window

                break;
            case EquipmentType.RightWeapon02:
                currentWeapon = player.playerInventoryManager.weaponsInRightHandSlots[1];
                if (currentWeapon.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    player.playerInventoryManager.AddItemToInventory(currentWeapon);
                }
                player.playerInventoryManager.weaponsInRightHandSlots[1] = currentItem as WeaponItem;
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);
                if (player.playerInventoryManager.rightHandWeaponIndex == 1)
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = currentItem.itemID;
                }
                PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();
                break;
            case EquipmentType.RightWeapon03:
                currentWeapon = player.playerInventoryManager.weaponsInRightHandSlots[2];
                if (currentWeapon.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    player.playerInventoryManager.AddItemToInventory(currentWeapon);
                }
                player.playerInventoryManager.weaponsInRightHandSlots[2] = currentItem as WeaponItem;
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                if (player.playerInventoryManager.rightHandWeaponIndex == 2)
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = currentItem.itemID;
                }
                PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();
                break;
            default:
                break;
        }

        PlayerUIManager.instance.playerUIEquipmentManager.SelectLastSelectedEquipmentSlot();
    }
}
