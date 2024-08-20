using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class CharacterNetworkManager : NetworkBehaviour
{
    CharacterManager character;

    [Header("Active")]
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Position")]
    public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero,  NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public Vector3 networkPositionVelocity;
    public float networkPositionSmoothTime = 0.1f;
    public float networkRotationSmoothTime = 0.1f;

    [Header("Animator")]
    public NetworkVariable<bool> isMoving = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> verticalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> horizontalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> moveAmount = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Target")]
    public NetworkVariable<ulong> currentTargetNetworkID = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Flags")]
    public NetworkVariable<bool> isInvulnerable = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isPoiseInvulnerable = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isLockedOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isChargingAttack = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Stats - VIT")]
    public NetworkVariable<int> vitality = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(20, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxHealth = new NetworkVariable<int>(20, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Stats - END")]
    public NetworkVariable<int> endurance = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> currentStamina = new NetworkVariable<float>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxStamina = new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void CheckHealth(int oldValue, int newValue)
    {
        if (currentHealth.Value <= 0)
        {
            StartCoroutine(character.ProcessDeathEvent());
        }

        // to prevent over-healing
        if (character.IsOwner)
        {
            if (currentHealth.Value > maxHealth.Value)
            {
                currentHealth.Value = maxHealth.Value;
            }
        }
    }

    public void OnLockOnTargetIDChange(ulong oldID, ulong newID)
    {
        if (!IsOwner)
        {
            character.characterCombatManager.currentTarget = NetworkManager.Singleton.SpawnManager.SpawnedObjects[newID].gameObject.GetComponent<CharacterManager>();
        }

    }

    public void OnIsLockedOnChanged(bool old, bool isLockedOn)
    {
        if (!isLockedOn)
        {
            character.characterCombatManager.currentTarget = null;
        }
    }

    public void OnIsChargingAttackChanged(bool old, bool newStatus)
    {
        character.animator.SetBool("IsChargingAttack", newStatus);
    }

    public void OnIsMovingChanged(bool old, bool newStatus)
    {
        character.animator.SetBool("IsMoving", isMoving.Value);
    }

    public virtual void OnIsActiveChanged(bool old, bool newStatus)
    {
        gameObject.SetActive(isActive.Value);
    }

    [ServerRpc] // Function called from a client to the server (host)
    public void NotifyTheServerOfActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
    {
        if (IsServer) // If this character is the host/server, then activate the client RPC
        {
            PlayActionAnimationForAllClientsClientRpc(clientID, animationID, applyRootMotion);
        }
    }

    [ClientRpc] // Client RPC is sent to all clients present from the server
    public void PlayActionAnimationForAllClientsClientRpc(ulong clientID, string animationID, bool applyRootMotion)
    {
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformActionAnimationFromServers(animationID, applyRootMotion);
        }
    }

    private void PerformActionAnimationFromServers(string animationID, bool applyRootMotion)
    {
        character.characterAnimatorManager.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(animationID, 0.2f);
    }

    [ServerRpc]
    public void NotifyTheServerOfAttackActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
    {
        if (IsServer)
        {
            PlayAttackActionAnimationForAllClientsClientRpc(clientID, animationID, applyRootMotion);
        }
    }

    [ClientRpc]
    public void PlayAttackActionAnimationForAllClientsClientRpc(ulong clientID, string animationID, bool applyRootMotion)
    {
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformAttackActionAnimationFromServers(animationID, applyRootMotion);
        }
    }

    private void PerformAttackActionAnimationFromServers(string animationID, bool applyRootMotion)
    {
        character.characterAnimatorManager.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(animationID, 0.2f);
    }

    [ServerRpc(RequireOwnership = false)]
    public void NotifyTheServerOfCharacterDamageServerRpc(ulong damagedCharacterID, ulong characterCausingDamageID,
        float physicalDamage, float magicDamage, float poiseDamage,
        float angleHitFrom, float contactPointX, float contantPointY, float contantPointZ)
    {
        if (IsServer)
        {
            NotifyTheServerOfCharacterDamageClientRpc(damagedCharacterID, characterCausingDamageID,
                physicalDamage, magicDamage, poiseDamage,
                angleHitFrom, contactPointX, contantPointY, contantPointZ);
        }
    }

    [ClientRpc]
    public void NotifyTheServerOfCharacterDamageClientRpc(ulong damagedCharacterID, ulong characterCausingDamageID,
        float physicalDamage, float magicDamage, float poiseDamage,
        float angleHitFrom, float contactPointX, float contantPointY, float contantPointZ)
    {
        ProcessCharacterDamageFromServer(damagedCharacterID, characterCausingDamageID,
                physicalDamage, magicDamage, poiseDamage,
                angleHitFrom, contactPointX, contantPointY, contantPointZ);
    }

    public void ProcessCharacterDamageFromServer(ulong damagedCharacterID, ulong characterCausingDamageID,
        float physicalDamage, float magicDamage, float poiseDamage,
        float angleHitFrom, float contactPointX, float contantPointY, float contantPointZ)
    {
        CharacterManager damagedCharacter = NetworkManager.Singleton.SpawnManager.SpawnedObjects[damagedCharacterID].gameObject.GetComponent<CharacterManager>();
        CharacterManager characterCausingDamage = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingDamageID].gameObject.GetComponent<CharacterManager>();

        TakeHealthDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);

        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.angleHitFrom = angleHitFrom;
        damageEffect.contactPoint = new Vector3(contactPointX, contantPointY, contantPointZ);
        damageEffect.characterCausingDamage = characterCausingDamage;

        damagedCharacter.characterEffectsManager.ProcessInstantEffect(damageEffect);
    }
}
