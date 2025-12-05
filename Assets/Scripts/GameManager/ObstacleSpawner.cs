using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;

    public float startDelay = 2.0f;

    public float spawnInterval = 3.0f;

    public float enemyYOffset = 2.0f;

    public float spawnYPosition = -3.5f;

    
    void Start()
    {
        InvokeRepeating("SpawnObstacle", startDelay, spawnInterval);
    }

    void SpawnObstacle()
    {
        // 1. Pick a random obstacle prefab
        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject prefabToSpawn = obstaclePrefabs[randomIndex];

        // --- NEW SIMPLER LOGIC: Check the prefab's NAME ---
        float actualSpawnY = spawnYPosition; // Default to hurdle height

        // If the prefab's name contains "Enemy" (case-insensitive)
        if (prefabToSpawn.name.ToLower().Contains("enemy"))
        {
            // Add the extra height
            actualSpawnY += enemyYOffset;
        }
        // ----------------------------------------------------

        // 3. Calculate the full spawn position
        Vector3 spawnPos = new Vector3(
            transform.position.x,   // Use the spawner's X
            actualSpawnY,           // Use the calculated Y
            transform.position.z    // Use the spawner's Z
        );

        // 4. Spawn the obstacle
        Instantiate(prefabToSpawn, spawnPos, prefabToSpawn.transform.rotation);
    }
}
