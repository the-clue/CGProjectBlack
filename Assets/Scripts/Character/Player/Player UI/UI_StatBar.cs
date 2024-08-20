using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatBar : MonoBehaviour
{
    protected Slider slider;
    // Variable to scale bar size depending on stat (higher stat = longer bar across screen)
    // Secondary bar behind main bar for polish effect (yellow bar)
    protected RectTransform rectTransform;

    [Header("Bar Options")]
    [SerializeField] protected bool scaleBarLengthWithStats = true;
    [SerializeField] protected float widthScaleMultiplier = 1;

    protected virtual void Awake()
    {
        slider = GetComponent<Slider>();
        rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void Start()
    {

    }

    public virtual void SetStat(int newValue)
    {
        slider.value = newValue;
    }

    public virtual void SetMaxStat(int maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = maxValue;

        if (scaleBarLengthWithStats)
        {
            rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);
            // Resets bars positions
            PlayerUIManager.instance.playerUIHudManager.RefreshHUD();
        }
    }
}
