using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    CharacterManager character;

    [Header("VFX")]
    [SerializeField] GameObject bloodSplashVFX;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
    {
        effect.ProcessEffect(character);
    }

    public void PlayBloodSplashVFX(Vector3 contactPoint)
    {
        if (bloodSplashVFX != null) // if we have a specific blood splash vfx on this model, then use that
        {
            GameObject bloodSplash = Instantiate(bloodSplashVFX, contactPoint, Quaternion.identity);
        }
        else // else use generic 
        {
            GameObject bloodSplash = Instantiate(WorldCharacterEffectsManager.instance.bloodSplashVFX, contactPoint, Quaternion.identity);
        }
    }

}
