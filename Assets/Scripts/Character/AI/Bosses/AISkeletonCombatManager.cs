using UnityEngine;

public class AISkeletonCombatManager : AICharacterCombatManager
{
    AISkeletonCharacterManager aiSkeletonManager;

    [Header("Damage Colliders")]
    [SerializeField] SkeletonDamageCollider leftHandDamageCollider;
    [SerializeField] SkeletonDamageCollider rightHandDamageCollider;
    [SerializeField] SkeletonDamageCollider auraDamageCollider;
    [SerializeField] SkeletonDamageCollider auraBiggerDamageCollider;
    [SerializeField] SkeletonImpactDamageCollider impactDamageCollider;
    [SerializeField] ImpactDamageCollider leftFootStompImpactDamageCollider;

    public float impactRadius = 2f;

    [Header("Collider Spawn Points")]
    [SerializeField] Transform crossAboveHeadSpawnPoint;
    [SerializeField] Transform crossLeftHandSpawnPoint;
    [SerializeField] Transform crossRightHandSpawnPoint;
    [SerializeField] Transform auraTransform;
    [SerializeField] Transform miniAuraTransform;

    [Header("Damage")]
    [SerializeField] int baseDamage = 50;
    [SerializeField] int basePoiseDamage = 25;
    [SerializeField] float attack01DamageModifier = 1.0f; // Punch
    [SerializeField] float attack02DamageModifier = 1.0f; // Swipe
    [SerializeField] float attack03DamageModifier = 2f; // Jump
    [SerializeField] float attack04DamageModifier = 1.5f; // Aura
    [SerializeField] float attack05DamageModifier = 1.5f; // Cross above head
    [SerializeField] float attack06DamageModifier = 1.0f; // Left Foot Stomp
    [HideInInspector] public float impactDamage;

    [Header("Attack Parameters")]
    [SerializeField] float attack05RangeMagicSpeed = 5f;
    [SerializeField] float attack05RangeMagicAngleSpeed = 5f;
    [SerializeField] int attack05RangeMagicMode = 2;
    [SerializeField] float attack05RangeMagicDelay = 10f;
    [SerializeField] float attack05RangeMagicHomingDuration = 10f;

    [Header("VFX")]
    public GameObject skeletonImpactVFX;
    public GameObject skeletonAuraVFX;
    public GameObject skeletonAuraBiggerVFX;
    public GameObject skeletonMiniAuraVFX;
    public GameObject skeletonCrossVFX;

    protected override void Awake()
    {
        base.Awake();

        if (aiSkeletonManager == null)
        {
            aiSkeletonManager = GetComponent<AISkeletonCharacterManager>();
        }
    }

    public void SetAttack01Damage() // punch right hand
    {
        _ = Instantiate(skeletonMiniAuraVFX, miniAuraTransform);
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        rightHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
        rightHandDamageCollider.poiseDamage = basePoiseDamage * attack01DamageModifier;
    }

