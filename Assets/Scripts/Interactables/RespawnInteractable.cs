using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnInteractable : Interactable
{
    [Header("Respawn Info")]
    [SerializeField] int respawnID;
    public NetworkVariable<bool> isActivated = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("VFX")]
    [SerializeField] GameObject activatedParticles;

    [Header("Interaction Text")]
    string unactivatedInteractionText = "Summon Orb of Time";
    string activatedInteractionText = "Rest";

    protected override void Start()
    {
        base.Start();

        if (IsOwner)
        {
            if (WorldSaveGameManager.instance.currentCharacterData.respawnPoints.ContainsKey(respawnID))
            {
                isActivated.Value = WorldSaveGameManager.instance.currentCharacterData.respawnPoints[respawnID];
            }
            else
            {
                isActivated.Value = false;
            }
        }

        if (isActivated.Value)
        {
            interactableText = activatedInteractionText;
        }
        else
        {
            interactableText = unactivatedInteractionText;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) // to force the onchange function to run when a client joins
        {
            OnIsActivatedChanged(false, isActivated.Value);
        }

        isActivated.OnValueChanged += OnIsActivatedChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        isActivated.OnValueChanged -= OnIsActivatedChanged;
    }

    private void RestoreRespawn(PlayerManager player)
    {
        isActivated.Value = true;

        // If save files contains info on respawn point, then remove it
        if (WorldSaveGameManager.instance.currentCharacterData.respawnPoints.ContainsKey(respawnID))
        {
            WorldSaveGameManager.instance.currentCharacterData.respawnPoints.Remove(respawnID);
        }

        // And add it with status of true
        WorldSaveGameManager.instance.currentCharacterData.respawnPoints.Add(respawnID, true);

        player.playerAnimatorManager.PlayTargetActionAnimation("Activate_Respawn_01", true);

        PlayerUIManager.instance.playerUIPopUpManager.SendGeneralPopUp("Orb of Time created");

        StartCoroutine(WaitForAnimationAndPopUpThenRestoreCollider());
    }

    private void RestAtRespawn(PlayerManager player)
    {
        // temporary, to change when the menu has been added
        interactableCollider.enabled = true;
        player.playerNetworkManager.currentHealth.Value = player.playerNetworkManager.maxHealth.Value;
        player.playerNetworkManager.currentStamina.Value = player.playerNetworkManager.maxStamina.Value;

        // player.playerAnimatorManager.PlayTargetActionAnimation("Rest_Respawn_01", true);

        WorldAIManager.instance.ResetAllCharacters();
    }

    private IEnumerator WaitForAnimationAndPopUpThenRestoreCollider()
    {
        yield return new WaitForSeconds(2);
        interactableCollider.enabled = true;
    }

    public override void Interact(PlayerManager player)
    {
        base.Interact(player);

        if (!isActivated.Value)
        {
            RestoreRespawn(player);
        }
        else
        {
            RestAtRespawn(player);
        }
    }

    private void OnIsActivatedChanged(bool oldStatus, bool newStatus)
    {
        if (isActivated.Value)
        {
            activatedParticles.SetActive(true);
            interactableText = activatedInteractionText;
        }
        else
        {
            interactableText = unactivatedInteractionText;
        }
    }
}
