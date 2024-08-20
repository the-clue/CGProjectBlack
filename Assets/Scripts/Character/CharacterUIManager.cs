using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUIManager : MonoBehaviour
{
    [Header("UI")]
    public bool hasFloatingHealthBar = true;
    public UI_Character_Health_Bar characterHealthBar;

    public void OnHealthChanged(int oldValue, int newValue)
    {
        characterHealthBar.oldHealthValue = oldValue;
        characterHealthBar.SetStat(newValue);
    }
}
