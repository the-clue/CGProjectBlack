using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerSave : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        WorldSaveGameManager.instance.SaveGame();
    }
}
