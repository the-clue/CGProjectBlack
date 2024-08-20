using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager instance;

    [Header("Title Screen Menu")]
    [SerializeField] GameObject titleScreenMainMenu;

    [Header("Buttons")]
    [SerializeField] Button noCharacterSlotsOkayButton;
    [SerializeField] Button deleteCharacterPopUpConfirmButton;


    [Header("Pop Ups")]
    [SerializeField] GameObject noCharacterSlotsPopUp;
    [SerializeField] GameObject deleteCharacterSlotPopUp;

    [Header("Character Slots")]
    public CharacterSlot currentSelectedSlot = CharacterSlot.NO_SLOT;

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

    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void StartNewGame()
    {
        WorldSaveGameManager.instance.AttemptToCreateNewGame();
    }

    public void DisplayNoFreeCharacterSlotsPopUp()
    {
        titleScreenMainMenu.SetActive(false);
        noCharacterSlotsPopUp.SetActive(true);
        noCharacterSlotsOkayButton.Select();
    }

    // Character Slots
    public void SelectCharacterSlot(CharacterSlot characterSlot)
    {
        currentSelectedSlot = characterSlot;
    }

    public void SelectNoSlot()
    {
        currentSelectedSlot = CharacterSlot.NO_SLOT;
    }

    public void AttemptToDeleteCharacterSlot()
    {
        if (currentSelectedSlot != CharacterSlot.NO_SLOT)
        {
            deleteCharacterSlotPopUp.SetActive(true);
            deleteCharacterPopUpConfirmButton.Select();
        }
    }

    public void DeleteCharacterSlot ()
    {
        WorldSaveGameManager.instance.DeleteGame(currentSelectedSlot);
    }
}
