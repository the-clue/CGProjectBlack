using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterCombatManager : MonoBehaviour
{
    protected CharacterManager character;

    [Header("Last Attack Animation Performed")]
    public string lastAttackAnimationPerformed;

    [Header("Attack Target")]
    public CharacterManager currentTarget;

    [Header("Attack Type")]
    public AttackType currentAttackType;

    [Header("Lock-On Transform")]
    public Transform lockOnTransform;

    [Header("Attack Flags")]
    public bool canDoDodgeAttack = false;
    public bool canDoDuckAttack = false;
    public bool canDoJumpAttack = false;
    public bool isDoingJumpattack = false;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void SetTarget(CharacterManager newTarget)
    {
        if (character.IsOwner)
        {
            if (newTarget != null)
            {
                currentTarget = newTarget;
                character.characterNetworkManager.currentTargetNetworkID.Value = newTarget.GetComponent<NetworkObject>().NetworkObjectId;
            }
            else
            {
                currentTarget = null;
            }
        }
    }

    public void EnableIsInvulnerable()
    {
        if (character.IsOwner)
        {
            character.characterNetworkManager.isInvulnerable.Value = true;
        }
    }
    public void DisableIsInvulnerable()
    {
        character.characterNetworkManager.isInvulnerable.Value = false;
    }

    public void EnableIsPoiseInvulnerable()
    {
        if (character.IsOwner)
        {
            character.characterNetworkManager.isPoiseInvulnerable.Value = true;
        }
    }
    public void DisableIsPoiseInvulnerable()
    {
        character.characterNetworkManager.isPoiseInvulnerable.Value = false;
    }

    public virtual void EnableCanDoCombo()
    {
    }
    public virtual void DisableCanDoCombo()
    {
    }

    public void EnableCanDoDodgeAttack()
    {
        canDoDodgeAttack = true;
    }
    public void DisableCanDoDodgeAttack()
    {
        canDoDodgeAttack = false;
    }

    public void EnableCanDoDuckAttack()
    {
        canDoDuckAttack = true;
    }
    public void DisableCanDoDuckAttack()
    {
        canDoDuckAttack = false;
    }

    public void EnableCanDoJumpAttack()
    {
        canDoJumpAttack = true;
    }
    public void DisableCanDoJumpAttack()
    {
        canDoJumpAttack = false;
    }
}
