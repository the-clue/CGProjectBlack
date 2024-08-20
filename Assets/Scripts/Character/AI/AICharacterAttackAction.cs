using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Attack")]
public class AICharacterAttackAction : ScriptableObject
{
    [Header("Attack")]
    [SerializeField] private string attackAnimation;

    [Header("Combo Action")]
    public AICharacterAttackAction comboAction;

    [Header("Action Values")]
    [SerializeField] AttackType attackType;
    public int attackWeight = 10;
    public float actionRecoveryTime = 1.5f; // Time before the character can perform another action after performing this one
    public float minimumAttackAngle = -45;
    public float maximumAttackAngle = 45;
    public float minimumAttackDistance = 0;
    public float maximumAttackDistance = 2;

    public void AttemptToPerformAction(AICharacterManager aiCharacter)
    {
        // Use this if NPC uses weapon based animations (ex. Invaders)
        // aiCharacter.characterAnimatorManager.PlayTargetAttackActionAnimation(weapon, attackAnimation, true);
        
        aiCharacter.characterAnimatorManager.PlayTargetActionAnimation(attackAnimation, true);
    }
}
