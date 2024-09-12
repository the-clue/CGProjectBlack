using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIPopUpManager : MonoBehaviour
{
    [Header("Message Pop Up")]
    [SerializeField] TextMeshProUGUI popUpMessageText;
    [SerializeField] GameObject popUpMessageGameObject;

    [Header("Item Pop Up")]
    [SerializeField] GameObject itemPopUpGameObject;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemName;
    // [SerializeField] TextMeshProUGUI itemAmount;

    [Header("YOU DIED Pop Up")]
    [SerializeField] GameObject youDiedPopUpGameObject;
    [SerializeField] TextMeshProUGUI youDiedPopUpBackGroundText;
    [SerializeField] TextMeshProUGUI youDiedPopUpText;
    [SerializeField] CanvasGroup youDiedPopUpCanvasGroup; // to fade alpha over-time

    [Header("Boss Defeated Pop Up")]
    [SerializeField] GameObject bossDefeatedPopUpGameObject;
    [SerializeField] TextMeshProUGUI bossDefeatedPopUpBackGroundText;
    [SerializeField] TextMeshProUGUI bossDefeatedPopUpText;
    [SerializeField] CanvasGroup bossDefeatedPopUpCanvasGroup;

    [Header("General Pop Up")]
    [SerializeField] GameObject generalPopUpGameObject;
    [SerializeField] TextMeshProUGUI generalPopUpBackGroundText;
    [SerializeField] TextMeshProUGUI generalPopUpText;
    [SerializeField] CanvasGroup generalPopUpCanvasGroup;

    public void CloseAllPopUpWindows()
    {
        popUpMessageGameObject.SetActive(false);
        itemPopUpGameObject.SetActive(false);

        PlayerUIManager.instance.popUpWindowIsOpen = false;
    }

    public void SendPlayerMessagePopUp(string messageText)
    {
        PlayerUIManager.instance.popUpWindowIsOpen = true;
        popUpMessageText.text = messageText;
        popUpMessageGameObject.SetActive(true);
    }

    public void SendItemPopUp(Item item)
    {
        itemIcon.sprite = item.itemIcon;
        itemName.text = item.itemName;

        itemPopUpGameObject.SetActive(true);
        PlayerUIManager.instance.popUpWindowIsOpen = true;
    }


    public void SendYouDiedPopUp()
    {
        youDiedPopUpGameObject.SetActive(true);
        youDiedPopUpBackGroundText.characterSpacing = 0;
        StartCoroutine(StretchPopUpTextOverTime(youDiedPopUpBackGroundText, 3, 2));
        StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 1.5f));
        StartCoroutine(WaitThenFadePopUpOverTime(youDiedPopUpCanvasGroup, 3.5f, 1.5f));
    }

    public void SendBossDefeatedPopUp(string bossDefeatedMessage)
    {
        if (bossDefeatedMessage != "")
        {
            bossDefeatedPopUpText.text = bossDefeatedMessage;
            bossDefeatedPopUpBackGroundText.text = bossDefeatedMessage;
        }
        bossDefeatedPopUpGameObject.SetActive(true);
        bossDefeatedPopUpBackGroundText.characterSpacing = 0;
        StartCoroutine(StretchPopUpTextOverTime(bossDefeatedPopUpBackGroundText, 5, 2));
        StartCoroutine(FadeInPopUpOverTime(bossDefeatedPopUpCanvasGroup, 1.5f));
        StartCoroutine(WaitThenFadePopUpOverTime(bossDefeatedPopUpCanvasGroup, 5.5f, 1.5f));
    }

    public void SendGeneralPopUp(string message)
    {
        generalPopUpText.text = message;
        generalPopUpBackGroundText.text = message;
        generalPopUpGameObject.SetActive(true);
        generalPopUpBackGroundText.characterSpacing = 0;
        StartCoroutine(StretchPopUpTextOverTime(generalPopUpBackGroundText, 3, 2));
        StartCoroutine(FadeInPopUpOverTime(generalPopUpCanvasGroup, 1.5f));
        StartCoroutine(WaitThenFadePopUpOverTime(generalPopUpCanvasGroup, 3.5f, 1.5f));
    }

    private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount)
    {
        if (duration > 0)
        {
            text.characterSpacing = 0;
            float timer = 0;

            yield return null;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * (Time.deltaTime / 20));

                yield return null;
            }
        }

        yield return null;
    }

    private IEnumerator FadeInPopUpOverTime(CanvasGroup canvas, float duration)
    {
        if (duration > 0)
        {
            canvas.alpha = 0;
            float timer = 0;

            yield return null;

            while(timer < duration)
            {
                timer += Time.deltaTime;
                canvas.alpha = Mathf.Lerp(0, 1, timer / duration);

                yield return null;
            }
        }

        canvas.alpha = 1;
        yield return null;
    }

    private IEnumerator WaitThenFadePopUpOverTime(CanvasGroup canvas, float delay, float duration)
    {
        if (duration > 0)
        {
            while (delay > 0)
            {
                delay -= Time.deltaTime;

                yield return null;
            }

            canvas.alpha = 1;
            float timer = 0;

            yield return null;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                canvas.alpha = Mathf.Lerp(1, 0, timer / duration);

                yield return null;
            }
        }

        canvas.alpha = 0;
        yield return null;
    }
}
