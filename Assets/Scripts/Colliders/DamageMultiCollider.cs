using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMultiCollider : MonoBehaviour
{
    [Header("Collider")]
    [SerializeField] protected Collider[] damageColliders;

    [Header("Damage")]
    [SerializeField] protected float physicalDamage = 0;
    [SerializeField] protected float magicDamage = 0;
    [SerializeField] protected float poiseDamage = 0;

    [Header("Contact Point")]
    protected Vector3 contactPoint;

    [Header("Characters Damaged")]
    protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

    protected virtual void Awake()
    {

    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

        if (damageTarget != null)
        {
            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            DamageTarget(damageTarget);
        }
    }

    protected virtual void DamageTarget(CharacterManager damageTarget)
    {
        if (charactersDamaged.Contains(damageTarget))
        {
            return;
        }

        charactersDamaged.Add(damageTarget);

        TakeHealthDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.poiseDamage = poiseDamage;

        damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
    }

    public virtual void SetDamage(float physical, float magic, float poise)
    {
        physicalDamage = physical;
        magicDamage = magic;
        poiseDamage = poise;
    }

    public virtual void EnableDamageColliders()
    {
        foreach (Collider collider in damageColliders)
        {
            collider.enabled = true;
        }
    }

    public virtual void DisableDamageColliders()
    {
        foreach (Collider collider in damageColliders)
        {
            collider.enabled = false;
        }

        charactersDamaged.Clear();
    }
}
