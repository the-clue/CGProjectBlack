using UnityEngine;

public class RangeMagicDamageCollider : DamageCollider
{
    private CharacterManager characterCausingDamage;
    private CharacterManager characterTarget;
    private float rangeMagicDamage;
    private float rangePoiseDamage;
    private float rangeMagicSpeed;
    private float rangeMagicAngleSpeed;
    private float rangeMagicDelay;
    private int rangeMagicMode; // 0 -> straight, 1 -> planar-homing, 2 -> homing
    private float rangeMagicHomingDuration;

    [SerializeField] bool rangeMagicIsDestroyable = true;
    [SerializeField] GameObject rangeMagicHitVFX;

    [SerializeField] private Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();

        if (damageCollider == null)
        {
            damageCollider = GetComponent<Collider>();
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        // This makes it possible for some projectiles to be undestructible (a sword can't destroy a projectile if false)
        if (other.gameObject.layer == 10 && !rangeMagicIsDestroyable) { return; }

        // If the gameObject comes into contact with another collider which isn't a damage collider
        if (other.gameObject.layer != 10) // layer 10 is damage collider (ex. swords or other projectiles)
        {
            CharacterManager characterHit = other.GetComponentInParent<CharacterManager>();

            if (characterHit != null)
            {
                if (characterHit == characterCausingDamage || characterHit.characterNetworkManager.isInvulnerable.Value)
                {
                    return;
                }
                else
                {
                    contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                    DamageTarget(characterHit);
                }
            }
        }
        else // else it is a damage collider
        {
            // The following makes it so that projectiles from the same caster won't destroy each other
            RangeMagicDamageCollider otherCollider = other.GetComponent<RangeMagicDamageCollider>();
            if (otherCollider != null)
            {
                if (otherCollider.characterCausingDamage == characterCausingDamage)
                {
                    return;
                }
            }
        }

        if (rangeMagicHitVFX != null)
        {
            _ = Instantiate(rangeMagicHitVFX, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    /* BACK UP
    private void FixedUpdate()
    {
        if (rangeMagicDelay > 0)
        {
            rangeMagicDelay -= 0.1f;
            return;
        }

        if (characterTarget != null)
        {
            Vector3 direction = (characterTarget.transform.position - transform.position).normalized;
            float rotateAmount = transform.InverseTransformDirection(Vector3.Cross(direction, transform.up)).z;

            if (rangeMagicIsHoming)
            {
                rb.velocity = transform.forward * rangeMagicSpeed;
                rb.angularVelocity = new Vector3(rangeMagicAngleSpeed, rangeMagicAngleSpeed * rotateAmount, 0);
            }
            else
            {
                rb.velocity = direction * rangeMagicSpeed;
                rb.angularVelocity = new Vector3(0, rangeMagicAngleSpeed * rotateAmount, 0);
            }
        }
    }
    */
    private void FixedUpdate()
    {
        if (rangeMagicDelay > 0)
        {
            Vector3 targetTransform = characterTarget.transform.position;
            targetTransform.y = targetTransform.y + 1.5f; // this makes it get the probable height of the target
            Vector3 direction = (targetTransform - transform.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rangeMagicAngleSpeed * Time.fixedDeltaTime);

            // If you ever want to change the starting rotation of just one axis
            // Vector3 temp = transform.rotation.eulerAngles;
            // temp.x = -45;
            // transform.rotation = Quaternion.Euler(temp);

            rangeMagicDelay -= 0.1f;
            return;
        }

        if (characterTarget != null)
        {
            if (rangeMagicMode == 0) // straight
            {
                rb.velocity = transform.forward * rangeMagicSpeed;
            }
            else
            {
                Vector3 targetTransform = characterTarget.transform.position;
                targetTransform.y = targetTransform.y + 1.5f; // this makes it get the probable center of the target
                Vector3 direction = (targetTransform - transform.position).normalized;
                float rotateAmount = transform.InverseTransformDirection(Vector3.Cross(direction, transform.up)).z;

                if (rangeMagicMode == 1) // planar homing (only homes left or right)
                {
                    rb.velocity = transform.forward * rangeMagicSpeed;
                    if (rangeMagicHomingDuration > 0)
                    {
                        rb.angularVelocity = new Vector3(0, rangeMagicAngleSpeed * rotateAmount, 0);
                        rangeMagicHomingDuration -= 0.1f;
                    }
                    else
                    {
                        rb.angularVelocity = new Vector3(0, 0, 0);
                    }
                }
                else if (rangeMagicMode == 2) // global homing (homes up, down, left or right)
                {
                    rb.velocity = transform.forward * rangeMagicSpeed;

                    if (rangeMagicHomingDuration > 0)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rangeMagicAngleSpeed * Time.fixedDeltaTime);
                        rangeMagicHomingDuration -= 0.1f;
                    }
                }
            }
        }
    }

    // Function that sets every variable of the range magic damage collider
    public void SetRangeMagicDamageCollider(CharacterManager characterCausingDamage, CharacterManager characterTarget,
        float rangeMagicDamage, float rangePoiseDamage, float rangeMagicSpeed, float rangeMagicAngleSpeed,
        int rangeMagicMode, float rangeMagicDelay = 0f, float rangeMagicHomingDuration = 5f, bool rangeMagicPointTargetAtStart = true)
    {
        this.characterCausingDamage = characterCausingDamage;
        this.characterTarget = characterTarget;
        this.rangeMagicDamage = rangeMagicDamage;
        this.rangePoiseDamage = rangePoiseDamage;
        this.rangeMagicSpeed = rangeMagicSpeed;
        this.rangeMagicAngleSpeed = rangeMagicAngleSpeed;
        this.rangeMagicMode = rangeMagicMode;
        this.rangeMagicDelay = rangeMagicDelay;
        this.rangeMagicHomingDuration = rangeMagicHomingDuration;

        if (rangeMagicPointTargetAtStart)
        {
            Vector3 targetTransform = characterTarget.transform.position;
            targetTransform.y = targetTransform.y + 1.5f;
            Vector3 direction = (targetTransform - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }
    }

    protected override void DamageTarget(CharacterManager characterAboutToBeDamaged)
    {
        // not really needed anymore because of a condition in OnTriggerEnter?
        if (characterAboutToBeDamaged == characterCausingDamage)
        {
            return;
        }

        if (charactersDamaged.Contains(characterAboutToBeDamaged))
        {
            return;
        }

        charactersDamaged.Add(characterAboutToBeDamaged);

        TakeHealthDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = 0;
        damageEffect.magicDamage = rangeMagicDamage;
        damageEffect.poiseDamage = rangePoiseDamage;
        damageEffect.contactPoint = contactPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(transform.forward, characterAboutToBeDamaged.transform.forward, Vector3.up);

        // Deal damage if AI hits target on the connected character's side, regardless of how it looks on any client
        if (characterAboutToBeDamaged.IsOwner)
        {
            characterAboutToBeDamaged.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(characterAboutToBeDamaged.NetworkObjectId,
                characterCausingDamage.NetworkObjectId, damageEffect.physicalDamage, damageEffect.magicDamage, damageEffect.poiseDamage,
                damageEffect.angleHitFrom, damageEffect.contactPoint.x, damageEffect.contactPoint.y, damageEffect.contactPoint.z);
        }
    }
}