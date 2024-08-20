using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletonImpactDamageCollider : DamageCollider
{
    [SerializeField] AISkeletonCharacterManager skeletonManager;

    protected override void Awake()
    {
        base.Awake();

        skeletonManager = GetComponentInParent<AISkeletonCharacterManager>();
    }

    public void ImpactAttack()
    {
        _ = Instantiate(skeletonManager.skeletonCombatManager.skeletonImpactVFX, transform.position, Quaternion.identity);
        skeletonManager.skeletonSoundFXManager.PlayImpactSFX();

        // Collider[] colliders = Physics.OverlapSphere(transform.position, skeletonManager.skeletonCombatManager.impactRadius, WorldUtilityManager.instance.GetCharacterLayers());
        // To form a "star" collider instead of considering a sphere
        Vector3 colliderBox = new Vector3(2.5f, 0.2f, 2.5f);
        Collider[] collidersBoxStraight = Physics.OverlapBox(transform.position, colliderBox, Quaternion.Euler(0, transform.rotation.y, 0), WorldUtilityManager.instance.GetCharacterLayers());
        Collider[] collidersBoxAngled = Physics.OverlapBox(transform.position, colliderBox, Quaternion.Euler(0, transform.rotation.y, 0) * Quaternion.Euler(0, 45, 0), WorldUtilityManager.instance.GetCharacterLayers());

        List<CharacterManager> charactersDamaged = new List<CharacterManager>();

        foreach (var collider in collidersBoxStraight)
        {
            CharacterManager character = collider.GetComponentInParent<CharacterManager>();

            if (character != null)
            {
                if (charactersDamaged.Contains(character))
                {
                    continue;
                }

                // So the skeleton doesn't hurt himself
                if (character == skeletonManager)
                {
                    continue;
                }

                charactersDamaged.Add(character);

                // so it only triggers if hit in your screen
                if (character.IsOwner)
                {
                    TakeHealthDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
                    damageEffect.physicalDamage = skeletonManager.skeletonCombatManager.impactDamage;
                    damageEffect.poiseDamage = skeletonManager.skeletonCombatManager.impactDamage;

                    character.characterEffectsManager.ProcessInstantEffect(damageEffect);
                }
            }
        }

        foreach (var collider in collidersBoxAngled)
        {
            CharacterManager character = collider.GetComponentInParent<CharacterManager>();

            if (character != null)
            {
                if (charactersDamaged.Contains(character))
                {
                    continue;
                }

                // So the skeleton doesn't hurt himself
                if (character == skeletonManager)
                {
                    continue;
                }

                charactersDamaged.Add(character);

                // so it only triggers if hit in your screen
                if (character.IsOwner)
                {
                    TakeHealthDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
                    damageEffect.physicalDamage = skeletonManager.skeletonCombatManager.impactDamage;
                    damageEffect.poiseDamage = skeletonManager.skeletonCombatManager.impactDamage;

                    character.characterEffectsManager.ProcessInstantEffect(damageEffect);
                }
            }
        }
    }
}
