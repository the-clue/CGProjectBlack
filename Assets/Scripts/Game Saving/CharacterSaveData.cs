using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// Since we want to reference this data for every save file, this script is not monobehaviour and is instead serializable
public class CharacterSaveData
{
    [Header("Stats")]
    public int vitality;
    public int currentHealth;
    public int endurance;
    public float currentStamina;

    [Header("Scene Index")]
    public int sceneIndex = 1; // default/starting world scene

    [Header("Character Name")]
    public string characterName = "Character";

    [Header("Time Played")]
    public float secondsPlayed;

    [Header("World Coordinates")] // Not Vector3 because we can only save basic values
    public float xPosition;
    public float yPosition;
    public float zPosition;

    [Header("Respawn Points")]
    public SerializableDictionary<int, bool> respawnPoints; // where int is the respawn point ID, bool is the activated status

    [Header("Bosses")]
    public SerializableDictionary<int, bool> bossesAwakened; // where int is the boss ID, bool is the awakened status
    public SerializableDictionary<int, bool> bossesDefeated; // where int is the boss ID, bool is the defeated status

    [Header("World Items")]
    public SerializableDictionary<int, bool> worldItemsLooted; // where int is the item ID, bool is the looted status

    public CharacterSaveData()
    {
        respawnPoints = new SerializableDictionary<int, bool>();
        bossesAwakened = new SerializableDictionary<int, bool>();
        bossesDefeated = new SerializableDictionary<int, bool>();
        worldItemsLooted = new SerializableDictionary<int, bool>();
    }
}
