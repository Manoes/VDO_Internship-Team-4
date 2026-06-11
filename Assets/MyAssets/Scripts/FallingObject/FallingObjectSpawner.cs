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

    [Header("Timing")]
    [SerializeField] private float spawnInterval = 0.75f;

    [Header("Chance")]
    [SerializeField, Range(0f, 1f)] private float coconutChance = 0.2f;

    private float spawnTimer;

    void Update()
    {
        GameObject prefab = Random.value < coconutChance
            ? coconutPrefab
            : bananaPrefab;
        
        if(prefab == null) return;

        Vector3 spawnPosition = new Vector3(
            Random.Range(minX, maxX),
            spawnY,
            0f
        );

        Instantiate(prefab, spawnPosition, Quaternion.identity);
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
