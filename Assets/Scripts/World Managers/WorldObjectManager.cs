using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WorldObjectManager : MonoBehaviour
{
    public static WorldObjectManager instance;

    [Header("Network Objects")]
    [SerializeField] List<NetworkObjectSpawner> networkObjectSpawners;
    [SerializeField] List<GameObject> spawnedInObjects;

    [Header("Fog Walls")]
    public List<FogWallInteractable> fogWalls;

    public void SpawnObject(NetworkObjectSpawner networkObjectSpawner)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            networkObjectSpawners.Add(networkObjectSpawner);
            networkObjectSpawner.AttemptToSpawnObject();
        }
    }

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

    public void AddFogWallToList(FogWallInteractable fogWall)
    {
        if (!fogWalls.Contains(fogWall))
        {
            fogWalls.Add(fogWall);
        }
    }

    public void RemoveFogWallToList(FogWallInteractable fogWall)
    {
        if (fogWalls.Contains(fogWall))
        {
            fogWalls.Remove(fogWall);
        }
    }
}
