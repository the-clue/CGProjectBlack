using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISkeletonCharacterManager : AIBossCharacterManager
{
    [HideInInspector] public AISkeletonSoundFXManager skeletonSoundFXManager;
    [HideInInspector] public AISkeletonCombatManager skeletonCombatManager;

    protected override void Awake()
    {
        base.Awake();

        skeletonSoundFXManager = GetComponent<AISkeletonSoundFXManager>();
        skeletonCombatManager = GetComponent<AISkeletonCombatManager>();
    }
}
