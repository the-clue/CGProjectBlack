using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FogWallInteractable : Interactable
{
    [Header("Fog")]
    [SerializeField] GameObject[] fogWallObjects;
    [SerializeField] Collider fogWallCollider;

    [Header("ID")]
    public int fogWallID;

    [Header("Sound")]
    [SerializeField] AudioClip fogWallSFX;
    private AudioSource fogWallAudioSource;

    [Header("Active")]
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected override void Awake()
    {
        base.Awake();

        fogWallAudioSource = GetComponent<AudioSource>();
    }

    public override void Interact(PlayerManager player)
    {
        base.Interact(player);

        Quaternion targetRotation = transform.localRotation;
        // Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward);
        player.transform.rotation = targetRotation;

        AllowPlayerThroughFogWallCollidersServerRpc(player.NetworkObjectId);
        player.playerAnimatorManager.PlayTargetActionAnimation("Enter_Fog_01", true);

        AIBossCharacterManager bossToActivate = WorldAIManager.instance.GetBossCharacterByID(fogWallID);
        bossToActivate.ActivateFight();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        OnIsActiveChanged(false, isActive.Value);
        isActive.OnValueChanged += OnIsActiveChanged;

        WorldObjectManager.instance.AddFogWallToList(this);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        isActive.OnValueChanged -= OnIsActiveChanged;

        WorldObjectManager.instance.RemoveFogWallToList(this);
    }

    private void OnIsActiveChanged(bool oldStatus, bool newStatus)
    {
        if (isActive.Value)
        {
            foreach(var fogObject in fogWallObjects)
            {
                fogObject.SetActive(true);
            }
        }
        else
        {
            foreach(var fogObject in fogWallObjects)
            {
                fogObject.SetActive(false);
            }
        }
    }

    // When a server rpc doesn't require ownership, clients (non-hosts) can activate the function
    [ServerRpc(RequireOwnership = false)]
    private void AllowPlayerThroughFogWallCollidersServerRpc(ulong playerObjectID)
    {
        if (IsServer)
        {
            AllowPlayerThroughFogWallCollidersClientRpc(playerObjectID);
        }
    }
    [ClientRpc]
    private void AllowPlayerThroughFogWallCollidersClientRpc(ulong playerObjectID)
    {
        PlayerManager player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].GetComponent<PlayerManager>();

        fogWallAudioSource.PlayOneShot(fogWallSFX);

        if (player != null)
        {
            StartCoroutine(DisableCollisionForTime(player));
        }
    }

    private IEnumerator DisableCollisionForTime(PlayerManager player)
    {
        Physics.IgnoreCollision(player.characterController, fogWallCollider, true);
        yield return new WaitForSeconds(1.14f); // Change this to your walking animation length
        Physics.IgnoreCollision(player.characterController, fogWallCollider, false);
        interactableCollider.enabled = true;
    }
}
