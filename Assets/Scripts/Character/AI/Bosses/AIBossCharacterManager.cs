using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AIBossCharacterManager : AICharacterManager
{
    public int bossID = 0;

    [Header("Music")]
    [SerializeField] AudioClip bossIntroClip;
    [SerializeField] AudioClip bossLoopClip;

    [Header("Status")]
    public NetworkVariable<bool> bossFightIsActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> hasBeenAwakened = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> hasBeenDefeated = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] List<FogWallInteractable> fogWalls;
    [SerializeField] string sleepAnimation;
    [SerializeField] string awakenAnimation;

    [Header("Phase Shift")]
    public float minimumHealthPercentageToShift = 50;
    [SerializeField] string phaseShiftAnimation = "Phase_Shift_01";
    [SerializeField] CombatStanceState secondPhaseCombatStanceState;
    public bool hasPhaseShifted = false;

    [Header("States")]
    [SerializeField] SleepState sleepState;

    GameObject bossHPBar;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        navMeshAgent.Warp(transform.position); // for some reason, navmeshagent is created at global origin, so this is needed

        bossFightIsActive.OnValueChanged += OnBossFightIsActiveChanged;
        OnBossFightIsActiveChanged(false, bossFightIsActive.Value);

        if (IsOwner)
        {
            sleepState = Instantiate(sleepState);
            currentState = sleepState;
        }

        if (IsServer)
        {
            // If save data does not contain information of this boss, add it
            if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, false);
                WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, false);

            }
            else
            {
                hasBeenDefeated.Value = WorldSaveGameManager.instance.currentCharacterData.bossesDefeated[bossID];
                hasBeenAwakened.Value = WorldSaveGameManager.instance.currentCharacterData.bossesAwakened[bossID];

            }

            // To locate fog wall
            StartCoroutine(GetFogWallsFromWorldObjectManager());

            // If boss is awakened, then enable the fog walls
            if (hasBeenAwakened.Value)
            {
                for (int i = 0; i < fogWalls.Count; i++)
                {
                    fogWalls[i].isActive.Value = true;
                }
            }

            // If boss is defeated, then disable the fog walls
            if (hasBeenDefeated.Value)
            {
                for (int i = 0; i < fogWalls.Count; i++)
                {
                    fogWalls[i].isActive.Value = false;
                }
                aiCharacterNetworkManager.isActive.Value = false;
            }
        }

        if (!hasBeenAwakened.Value)
        {
            animator.Play(sleepAnimation);
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (bossHPBar != null)
        {
            Destroy(bossHPBar);
        }
        bossFightIsActive.OnValueChanged -= OnBossFightIsActiveChanged;
    }

    private IEnumerator GetFogWallsFromWorldObjectManager()
    {
        while (WorldObjectManager.instance.fogWalls.Count == 0)
        {
            yield return new WaitForEndOfFrame();
        }

        fogWalls = new List<FogWallInteractable>();

        foreach (var fogWall in WorldObjectManager.instance.fogWalls)
        {
            if (fogWall.fogWallID == bossID)
            {
                fogWalls.Add(fogWall);
            }
        }
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        PlayerUIManager.instance.playerUIPopUpManager.SendBossDefeatedPopUp("");

        if (IsOwner)
        {
            characterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;
            bossFightIsActive.Value = false;

            foreach (var fogWall in fogWalls)
            {
                fogWall.isActive.Value = false;
            }

            // reset necessary flags

            // if not grounded, play air death animation

            if (!manuallySelectDeathAnimation)
            {
                characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
            }

            hasBeenDefeated.Value = true;

            if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
            }
            else
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(bossID);
                WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Remove(bossID);
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
            }
            WorldSaveGameManager.instance.SaveGame();
        }

        // death sfx

        yield return new WaitForSeconds(5);
        // award player with runes
        // disable character
    }

    public void WakeBoss()
    {
        if (IsOwner)
        {
            if (!hasBeenAwakened.Value)
            {
                characterAnimatorManager.PlayTargetActionAnimation(awakenAnimation, true);
            }

            bossFightIsActive.Value = true;
            hasBeenAwakened.Value = true;
            currentState = idleState;

            if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
            }
            else
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(bossID);
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
            }

            for (int i = 0; i < fogWalls.Count; i++)
            {
                fogWalls[i].isActive.Value = true;
            }
        }
    }

    private void OnBossFightIsActiveChanged(bool oldStatus, bool newStatus)
    {
        if (bossFightIsActive.Value)
        {
            WorldSoundFXManager.instance.PlayBossTrack(bossIntroClip, bossLoopClip);

            bossHPBar = Instantiate(PlayerUIManager.instance.playerUIHudManager.bossHPBarObject, PlayerUIManager.instance.playerUIHudManager.bossHPBarParent);

            UI_Boss_HP_Bar uiBossHPBar = bossHPBar.GetComponentInChildren<UI_Boss_HP_Bar>();
            uiBossHPBar.EnableBossHPBar(this);
        }
        else
        {
            WorldSoundFXManager.instance.StopBossMusic();
        }
    }

    public void PhaseShift()
    {
        // Added so bosses don't repeat phase shifting animation after phase shifting once
        hasPhaseShifted = true;
        characterCombatManager.EnableIsPoiseInvulnerable(); // remember to put DisableIsPoiseInvulnerable in the Phase Shift animation

        characterAnimatorManager.PlayTargetActionAnimation(phaseShiftAnimation, true);
        combatStanceState = Instantiate(secondPhaseCombatStanceState);
        currentState = combatStanceState;
    }

    public void ActivateFight()
    {
        bossFightIsActive.Value = true;

        currentState = idleState;
    }
}
