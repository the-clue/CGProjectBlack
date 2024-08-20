using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    public Collider damageCollider;

    [Header("Damage")]
    public float physicalDamage = 0;
    public float magicDamage = 0;
    public float poiseDamage = 0;

    [Header("Contact Point")]
    public Vector3 contactPoint;

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

    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public virtual void DisableDamageCollider()
    {
        damageCollider.enabled = false;
        charactersDamaged.Clear(); // to reset characters that have been hit
    }
}
