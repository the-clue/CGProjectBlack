using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDeathSovereignCharacterManager : AIBossCharacterManager
{
    [HideInInspector] public AIDeathSovereignSoundFXManager deathSovereignSoundFXManager;
    [HideInInspector] public AIDeathSovereignCombatManager deathSovereignCombatManager;

    protected override void Awake()
    {
        base.Awake();

        deathSovereignSoundFXManager = GetComponent<AIDeathSovereignSoundFXManager>();
        deathSovereignCombatManager = GetComponent<AIDeathSovereignCombatManager>();
    }
}
