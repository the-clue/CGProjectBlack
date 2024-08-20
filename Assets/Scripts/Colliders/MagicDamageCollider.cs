using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicDamageCollider : DamageCollider
{
    [SerializeField] CharacterManager characterCausingDamage;

    protected override void Awake()
    {
        base.Awake();

        if (damageCollider == null)
        {
            damageCollider = GetComponent<Collider>();
        }
        if (characterCausingDamage == null)
        {
            characterCausingDamage = GetComponentInParent<CharacterManager>();
        }
    }

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        // Can magic damage self?
        if (damageTarget == characterCausingDamage)
        {
            return;
        }

        if (charactersDamaged.Contains(damageTarget))
        {
            return;
        }

        charactersDamaged.Add(damageTarget);

        TakeHealthDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.contactPoint = contactPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(transform.forward, damageTarget.transform.forward, Vector3.up);

        /*
        // Option 1: deal damage if AI hits target on the host's side, regardless of how it looks on any client
        if (bossCharacter.IsOwner)
        {
            damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(damageTarget.NetworkObjectId,
                bossCharacter.NetworkObjectId, damageEffect.physicalDamage, damageEffect.magicDamage, damageEffect.poiseDamage,
                damageEffect.angleHitFrom, damageEffect.contactPoint.x, damageEffect.contactPoint.y, damageEffect.contactPoint.z);
        }
        */

        // Option 2: deal damage if AI hits target on the connected character's side, regardless of how it looks on any client
        if (damageTarget.IsOwner)
        {
            damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(damageTarget.NetworkObjectId,
                characterCausingDamage.NetworkObjectId, damageEffect.physicalDamage, damageEffect.magicDamage, damageEffect.poiseDamage,
                damageEffect.angleHitFrom, damageEffect.contactPoint.x, damageEffect.contactPoint.y, damageEffect.contactPoint.z);
        }

        // damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
    }

    public void SetCharacterCausingDamage(CharacterManager character)
    {
        characterCausingDamage = character;
    }
}
