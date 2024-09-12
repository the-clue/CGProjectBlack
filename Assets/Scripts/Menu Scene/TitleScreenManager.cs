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
    [SerializeField] GameObject titleScreenIntroScene;
    [SerializeField] Image fader;
 
    [Header("Buttons")]
    [SerializeField] Button noCharacterSlotsOkayButton;
    [SerializeField] Button deleteCharacterPopUpConfirmButton;
    [SerializeField] Button introSceneButton;


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

    public void CheckForFreeCharacterSlots()
    {
        WorldSaveGameManager.instance.CheckForFreeCharacterSlots();
    }

    public void DisplayIntro()
    {
        StartCoroutine(FadeToIntro());
    }

    IEnumerator FadeToIntro()
    {
        fader.enabled = true;

        introSceneButton.Select();

        float elapsedTime = 0f;
        float fadeDuration = 1f;

        // Loop until the specified fade duration has elapsed
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float alphaValue = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);

            Color newColor = new Color(fader.color.r, fader.color.g, fader.color.b, alphaValue);
            fader.color = newColor;

            yield return null; // Wait until the next frame
        }

        fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, 0);

        titleScreenIntroScene.SetActive(true);
        introSceneButton.Select();
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
