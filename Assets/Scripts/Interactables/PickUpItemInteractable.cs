using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PickUpItemInteractable : Interactable
{
    // two types of item pick ups:
    // 1. World Item: drop once, unique id -> one per save data
    // 2. Droppables: sometimes dropped by enemies/characters

    public ItemPickUpType pickUpType;

    [Header("Item")]
    [SerializeField] Item item;

    [Header("World Spawn Pick Up")]
    [SerializeField] int worldItemID;
    [SerializeField] bool hasBeenLooted = false;

    protected override void Start()
    {
        base.Start();

        if (pickUpType == ItemPickUpType.WorldSpawn)
        {
            CheckIfWorldItemIsLooted();
        }
    }

    private void CheckIfWorldItemIsLooted()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.ContainsKey(worldItemID))
        {
            WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Add(worldItemID, false);
        }
        
        hasBeenLooted = WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted[worldItemID];

        if (hasBeenLooted)
        {
            gameObject.SetActive(false);
        }
    }

    public override void Interact(PlayerManager player)
    {
        base.Interact(player);

        // Play SFX
        player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.pickUpItemSFX, 2);

        StatusItem statusItem = item as StatusItem;
        if (statusItem != null)
        {
            player.AddStatusPoints(statusItem.statusType, statusItem.statusPoints);
        }
        else
        {
            player.playerInventoryManager.AddItemToInventory(item);
        }
        
        // Play UI Pop Up
        PlayerUIManager.instance.playerUIPopUpManager.SendItemPopUp(item);

        // Save loot status
        if (pickUpType == ItemPickUpType.WorldSpawn)
        {
            if (WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.ContainsKey(worldItemID))
            {
                WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Remove(worldItemID);
            }
            
            WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Add(worldItemID, true);
        }

        WorldSaveGameManager.instance.SaveGameWithoutPosition();

        // Hide or destroy game object
        Destroy(gameObject);
    }
}
