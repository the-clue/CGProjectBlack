using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUICharacterMenuManager : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] GameObject menu;

    public void OpenCharacterMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = true;
        menu.SetActive(true);
    }

    public void CloseCharacterMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = false;
        menu.SetActive(false);
    }

    // needed if you jump with x, so that pressing Exit button won't make the character jump
    public void CloseCharacterMenuAfterFixedUpdate()
    {
        StartCoroutine(WaitThenCloseMenu());
    }

    private IEnumerator WaitThenCloseMenu()
    {
        yield return new WaitForFixedUpdate();

        PlayerUIManager.instance.menuWindowIsOpen = false;
        menu.SetActive(false);
    }
}
