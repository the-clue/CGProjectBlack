using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] public MeleeWeaponDamageCollider meleeWeaponDamageCollider;

    private void Awake()
    {
        meleeWeaponDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
    }

    public void SetWeaponDamage(CharacterManager characterWieldingWeapon, WeaponItem weapon)
    {
        meleeWeaponDamageCollider.characterCausingDamage = characterWieldingWeapon;
        meleeWeaponDamageCollider.physicalDamage = weapon.physicalDamage;
        meleeWeaponDamageCollider.magicDamage = weapon.magicalDamage;
        meleeWeaponDamageCollider.poiseDamage = weapon.poiseDamage;

        meleeWeaponDamageCollider.light_Attack_01_Modifier = weapon.light_Attack_01_Modifier;
        meleeWeaponDamageCollider.light_Attack_02_Modifier = weapon.light_Attack_02_Modifier;
        meleeWeaponDamageCollider.light_Attack_03_Modifier = weapon.light_Attack_03_Modifier;
        meleeWeaponDamageCollider.running_Attack_01_Modifier = weapon.running_Attack_01_Modifier;
        meleeWeaponDamageCollider.dodge_Attack_01_Modifier = weapon.dodge_Attack_01_Modifier;
        meleeWeaponDamageCollider.duck_Attack_01_Modifier = weapon.duck_Attack_01_Modifier;
        meleeWeaponDamageCollider.jump_Attack_01_Modifier = weapon.jump_Attack_01_Modifier;
        meleeWeaponDamageCollider.heavy_Attack_01_Modifier = weapon.heavy_Attack_01_Modifier;
        meleeWeaponDamageCollider.heavy_Charged_Attack_01_Modifier = weapon.heavy_Charged_Attack_01_Modifier;
        meleeWeaponDamageCollider.heavy_Running_Attack_01_Modifier = weapon.heavy_Running_Attack_01_Modifier;
        meleeWeaponDamageCollider.heavy_Jump_Attack_01_Modifier = weapon.heavy_Jump_Attack_01_Modifier;
    }
}
