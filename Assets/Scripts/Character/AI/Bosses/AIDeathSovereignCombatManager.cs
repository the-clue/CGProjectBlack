using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class AIDeathSovereignCombatManager : AICharacterCombatManager
{
    AIDeathSovereignCharacterManager aiDeathSovereignManager;

    [Header("Damage Colliders")]
    // [SerializeField] MagicBulletDamageCollider attack01DamageCollider;
    // [SerializeField] MagicDamageCollider attack02DamageCollider;
    [SerializeField] MeleeDamageMultiCollider attack02DamageCollider;
    // [SerializeField] MagicDamageCollider attack03DamageCollider;
    // [SerializeField] MagicDamageCollider attack04DamageCollider;
    [SerializeField] MagicDamageCollider attack05DamageCollider;

    [Header("Damage")]
    [SerializeField] int baseDamage = 50;
    [SerializeField] int basePoiseDamage = 20;
    [SerializeField] float attack01DamageModifier = 1.0f; // Magic Bullet
    [SerializeField] float attack02DamageModifier = 1.5f; // Scythe
    [SerializeField] float attack03DamageModifier = 1.5f; // Magic Meteor
    [SerializeField] float attack04DamageModifier = 1.5f; // Magic Range Scythe
    [SerializeField] float PhaseShiftDamageModifier = 2.0f; // Magic AoE Slashes
    [SerializeField] float attack05DamageModifier = 0.25f; // Flamethrower

    [Header("Attack Parameters")]
    [SerializeField] float magicBulletSpeed = 15f;
    [SerializeField] float magicBulletRotationSpeed = 5f;
    [SerializeField] int magicBulletMode = 1;
    [SerializeField] float magicBulletDelay = 0f;
    [SerializeField] float magicBulletHomingDuration = 10f;
    [SerializeField] float magicRangeScytheSpeed = 15f;
    [SerializeField] float magicRangeScytheRotationSpeed = 5f;
    [SerializeField] int magicRangeScytheMode = 1;
    [SerializeField] float magicRangeScytheDelay = 0f;
    [SerializeField] float magicRangeScytheHomingDuration = 10f;
    // [SerializeField] float magicTeleportDistance = 15f;
    private Collider magicTeleportArea;

    [Header("Magic Spawn Points")]
    [SerializeField] Transform magicBulletSpawnPoint;
    [SerializeField] Transform magicBladeSpawnPoint;
    [SerializeField] Transform magicAoESlashesSpawnPoint;
    [SerializeField] Transform magicRangeScytheSpawnPoint;
    [SerializeField] Transform magicFlameSpawnPoint;

    [Header("VFX")]
    [SerializeField] GameObject magicBulletPrefab;
    [SerializeField] GameObject magicBladePrefab;
    [SerializeField] GameObject magicMeteorPrefab;
    [SerializeField] GameObject magicAoESlahesPrefab;
    [SerializeField] GameObject magicRangeScythePrefab;
    [SerializeField] GameObject magicFlamePrefab;
    [SerializeField] GameObject magicTeleportPrefab;

    protected override void Awake()
    {
        base.Awake();

        aiDeathSovereignManager = GetComponent<AIDeathSovereignCharacterManager>();
        magicTeleportArea = GameObject.FindGameObjectWithTag("SovereignTeleportArea").GetComponent<Collider>();
    }

    public void CastAttack01() // Magic Dagger Throw
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();

        aiCharacter.characterSoundFXManager.PlayWhooshes();

        float magicDamage = baseDamage * attack01DamageModifier;
        float magicPoiseDamage = basePoiseDamage * attack01DamageModifier;
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        eulerRotation.x = 0;
        eulerRotation.y = 0;
        GameObject rangeMagicGameObject = Instantiate(magicBulletPrefab, magicBulletSpawnPoint.position, Quaternion.Euler(eulerRotation));
        RangeMagicDamageCollider rangeMagicDamageCollider = rangeMagicGameObject.GetComponent<RangeMagicDamageCollider>();
        rangeMagicDamageCollider.SetRangeMagicDamageCollider(aiDeathSovereignManager, currentTarget, magicDamage, magicPoiseDamage,
            magicBulletSpeed, magicBulletRotationSpeed, magicBulletMode, magicBulletDelay, magicBulletHomingDuration);
    }

    public void SetAttack02()
    {
        magicBladePrefab.SetActive(true);

        attack02DamageCollider.SetDamage(0, baseDamage * attack02DamageModifier, basePoiseDamage * attack02DamageModifier);
    }
    public void CastAttack02()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        aiCharacter.characterSoundFXManager.PlayWhooshes();

        attack02DamageCollider.EnableDamageColliders();
    }
    public void RemoveAttack02()
    {
        attack02DamageCollider.DisableDamageColliders();
        // magicBladePrefab.SetActive(false); // Managed by a utility script
    }

    public void SetAttack03()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();

        var magicMeteor = Instantiate(magicMeteorPrefab, currentTarget.transform.position, Quaternion.identity);
        MagicDamageCollider magicMeteorCollider = magicMeteor.GetComponent<MagicDamageCollider>();
        magicMeteorCollider.SetCharacterCausingDamage(aiDeathSovereignManager);
        magicMeteorCollider.magicDamage = baseDamage * attack03DamageModifier;
        magicMeteorCollider.poiseDamage = basePoiseDamage * attack03DamageModifier;
    }

    /* Managed by a script in the magicMeteorPrefab
    public void EnableMeteorCollider()
    {
        attack03DamageCollider.EnableDamageCollider();
    }
    public void DisableMeteorCollider()
    {
        attack03DamageCollider.DisableDamageCollider();
    }
    */

    public void CastAttack04() // Magic Scythe Throw
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();

        float magicDamage = baseDamage * attack04DamageModifier;
        float magicPoiseDamage = basePoiseDamage * attack04DamageModifier;
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        eulerRotation.x = 0;
        eulerRotation.y = 0;
        GameObject rangeMagicGameObject = Instantiate(magicRangeScythePrefab, magicRangeScytheSpawnPoint.position, Quaternion.Euler(eulerRotation));
        RangeMagicDamageCollider rangeMagicDamageCollider = rangeMagicGameObject.GetComponent<RangeMagicDamageCollider>();
        rangeMagicDamageCollider.SetRangeMagicDamageCollider(aiDeathSovereignManager, currentTarget, magicDamage, magicPoiseDamage,
            magicRangeScytheSpeed, magicRangeScytheRotationSpeed, magicRangeScytheMode,
            magicRangeScytheDelay, magicRangeScytheHomingDuration);
    }

    public void SetAttackPhaseShift() // Used by Phase Shift
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        var magicSlashes = Instantiate(magicAoESlahesPrefab, magicAoESlashesSpawnPoint);
        MagicDamageCollider magicSlashesCollider = magicSlashes.GetComponent<MagicDamageCollider>();
        magicSlashesCollider.magicDamage = baseDamage * PhaseShiftDamageModifier;
        magicSlashesCollider.poiseDamage = basePoiseDamage * PhaseShiftDamageModifier;
    }

    /* Managed by a script in the magicAoESlashesPrefab
    public void EnableAoESlashesCollider()
    {
        attack04DamageCollider.EnableDamageCollider();
    }
    public void DisableAoESlashesCollider()
    {
        attack04DamageCollider.DisableDamageCollider();
    }
    */

    public void SetAttack05()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        attack05DamageCollider.magicDamage = baseDamage * attack05DamageModifier;
        attack05DamageCollider.poiseDamage = basePoiseDamage * attack05DamageModifier;
    }

    public void EnableFlameCollider()
    {
        _ = Instantiate(magicFlamePrefab, magicFlameSpawnPoint.position, magicFlameSpawnPoint.rotation);
        attack05DamageCollider.EnableDamageCollider();
    }
    public void DisableFlameCollider()
    {
        attack05DamageCollider.DisableDamageCollider();
    }

    public void Teleport()
    {
        _ = Instantiate(magicTeleportPrefab, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.LookRotation(transform.up));

        /*
        Vector3 randomDirection = Random.insideUnitSphere * magicTeleportDistance;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, magicTeleportDistance, 1);
        Vector3 finalPosition = hit.position;
        transform.position = finalPosition;

        Vector3 targetTransform = aiDeathSovereignManager.characterCombatManager.currentTarget.transform.position;
        Vector3 direction = (targetTransform - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = targetRotation;
        */

        Bounds bounds = magicTeleportArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        transform.position = new Vector3(x, transform.position.y, z);

        Vector3 direction = aiDeathSovereignManager.characterCombatManager.currentTarget.transform.position - transform.position;
        direction.y = 0;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = rotation;

        _ = Instantiate(magicTeleportPrefab, new Vector3(x, transform.position.y + 1, z), Quaternion.LookRotation(transform.up));
        // aiDeathGodManager.navMeshAgent.Warp(finalPosition); // not needed apparently
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
