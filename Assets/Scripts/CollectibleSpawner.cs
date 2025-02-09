using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManagerSO; // Reference to ScriptableObject
    [SerializeField] private Transform collectibleParent;
    private int totalCollectibles;

    private void Start()
    {
        if (gameManagerSO == null || collectibleParent == null)
        {
            Debug.LogWarning("GameManagerSO or CollectibleParent is missing in CollectibleSpawner!");
            return;
        }

        totalCollectibles = collectibleParent.childCount;

        Debug.Log($"CollectibleSpawner detected {totalCollectibles} collectibles in parent.");

        gameManagerSO.SetTotalCollectibles(totalCollectibles); // Esto actualiza el total en GameManagerSO
        SpawnCollectibles();

        //int total = collectibleParent.childCount;
        //gameManagerSO.SetTotalCollectibles(total); // Esto activará el evento y actualizará la UI
        //SpawnCollectibles();

    }

    private void SpawnCollectibles()
    {
        if (gameManagerSO == null || gameManagerSO.CollectiblePrefab == null || collectibleParent == null)
        {
            Debug.LogWarning("GameManagerSO, Collectible Prefab, or Collectible Parent is missing!");
            return;
        }

        foreach (Transform spawnPoint in collectibleParent)
        {
            Vector3 globalPosition = spawnPoint.position; // Use world position
            Instantiate(gameManagerSO.CollectiblePrefab, globalPosition, Quaternion.identity);
        }
    }
}
