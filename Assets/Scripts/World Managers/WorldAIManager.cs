using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    [Header("Characters")]
    [SerializeField] List<AICharacterSpawner> aiCharacterSpawners;
    [SerializeField] List<AICharacterManager> spawnedInCharacters;
    [SerializeField] List<AIBossCharacterManager> spawnedInBosses;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnCharacter(AICharacterSpawner aiCharacterSpawner)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            aiCharacterSpawners.Add(aiCharacterSpawner);
            aiCharacterSpawner.AttemptToSpawnCharacter();
        }
    }

    public void AddCharacterToSpawnedCharactersList(AICharacterManager character)
    {
        if (spawnedInCharacters.Contains(character))
        {
            return;
        }

        spawnedInCharacters.Add(character);

        AIBossCharacterManager bossCharacter = character as AIBossCharacterManager;

        if (bossCharacter != null)
        {
            if (spawnedInBosses.Contains(bossCharacter))
            {
                return;
            }

            spawnedInBosses.Add(bossCharacter);
        }
    }

    public AIBossCharacterManager GetBossCharacterByID(int ID)
    {
        return spawnedInBosses.FirstOrDefault(boss => boss.bossID == ID);
    }

    public void ResetAllCharacters()
    {
        DespawnAllCharacters();

        foreach (var spawner in aiCharacterSpawners)
        {
            spawner.AttemptToSpawnCharacter();
        }
    }

    private void DespawnAllCharacters()
    {
        foreach (var character in spawnedInCharacters)
        {
            character.GetComponent<NetworkObject>().Despawn();
        }

        spawnedInCharacters.Clear();
        spawnedInBosses.Clear();
    }

    private void DisableAllCharacters()
    {


    }
}
