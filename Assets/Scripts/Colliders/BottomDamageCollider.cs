using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomDamageCollider : DamageCollider
{
    protected override void DamageTarget(CharacterManager damageTarget)
    {
        damageTarget.characterNetworkManager.currentHealth.Value = 0;
    }
}
