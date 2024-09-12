using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Status")]
public class StatusItem : Item
{
    [Header("Parameters")]
    public int statusType = 0; // 0: Vitality | 1: Endurance
    public int statusPoints = 0;
}