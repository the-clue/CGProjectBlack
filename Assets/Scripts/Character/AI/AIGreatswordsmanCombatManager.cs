using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGreatswordsmanCombatManager : AICharacterCombatManager
{
    [Header("Damage Colliders")]
    [SerializeField] GreatswordsmanWeaponCollider greatSwordDamageCollider;

    [Header("Damage")]
    [SerializeField] int baseDamage = 25;
    [SerializeField] float attack01DamageModifier = 1.0f;
    [SerializeField] float attack02DamageModifier = 1.2f;

    public void SetAttack01Damage()
    {
        greatSwordDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
    }
    public void SetAttack02Damage()
    {
        greatSwordDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
    }

    public void OpenGreatSwordDamageCollider()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        greatSwordDamageCollider.EnableDamageCollider();
    }
    public void CloseGreatSwordDamageCollider()
    {
        greatSwordDamageCollider.DisableDamageCollider();
    }
}
