using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerBossFight : MonoBehaviour
{
    [SerializeField] int bossID;

    // [Header("Player Quit Respawn Position")]
    // [SerializeField] Vector3 playerQuitRespawnPosition = Vector3.zero;

    private void OnTriggerEnter(Collider other)
    {
        AIBossCharacterManager boss = WorldAIManager.instance.GetBossCharacterByID(bossID);

        if (boss != null && !boss.hasBeenAwakened.Value)
        {
            boss.WakeBoss();
        }

        // If you want to consider saving here, create a new SaveGame function which also changes the position
        // Using the current SaveGame doesn't consider this position but the character's current position
        // WorldSaveGameManager.instance.currentCharacterData.xPosition = playerQuitRespawnPosition.x;
        // WorldSaveGameManager.instance.currentCharacterData.yPosition = playerQuitRespawnPosition.y;
        // WorldSaveGameManager.instance.currentCharacterData.zPosition = playerQuitRespawnPosition.z;

        // WorldSaveGameManager.instance.SaveGame();
    }

    private void OnTriggerExit(Collider other)
    {
        gameObject.SetActive(false);
    }
}
