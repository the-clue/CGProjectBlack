using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBulletDamageCollider : DamageCollider
{
    [SerializeField] CharacterManager characterBulletCaster;
    [SerializeField] CharacterManager characterTarget;
    [SerializeField] bool isHoming = false;
    [SerializeField] float movementSpeed = 7.5f;
    [SerializeField] float angleChangingSpeed = 2.5f;
    [SerializeField] Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isHoming)
        {
            if (characterTarget != null)
            {
                Vector3 direction = (characterTarget.transform.position - transform.position).normalized;
                float rotateAmount = transform.InverseTransformDirection(Vector3.Cross(direction, transform.up)).z;

                rb.angularVelocity = new Vector3(0, angleChangingSpeed * rotateAmount, 0);
                rb.velocity = transform.forward * movementSpeed;
            }
        }
    }

    public void SetCharacterBulletCaster(CharacterManager caster)
    {
        characterBulletCaster = caster;
    }
    public void SetTarget(CharacterManager target)
    {
        characterTarget = target;
    }
    public void SetIsHoming(bool value)
    {
        isHoming = value;
    }
    public void SetSpeedAndRotation(float speed, float rotationSpeed)
    {
        movementSpeed = speed;
        angleChangingSpeed = rotationSpeed;
    }

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        if (damageTarget == characterBulletCaster)
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
                characterBulletCaster.NetworkObjectId, damageEffect.physicalDamage, damageEffect.magicDamage, damageEffect.poiseDamage,
                damageEffect.angleHitFrom, damageEffect.contactPoint.x, damageEffect.contactPoint.y, damageEffect.contactPoint.z);
        }

        // damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
    }
}

