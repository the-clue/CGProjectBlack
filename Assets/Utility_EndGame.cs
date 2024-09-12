using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Utility_EndGame : MonoBehaviour
{
    [SerializeField] float delay = 10f;

    private void Awake()
    {
        StartCoroutine(LoadMainMenuAfterDelay());
    }

    private IEnumerator LoadMainMenuAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        NetworkManager.Singleton.Shutdown();

        WorldSaveGameManager.instance.LoadWorldScene(0);
    }
}
