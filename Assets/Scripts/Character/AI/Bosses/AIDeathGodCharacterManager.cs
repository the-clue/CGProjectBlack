using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDeathGodCharacterManager : AIBossCharacterManager
{
    [HideInInspector] public AIDeathGodSoundFXManager deathGodSoundFXManager;
    [HideInInspector] public AIDeathGodCombatManager deathGodCombatManager;

    protected override void Awake()
    {
        base.Awake();

        deathGodSoundFXManager = GetComponent<AIDeathGodSoundFXManager>();
        deathGodCombatManager = GetComponent<AIDeathGodCombatManager>();
    }
}
