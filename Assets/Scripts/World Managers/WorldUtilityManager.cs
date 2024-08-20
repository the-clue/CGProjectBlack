using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUtilityManager : MonoBehaviour
{
    public static WorldUtilityManager instance;

    [SerializeField] LayerMask characterLayers;
    [SerializeField] LayerMask environmentLayers;

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

    public LayerMask GetCharacterLayers()
    {
        return characterLayers;
    }

    public LayerMask GetEnvironmentLayers() 
    { 
        return environmentLayers;
    }

    public bool IsTargetAttackable(CharacterGroup attackingCharacter, CharacterGroup targetCharacter)
    {
        if (attackingCharacter == CharacterGroup.Good)
        {
            switch (targetCharacter)
            {
                case CharacterGroup.Good:
                    return false;
                case CharacterGroup.Bad:
                    return true;
                default:
                    break;
            }
        }
        else if (attackingCharacter == CharacterGroup.Bad)
        {
            switch (targetCharacter)
            {
                case CharacterGroup.Good:
                    return true;
                case CharacterGroup.Bad:
                    return false;
                default:
                    break;
            }
        }

        return false;
    }

    public float GetAngleOfTarget(Transform characterTransform, Vector3 targetDirection)
    {
        targetDirection.y = 0;
        float viewableAngle = Vector3.Angle(characterTransform.forward, targetDirection);
        Vector3 cross = Vector3.Cross(characterTransform.forward, targetDirection);

        if (cross.y < 0)
        {
            viewableAngle = -viewableAngle;
        }

        return viewableAngle;
    }
}
