using System.Collections.Generic;
using UnityEngine;

public class FallingObjectSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject bananaPrefab;
    [SerializeField] private GameObject coconutPrefab;

    [Header("Spawn Area")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float spawnY = 6f;

    [SerializeField] private float minSpawnDistance = 1.2f;
    [SerializeField] private int maxSpawnAttempts = 10;

    [SerializeField] private int recentSpawnMemory = 5;

    [Header("Timing")]
    [SerializeField] private float spawnInterval = 0.75f;

    [Header("Chance")]
    [SerializeField, Range(0f, 1f)] private float startingCoconutChance = 0.2f;
    [SerializeField, Range(0f, 1f)] private float maxCoconutChance = 0.6f;
    [SerializeField] private float coconutIncreasePerSecond = 0.005f;

    private readonly List<float> recentSpawnXs = new();

    private float spawnTimer;
    private float gamTimer;

    void Update()
    {
        gamTimer += Time.deltaTime;

        spawnTimer -= Time.deltaTime;

        if(spawnTimer <= 0f)
        {
            SpawnObject();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnObject()
    {
        float currentCoconutChance = Mathf.Min(
            startingCoconutChance + gamTimer * coconutIncreasePerSecond,
            maxCoconutChance
        );

        GameObject prefab = Random.value < currentCoconutChance
           ? coconutPrefab
           : bananaPrefab;

        if (prefab == null) return;

        Vector3 spawnPosition = GetSpawnPosition();

        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }

    private Vector3 GetSpawnPosition()
    {
        float x = Random.Range(minX, maxX);

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            bool tooClose = false;

            foreach (float recentX in recentSpawnXs)
            {
                if(Mathf.Abs(x - recentX) < minSpawnDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if(!tooClose)
                break;
            
            x = Random.Range(minX, maxX);
        }

        recentSpawnXs.Add(x);

        if(recentSpawnXs.Count > recentSpawnMemory)
            recentSpawnXs.RemoveAt(0);
        
        return new Vector3(x, spawnY, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            new Vector3(minX, spawnY, 0f),
            new Vector3(maxX, spawnY, 0f)
        );
    }
}
