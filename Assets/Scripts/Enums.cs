using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{

}

public enum CharacterSlot
{
    CharacterSlot_01,
    CharacterSlot_02,
    CharacterSlot_03,
    CharacterSlot_04,
    CharacterSlot_05,
    CharacterSlot_06,
    CharacterSlot_07,
    CharacterSlot_08,
    CharacterSlot_09,
    CharacterSlot_10,
    NO_SLOT
}

public enum CharacterGroup
{
    Good,
    Bad,
    //Neutral
}

public enum WeaponSlot
{
    RightHand,
    LefHand,
}

public enum AttackType
{
    LightAttack01,
    LightAttack02,
    LightAttack03,
    HeavyAttack01,
    HeavyChargedAttack01,
    RunningAttack01,
    HeavyRunningAttack01,
    DodgeAttack01,
    DuckAttack01,
    JumpAttack01,
    HeavyJumpAttack01
}

public enum ItemPickUpType
{
    WorldSpawn,
    CharacterDrop
}

public enum EquipmentType
{
    RightWeapon01, // 0
    RightWeapon02, // 1
    RightWeapon03  // 2
}