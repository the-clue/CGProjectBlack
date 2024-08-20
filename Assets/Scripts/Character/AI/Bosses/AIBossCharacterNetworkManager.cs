using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBossCharacterNetworkManager : AICharacterNetworkManager
{
    AIBossCharacterManager aiBossCharacter;

    protected override void Awake()
    {
        base.Awake();

        aiBossCharacter = GetComponent<AIBossCharacterManager>();
    }

    public override void CheckHealth(int oldValue, int newValue)
    {
        base.CheckHealth(oldValue, newValue);

        if (IsOwner)
        {
            // So it doesn't phase shift when already dead or if it already has
            if (aiBossCharacter.hasPhaseShifted || currentHealth.Value <= 0)
            {
                return;
            }

            // Phase Shifting
            float healthNeededToShift = maxHealth.Value * (aiBossCharacter.minimumHealthPercentageToShift / 100);

            if (currentHealth.Value <= healthNeededToShift)
            {
                aiBossCharacter.PhaseShift();
            }
        }
    }
}
