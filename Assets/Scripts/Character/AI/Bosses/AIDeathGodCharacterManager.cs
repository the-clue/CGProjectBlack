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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (hasBeenAwakened.Value)
        {
            deathGodCombatManager.MakeWeaponAppear();
        }
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        PlayerUIManager.instance.playerUIPopUpManager.SendBossDefeatedPopUp("DEITY DEFEATED");

        if (IsOwner)
        {
            characterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;
            bossFightIsActive.Value = false;

            if (!manuallySelectDeathAnimation)
            {
                characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
            }
        }

        yield return new WaitForSeconds(5);

        WorldSaveGameManager.instance.LoadWorldScene(2);
    }
}