    public void SetAttack02Damage() // swipe left hand
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        leftHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
        leftHandDamageCollider.poiseDamage = basePoiseDamage * attack02DamageModifier;
    }

    public void SetAttack03Damage() // jump attack
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        impactDamage = baseDamage * attack03DamageModifier;
    }

    public void SetAttack04Damage() // aura attack
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        auraDamageCollider.physicalDamage = baseDamage * attack04DamageModifier;
        auraDamageCollider.poiseDamage = basePoiseDamage * attack04DamageModifier;
        auraBiggerDamageCollider.physicalDamage = baseDamage * attack04DamageModifier;
        auraBiggerDamageCollider.poiseDamage = basePoiseDamage * attack04DamageModifier;
    }

    public void SetAttack05Damage() // cross above head attack
    {
        float magicDamage = baseDamage * attack05DamageModifier;
        float magicPoiseDamage = basePoiseDamage * attack05DamageModifier;
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        eulerRotation.x = 0; // Set the x axis rotation to 0
        GameObject rangeMagicGameObject = Instantiate(skeletonCrossVFX, crossAboveHeadSpawnPoint.position, Quaternion.Euler(eulerRotation));
        RangeMagicDamageCollider rangeMagicDamageCollider = rangeMagicGameObject.GetComponent<RangeMagicDamageCollider>();
        rangeMagicDamageCollider.SetRangeMagicDamageCollider(aiSkeletonManager, currentTarget, magicDamage, magicPoiseDamage,
            attack05RangeMagicSpeed, attack05RangeMagicAngleSpeed, attack05RangeMagicMode,
            attack05RangeMagicDelay, attack05RangeMagicHomingDuration);
    }

    public void SetAttackCrossLeftHand()
    {
        if (aiSkeletonManager.hasPhaseShifted)
        {
            float magicDamage = baseDamage * attack05DamageModifier;
            float magicPoiseDamage = basePoiseDamage * attack05DamageModifier;
            Vector3 eulerRotation = transform.rotation.eulerAngles;
            eulerRotation.x = 0; // Set the x axis rotation to 0
            GameObject rangeMagicGameObject = Instantiate(skeletonCrossVFX, crossLeftHandSpawnPoint.position, Quaternion.Euler(eulerRotation));
            RangeMagicDamageCollider rangeMagicDamageCollider = rangeMagicGameObject.GetComponent<RangeMagicDamageCollider>();
            rangeMagicDamageCollider.SetRangeMagicDamageCollider(aiSkeletonManager, currentTarget, magicDamage, magicPoiseDamage,
                attack05RangeMagicSpeed * 4, attack05RangeMagicAngleSpeed / 1.5f, 1,
                attack05RangeMagicDelay / 4, attack05RangeMagicHomingDuration / 2);
        }
    }

    public void SetAttackCrossRightHand()
    {
        if (aiSkeletonManager.hasPhaseShifted)
        {
            float magicDamage = baseDamage * attack05DamageModifier;
            float magicPoiseDamage = basePoiseDamage * attack05DamageModifier;
            Vector3 eulerRotation = transform.rotation.eulerAngles;
            eulerRotation.x = 0; // Set the x axis rotation to 0
            GameObject rangeMagicGameObject = Instantiate(skeletonCrossVFX, crossRightHandSpawnPoint.position, Quaternion.Euler(eulerRotation));
            RangeMagicDamageCollider rangeMagicDamageCollider = rangeMagicGameObject.GetComponent<RangeMagicDamageCollider>();
            rangeMagicDamageCollider.SetRangeMagicDamageCollider(aiSkeletonManager, currentTarget, magicDamage, magicPoiseDamage,
                attack05RangeMagicSpeed * 2, attack05RangeMagicAngleSpeed / 2, attack05RangeMagicMode,
                attack05RangeMagicDelay / 2, attack05RangeMagicHomingDuration / 2);
        }
    }

    public void OpenLeftHandDamageCollider()
    {
        aiCharacter.characterSoundFXManager.PlayWhooshes();
        leftHandDamageCollider.EnableDamageCollider();
    }
    public void CloseLeftHandDamageCollider()
    {
        leftHandDamageCollider.DisableDamageCollider();
    }

    public void OpenRightHandDamageCollider()
    {
        aiCharacter.characterSoundFXManager.PlayWhooshes();
        rightHandDamageCollider.EnableDamageCollider();
    }
    public void CloseRightHandDamageCollider()
    {
        rightHandDamageCollider.DisableDamageCollider();
    }

    public void OpenAuraDamageCollider()
    {
        if (aiSkeletonManager.hasPhaseShifted) // Bigger Aura Attack in Phase 2
        {
            _ = Instantiate(skeletonAuraBiggerVFX, auraTransform);
            auraBiggerDamageCollider.EnableDamageCollider();
        }
        else
        {
            _ = Instantiate(skeletonAuraVFX, auraTransform);
            auraDamageCollider.EnableDamageCollider();
        }
        aiSkeletonManager.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(aiSkeletonManager.skeletonSoundFXManager.auraSounds));
    }
    public void CloseAuraDamageCollider()
    {
        auraDamageCollider.DisableDamageCollider();
        auraBiggerDamageCollider.DisableDamageCollider();
    }

    public void ActivateImpact() // You should refactor this soon using the new class for Impact Attacks
    {
        impactDamageCollider.ImpactAttack();
    }

    public void SetAttack06()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        float impactDamage = baseDamage * attack06DamageModifier;
        float impactPoiseDamage = basePoiseDamage * attack06DamageModifier;
        leftFootStompImpactDamageCollider.SetImpactDamageCollider(aiSkeletonManager, impactDamage, impactPoiseDamage,
            skeletonImpactVFX, aiSkeletonManager.skeletonSoundFXManager.impactSounds[0], 2.5f, 0.2f);
    }
    public void ActivateLeftFootStompImpactAttack()
    {
        leftFootStompImpactDamageCollider.ActivateImpact();
    }

    public override void PivotTowardsTarget(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)
        {
            return;
        }

        if (viewableAngle >= 75 && viewableAngle <= 105)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_90", true);
        }
        else if (viewableAngle >= 150 && viewableAngle <= 180)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_180", true);
        }
        else if (viewableAngle <= -75 && viewableAngle >= -105)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_90", true);
        }
        else if (viewableAngle <= -150 && viewableAngle >= -180)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_180", true);
        }
    }
}
