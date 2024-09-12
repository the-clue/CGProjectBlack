using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerStatsManager : CharacterStatsManager
{
    PlayerManager player;

    [Header("Health Regeneration")]
    [SerializeField] int healthRegenAmount = 2;
    private float healthRegenerationTimer = 0;
    private float healthTickTimer = 0;
    [SerializeField] float healthRegenDelay = 2;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Start()
    {
        base.Start();

        // CalculateHealthBasedOnVitalityLevel(player.playerNetworkManager.vitality.Value);
        // CalculateStaminaBasedOnEnduranceLevel(player.playerNetworkManager.endurance.Value);
    }

    public virtual void RegenerateHealth()
    {
        if (!player.IsOwner)
        {
            return;
        }
        // No regeneration when dead
        if (player.isDead.Value)
        {
            return;
        }
        if (player.isPerformingAction) // No regeneration when attacking?
        {
            return;
        }

        healthRegenerationTimer += Time.deltaTime;

        if (healthRegenerationTimer >= healthRegenDelay)
        {
            if (player.characterNetworkManager.currentHealth.Value < player.characterNetworkManager.maxHealth.Value)
            {
                healthTickTimer += Time.deltaTime;

                if (healthTickTimer >= 0.35f) // this 0.35f controls how fast you regenerate
                {
                    healthTickTimer = 0;
                    player.characterNetworkManager.currentHealth.Value += healthRegenAmount;
                }
            }
        }

    }

    public virtual void ResetHealthRegenTimer(int previousHealthAmount, int currentHealthAmount)
    {
        // We only want to reset the regen if the player gets damaged
        // We don't want to reset if we are already regenerating
        if (currentHealthAmount < previousHealthAmount)
        {
            healthRegenerationTimer = 0;
        }
    }
}
