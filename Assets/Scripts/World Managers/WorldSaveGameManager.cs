using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance;

    public PlayerManager player;

    [Header("Save/Load")]
    [SerializeField] bool saveGame;
    [SerializeField] bool loadGame;

    [Header("World Scene Index")]
    [SerializeField] int worldSceneIndex = 1;

    [Header("Save File Creator")]
    private SaveFileCreator saveFileCreator;

    [Header("Current Character Data")]
    public CharacterSlot currentCharacterSlotBeingUsed;
    public CharacterSaveData currentCharacterData;
    private string saveFileName;

    [Header("Character Slots")]
    public CharacterSaveData characterSlot01;
    public CharacterSaveData characterSlot02;
    public CharacterSaveData characterSlot03;
    public CharacterSaveData characterSlot04;
    public CharacterSaveData characterSlot05;
    public CharacterSaveData characterSlot06;
    public CharacterSaveData characterSlot07;
    public CharacterSaveData characterSlot08;
    public CharacterSaveData characterSlot09;
    public CharacterSaveData characterSlot10;

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

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadAllCharacterProfiles();
    }

    private void Update()
    {
        if (saveGame)
        {
            saveGame = false;
            SaveGame();
        }

        if (loadGame)
        {
            loadGame = false;
            LoadGame();
        }
    }

    public string DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot characterSlot)
    {
        string fileName = "";

        switch(characterSlot)
        {
            case CharacterSlot.CharacterSlot_01:
                fileName = "characterSlot_01";
                break;
            case CharacterSlot.CharacterSlot_02:
                fileName = "characterSlot_02";
                break;
            case CharacterSlot.CharacterSlot_03:
                fileName = "characterSlot_03";
                break;
            case CharacterSlot.CharacterSlot_04:
                fileName = "characterSlot_04";
                break;
            case CharacterSlot.CharacterSlot_05:
                fileName = "characterSlot_05";
                break;
            case CharacterSlot.CharacterSlot_06:
                fileName = "characterSlot_06";
                break;
            case CharacterSlot.CharacterSlot_07:
                fileName = "characterSlot_07";
                break;
            case CharacterSlot.CharacterSlot_08:
                fileName = "characterSlot_08";
                break;
            case CharacterSlot.CharacterSlot_09:
                fileName = "characterSlot_09";
                break;
            case CharacterSlot.CharacterSlot_10:
                fileName = "characterSlot_10";
                break;
        }
        return fileName;
    }

    // To refactor to remove unnecessary code
    public void AttemptToCreateNewGame()
    {
        saveFileCreator = new SaveFileCreator();
        saveFileCreator.saveFileDirectoryPath = Application.persistentDataPath;

        // Check to see if we can create a new save file
        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);
        if (!saveFileCreator.CheckIfFileExists()) // If slot not taken, use slot
        {
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_01;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);
        if (!saveFileCreator.CheckIfFileExists())
        {
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_02;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);
        if (!saveFileCreator.CheckIfFileExists())
        {
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_03;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_04);
        if (!saveFileCreator.CheckIfFileExists())
        {
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_04;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_05);
        if (!saveFileCreator.CheckIfFileExists())
        {
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_05;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_06);
        if (!saveFileCreator.CheckIfFileExists())
        {
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_06;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_07);
        if (!saveFileCreator.CheckIfFileExists())
        {
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_07;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_08);
        if (!saveFileCreator.CheckIfFileExists())
        {
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_08;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_09);
        if (!saveFileCreator.CheckIfFileExists())
        {
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_09;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_10);
        if (!saveFileCreator.CheckIfFileExists())
        {
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_10;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        // If there are no slots, notify player
        TitleScreenManager.instance.DisplayNoFreeCharacterSlotsPopUp();
    }

    public void CheckForFreeCharacterSlots()
    {
        saveFileCreator = new SaveFileCreator();
        saveFileCreator.saveFileDirectoryPath = Application.persistentDataPath;

        // Check to see if we can create a new save file
        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);
        if (!saveFileCreator.CheckIfFileExists())
        {
            TitleScreenManager.instance.DisplayIntro();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);
        if (!saveFileCreator.CheckIfFileExists())
        {
            TitleScreenManager.instance.DisplayIntro();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);
        if (!saveFileCreator.CheckIfFileExists())
        {
            TitleScreenManager.instance.DisplayIntro();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_04);
        if (!saveFileCreator.CheckIfFileExists())
        {
            TitleScreenManager.instance.DisplayIntro();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_05);
        if (!saveFileCreator.CheckIfFileExists())
        {
            TitleScreenManager.instance.DisplayIntro();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_06);
        if (!saveFileCreator.CheckIfFileExists())
        {
            TitleScreenManager.instance.DisplayIntro();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_07);
        if (!saveFileCreator.CheckIfFileExists())
        {
            TitleScreenManager.instance.DisplayIntro();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_08);
        if (!saveFileCreator.CheckIfFileExists())
        {
            TitleScreenManager.instance.DisplayIntro();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_09);
        if (!saveFileCreator.CheckIfFileExists())
        {
            TitleScreenManager.instance.DisplayIntro();
            return;
        }

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_10);
        if (!saveFileCreator.CheckIfFileExists())
        {
            TitleScreenManager.instance.DisplayIntro();
            return;
        }

        // If there are no slots, notify player
        TitleScreenManager.instance.DisplayNoFreeCharacterSlotsPopUp();
    }

    private void NewGame()
    {
        player.playerNetworkManager.vitality.Value = 10;
        player.playerNetworkManager.endurance.Value = 10;

        SaveGame();

        LoadWorldScene(worldSceneIndex);
    }

    public void SaveGame()
    {
        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

        saveFileCreator = new SaveFileCreator();
        saveFileCreator.saveFileDirectoryPath = Application.persistentDataPath;
        saveFileCreator.saveFileName = saveFileName;

        player.SaveGameDataFromCurrentCharacterData(ref currentCharacterData);

        saveFileCreator.CreateNewCharacterSaveFile(currentCharacterData);
    }

    public void SaveGameWithoutPosition()
    {
        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

        saveFileCreator = new SaveFileCreator();
        saveFileCreator.saveFileDirectoryPath = Application.persistentDataPath;
        saveFileCreator.saveFileName = saveFileName;

        player.SaveGameDataFromCurrentCharacterDataWithoutPosition(ref currentCharacterData);

        saveFileCreator.CreateNewCharacterSaveFile(currentCharacterData);
    }

    public void LoadGame()
    {
        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

        saveFileCreator = new SaveFileCreator();
        // Generally works on multiple machine types
        saveFileCreator.saveFileDirectoryPath = Application.persistentDataPath;
        saveFileCreator.saveFileName = saveFileName;
        currentCharacterData = saveFileCreator.LoadSaveFile();

        LoadWorldScene(worldSceneIndex);
    }

    public void DeleteGame(CharacterSlot characterSlot)
    {
        // Choose file to delete
        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

        saveFileCreator = new SaveFileCreator();
        saveFileCreator.saveFileDirectoryPath = Application.persistentDataPath;
        saveFileCreator.saveFileName = saveFileName;
        saveFileCreator.DeleteSaveFile();
    }

    public void LoadAllCharacterProfiles()
    {
        saveFileCreator = new SaveFileCreator();
        saveFileCreator.saveFileDirectoryPath = Application.persistentDataPath;

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);
        characterSlot01 = saveFileCreator.LoadSaveFile();

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);
        characterSlot02 = saveFileCreator.LoadSaveFile();

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);
        characterSlot03 = saveFileCreator.LoadSaveFile();

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_04);
        characterSlot04 = saveFileCreator.LoadSaveFile();

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_05);
        characterSlot05 = saveFileCreator.LoadSaveFile();

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_06);
        characterSlot06 = saveFileCreator.LoadSaveFile();

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_07);
        characterSlot07 = saveFileCreator.LoadSaveFile();

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_08);
        characterSlot08 = saveFileCreator.LoadSaveFile();

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_09);
        characterSlot09 = saveFileCreator.LoadSaveFile();

        saveFileCreator.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_10);
        characterSlot10 = saveFileCreator.LoadSaveFile();
    }

    public void LoadWorldScene(int buildIndex)
    {
        /*
         * When it was IEnumerator instead of void
        // If only one world scene is needed
        // AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        // Else
        int index = currentCharacterData.sceneIndex;
        if (index == 0)
        {
            index = worldSceneIndex;
        }
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(index);

        player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);
        yield return null;
        */

        string worldScene = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        NetworkManager.Singleton.SceneManager.LoadScene(worldScene, LoadSceneMode.Single);

        // Added so that the player is loaded after completely loading the scene
        // Without this, the player falls off the ground when loading
        NetworkManager.Singleton.SceneManager.OnLoadComplete += LoadCharacterGameData;
    }

    private void LoadCharacterGameData(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);
    }

    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }
}
