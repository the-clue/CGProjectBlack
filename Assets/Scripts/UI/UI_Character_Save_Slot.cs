using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Character_Save_Slot : MonoBehaviour
{
    SaveFileCreator saveFileCreator;

    [Header("Game Slot")]
    public CharacterSlot characterSlot;

    [Header("Character Info")]
    [SerializeField] Image saveSlotIcon_01;
    [SerializeField] Image saveSlotIcon_02;
    [SerializeField] Image saveSlotIcon_03;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI timePlayed;

    private void OnEnable()
    {
        LoadSaveSlots();
    }

    private void LoadSaveSlots()
    {
        saveFileCreator = new SaveFileCreator();
        saveFileCreator.saveFileDirectoryPath = Application.persistentDataPath;

        switch(characterSlot)
        {
            case CharacterSlot.CharacterSlot_01:
                saveFileCreator.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileCreator.CheckIfFileExists())
                {
                    // characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                    characterName.text = "RUN 1";
                    saveSlotIcon_01.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot01.weaponsInWeaponSlots[0]).itemIcon;
                    saveSlotIcon_02.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot01.weaponsInWeaponSlots[1]).itemIcon;
                    saveSlotIcon_03.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot01.weaponsInWeaponSlots[2]).itemIcon;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case CharacterSlot.CharacterSlot_02:
                saveFileCreator.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileCreator.CheckIfFileExists())
                {
                    // characterName.text = WorldSaveGameManager.instance.characterSlot02.characterName;
                    characterName.text = "RUN 2";
                    saveSlotIcon_01.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot02.weaponsInWeaponSlots[0]).itemIcon;
                    saveSlotIcon_02.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot02.weaponsInWeaponSlots[1]).itemIcon;
                    saveSlotIcon_03.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot02.weaponsInWeaponSlots[2]).itemIcon;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case CharacterSlot.CharacterSlot_03:
                saveFileCreator.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileCreator.CheckIfFileExists())
                {
                    // characterName.text = WorldSaveGameManager.instance.characterSlot03.characterName;
                    characterName.text = "RUN 3";
                    saveSlotIcon_01.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot03.weaponsInWeaponSlots[0]).itemIcon;
                    saveSlotIcon_02.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot03.weaponsInWeaponSlots[1]).itemIcon;
                    saveSlotIcon_03.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot03.weaponsInWeaponSlots[2]).itemIcon;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case CharacterSlot.CharacterSlot_04:
                saveFileCreator.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileCreator.CheckIfFileExists())
                {
                    // characterName.text = WorldSaveGameManager.instance.characterSlot04.characterName;
                    characterName.text = "RUN 4";
                    saveSlotIcon_01.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot04.weaponsInWeaponSlots[0]).itemIcon;
                    saveSlotIcon_02.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot04.weaponsInWeaponSlots[1]).itemIcon;
                    saveSlotIcon_03.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot04.weaponsInWeaponSlots[2]).itemIcon;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case CharacterSlot.CharacterSlot_05:
                saveFileCreator.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileCreator.CheckIfFileExists())
                {
                    // characterName.text = WorldSaveGameManager.instance.characterSlot05.characterName;
                    characterName.text = "RUN 5";
                    saveSlotIcon_01.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot05.weaponsInWeaponSlots[0]).itemIcon;
                    saveSlotIcon_02.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot05.weaponsInWeaponSlots[1]).itemIcon;
                    saveSlotIcon_03.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot05.weaponsInWeaponSlots[2]).itemIcon;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case CharacterSlot.CharacterSlot_06:
                saveFileCreator.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileCreator.CheckIfFileExists())
                {
                    // characterName.text = WorldSaveGameManager.instance.characterSlot06.characterName;
                    characterName.text = "RUN 6";
                    saveSlotIcon_01.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot06.weaponsInWeaponSlots[0]).itemIcon;
                    saveSlotIcon_02.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot06.weaponsInWeaponSlots[1]).itemIcon;
                    saveSlotIcon_03.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot06.weaponsInWeaponSlots[2]).itemIcon;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case CharacterSlot.CharacterSlot_07:
                saveFileCreator.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileCreator.CheckIfFileExists())
                {
                    // characterName.text = WorldSaveGameManager.instance.characterSlot07.characterName;
                    characterName.text = "RUN 7";
                    saveSlotIcon_01.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot07.weaponsInWeaponSlots[0]).itemIcon;
                    saveSlotIcon_02.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot07.weaponsInWeaponSlots[1]).itemIcon;
                    saveSlotIcon_03.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot07.weaponsInWeaponSlots[2]).itemIcon;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case CharacterSlot.CharacterSlot_08:
                saveFileCreator.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileCreator.CheckIfFileExists())
                {
                    // characterName.text = WorldSaveGameManager.instance.characterSlot08.characterName;
                    characterName.text = "RUN 8";
                    saveSlotIcon_01.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot08.weaponsInWeaponSlots[0]).itemIcon;
                    saveSlotIcon_02.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot08.weaponsInWeaponSlots[1]).itemIcon;
                    saveSlotIcon_03.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot08.weaponsInWeaponSlots[2]).itemIcon;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case CharacterSlot.CharacterSlot_09:
                saveFileCreator.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileCreator.CheckIfFileExists())
                {
                    // characterName.text = WorldSaveGameManager.instance.characterSlot09.characterName;
                    characterName.text = "RUN 9";
                    saveSlotIcon_01.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot09.weaponsInWeaponSlots[0]).itemIcon;
                    saveSlotIcon_02.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot09.weaponsInWeaponSlots[1]).itemIcon;
                    saveSlotIcon_03.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot09.weaponsInWeaponSlots[2]).itemIcon;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case CharacterSlot.CharacterSlot_10:
                saveFileCreator.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileCreator.CheckIfFileExists())
                {
                    // characterName.text = WorldSaveGameManager.instance.characterSlot10.characterName;
                    characterName.text = "RUN 10";
                    saveSlotIcon_01.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot10.weaponsInWeaponSlots[0]).itemIcon;
                    saveSlotIcon_02.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot10.weaponsInWeaponSlots[1]).itemIcon;
                    saveSlotIcon_03.sprite = WorldItemDatabase.instance.GetWeaponByID(WorldSaveGameManager.instance.characterSlot10.weaponsInWeaponSlots[2]).itemIcon;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
        }
    }

    public void LoadGameFromCharacterSlot()
    {
        WorldSaveGameManager.instance.currentCharacterSlotBeingUsed = characterSlot;
        WorldSaveGameManager.instance.LoadGame();
    }

    public void SelectCurrentSlot()
    {
        TitleScreenManager.instance.SelectCharacterSlot(characterSlot);
    }
}
