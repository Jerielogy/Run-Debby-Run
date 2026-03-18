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

    // --- THE FIX: REMOVED 'hasStarted' TO CLEAR THE WARNING ---
    private float timer;

    void Start()
    {
        // We set the timer so the first spawn happens after the startDelay
        timer = spawnInterval - startDelay;
    }

    void Update()
    {
        // 1. HARD FREEZE: Stops spawning while the Region 1 Intro is open
        if (Time.timeScale == 0) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnObstacle();
            timer = 0;
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject prefabToSpawn = obstaclePrefabs[randomIndex];

        float actualSpawnY = spawnYPosition;

        // Uses your logic to lift "Enemy" types higher than ground hurdles
        if (prefabToSpawn.name.ToLower().Contains("enemy"))
        {
            actualSpawnY += enemyYOffset;
        }

        Vector3 spawnPos = new Vector3(
            transform.position.x,
            actualSpawnY,
            transform.position.z
        );

        Instantiate(prefabToSpawn, spawnPos, prefabToSpawn.transform.rotation);
    }
}