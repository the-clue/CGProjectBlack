using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Interactable : NetworkBehaviour
{
    public string interactableText;
    [SerializeField] protected Collider interactableCollider;
    [SerializeField] protected bool hostOnlyInteractable = true;

    protected virtual void Awake()
    {
        if (interactableCollider == null)
        {
            interactableCollider = GetComponent<Collider>();
        }
    }

    protected virtual void Start()
    {

    }

    public virtual void Interact(PlayerManager player)
    {
        Debug.Log("You have interacted!");

        if (!player.IsOwner)
        {
            return;
        }

        interactableCollider.enabled = false;
        player.playerInteractionManager.RemoveInteractionFromList(this);
        PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();

        if (player != null)
        {
            if (!player.playerNetworkManager.IsHost && hostOnlyInteractable)
            {
                return;
            }

            if (!player.IsOwner)
            {
                return;
            }

            // pass interaction to player
            player.playerInteractionManager.AddInteractionToList(this);
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();

        if (player != null)
        {
            if (!player.playerNetworkManager.IsHost && hostOnlyInteractable)
            {
                return;
            }

            if (!player.IsOwner)
            {
                return;
            }

            // remove interaction from player
            player.playerInteractionManager.RemoveInteractionFromList(this);

            PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
        }
    }
}
