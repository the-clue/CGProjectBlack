using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ImpactDamageCollider : DamageCollider
{
    private CharacterManager characterCausingDamage;
    private float impactDamage;
    private float impactPoiseDamage;
    private GameObject impactVFX;
    private AudioClip impactSFX;
    private float colliderSize;
    private float colliderHeight;
    private float groundY;

    protected override void Awake()
    {
        base.Awake();
    }

    // Function that sets every variable of the impact damage collider
    public void SetImpactDamageCollider(CharacterManager characterCausingDamage, float impactDamage, float impactPoiseDamage,
        GameObject impactVFX, AudioClip impactSFX, float colliderSize, float colliderHeight, float groundY = -1000)
    {
        this.characterCausingDamage = characterCausingDamage;
        this.impactDamage = impactDamage;
        this.impactPoiseDamage = impactPoiseDamage;
        this.impactVFX = impactVFX;
        this.impactSFX = impactSFX;
        this.colliderSize = colliderSize;
        this.colliderHeight = colliderHeight;
        this.groundY = groundY;
    }

    // Function that creates two box damage colliders forming a Star of David
    public void ActivateImpact()
    {
        Vector3 position = transform.position;
        if (groundY != -1000) // then consider the given ground level so that the impact happens on the ground
        {
            float adjustedHeight = groundY;
            position = new Vector3(transform.position.x, adjustedHeight, transform.position.z);
        }

        if (impactVFX != null)
        {
            _ = Instantiate(impactVFX, position, Quaternion.identity);
            //_ = Instantiate(impactVFX, transform.position, Quaternion.identity);
        }
        if (impactSFX != null)
        {
            characterCausingDamage.characterSoundFXManager.PlaySoundFX(impactSFX);
        }

        Vector3 colliderBoxDimensions = new(colliderSize, colliderHeight, colliderSize);
        Collider[] colliderBox1 =
            Physics.OverlapBox(position, colliderBoxDimensions,
            Quaternion.Euler(0, transform.rotation.y, 0),
            WorldUtilityManager.instance.GetCharacterLayers());
        Collider[] colliderBox2 = Physics.OverlapBox(position, colliderBoxDimensions,
            Quaternion.Euler(0, transform.rotation.y, 0) * Quaternion.Euler(0, 45, 0),
            WorldUtilityManager.instance.GetCharacterLayers());

        List<CharacterManager> charactersDamaged = new();

        List<Collider[]> collidersList = new()
        {
            colliderBox1,
            colliderBox2
        };

        foreach (var c in collidersList)
        {
            foreach (var collider in c)
            {
                CharacterManager characterAboutToBeDamaged = collider.GetComponentInParent<CharacterManager>();

                if (characterAboutToBeDamaged != null)
                {
                    if (charactersDamaged.Contains(characterAboutToBeDamaged))
                    {
                        continue;
                    }

                    if (characterAboutToBeDamaged == characterCausingDamage)
                    {
                        continue;
                    }

                    charactersDamaged.Add(characterAboutToBeDamaged);

                    if (characterAboutToBeDamaged.IsOwner)
                    {
                        TakeHealthDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
                        damageEffect.physicalDamage = impactDamage;
                        damageEffect.poiseDamage = impactPoiseDamage;
                        damageEffect.contactPoint = contactPoint;
                        damageEffect.angleHitFrom = Vector3.SignedAngle(transform.forward, characterAboutToBeDamaged.transform.forward, Vector3.up);

                        // characterAboutToBeDamaged.characterEffectsManager.ProcessInstantEffect(damageEffect);
                        if (characterAboutToBeDamaged.IsOwner)
                        {
                            characterAboutToBeDamaged.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(characterAboutToBeDamaged.NetworkObjectId,
                                characterCausingDamage.NetworkObjectId, damageEffect.physicalDamage, damageEffect.magicDamage, damageEffect.poiseDamage,
                                damageEffect.angleHitFrom, damageEffect.contactPoint.x, damageEffect.contactPoint.y, damageEffect.contactPoint.z);
                        }
                    }
                }
            }
        }
    }
}
