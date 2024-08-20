using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Poise")]
    public float totalPoiseDamage; // damage taken in recent attacks
    // public float offensivePoiseBonus; // bonus poise when swinging some weapon types (ex. Great Swords)
    public float defaultPoiseResetTimer = 8; // Time it takes for poise damage to reset
    public float basePoiseDefense;
    public float poiseResetTimer = 0; // Current poise timer

    [Header("Stamina Regeneration")]
    [SerializeField] float staminaRegenAmount = 2;
    private float staminaRegenerationTimer = 0;
    private float staminaTickTimer = 0;
    [SerializeField] float staminaRegenDelay = 2;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        HandlePoiseResetTimer();
        // You can also move RegenerateStamina() here
    }

    public int CalculateHealthBasedOnVitalityLevel(int vitality)
    {
        int health = vitality * 20;

        return health;
    }

    public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
    {
        int stamina = endurance * 10;

        return stamina;
    }

    public virtual void RegenerateStamina()
    {
        if (!character.IsOwner)
        {
            return;
        }
        // No regeneration when sprinting
        if (character.characterNetworkManager.isSprinting.Value)
        {
            return;
        }
        if (character.isPerformingAction)
        {
            return;
        }

        staminaRegenerationTimer += Time.deltaTime;

        if (staminaRegenerationTimer >= staminaRegenDelay)
        {
            if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
            {
                staminaTickTimer += Time.deltaTime;

                if (staminaTickTimer >= 0.1)
                {
                    staminaTickTimer = 0;
                    character.characterNetworkManager.currentStamina.Value += staminaRegenAmount;
                }
            }
        }

    }

    public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount)
    {
        // We only want to reset the regen if the action used stamina
        // We don't want to reset if we are already regenerating
        if (currentStaminaAmount < previousStaminaAmount)
        {
            staminaRegenerationTimer = 0;
        }
    }

    protected virtual void HandlePoiseResetTimer()
    {
        if (poiseResetTimer > 0)
        {
            poiseResetTimer -= Time.deltaTime;
        }
        else
        {
            totalPoiseDamage = 0;
        }
    }
}
