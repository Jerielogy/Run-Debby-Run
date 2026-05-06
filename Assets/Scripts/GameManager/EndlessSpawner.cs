using UnityEngine;

public class EndlessSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] obstacles; // Drag your obstacle prefabs here
    public Transform spawnPoint;   // Position off-screen to the right

    [Header("Timing")]
    public float minDelay = 1.2f;
    public float maxDelay = 2.5f;

    private float nextSpawnTime;
    private float baseSpeed = 5f; // Matches the starting speed in EndlessManager

    void Start()
    {
        CalculateNextSpawn();
    }

    void Update()
    {
        // 1. Stop spawning if the game is over
        if (EndlessManager.Instance == null || EndlessManager.Instance.isGameOver)
            return;

        // 2. Check if it's time to spawn the next obstacle
        if (Time.time >= nextSpawnTime)
        {
            SpawnObstacle();
            CalculateNextSpawn();
        }
    }

    void SpawnObstacle()
    {
        if (obstacles.Length == 0) return;

        // Pick a random obstacle from the array
        int index = Random.Range(0, obstacles.Length);

        // Instantiate at the spawn point
        GameObject newObstacle = Instantiate(obstacles[index], spawnPoint.position, Quaternion.identity);

        // Ensure it gets cleaned up after 10 seconds if it somehow misses the destruction trigger
        Destroy(newObstacle, 10f);
    }

    void CalculateNextSpawn()
    {
        // Difficulty Scaling: 
        // As worldSpeed increases, the delay decreases so obstacles don't feel too far apart.
        float currentSpeed = EndlessManager.Instance.worldSpeed;
        float speedFactor = baseSpeed / currentSpeed;

        float randomDelay = Random.Range(minDelay, maxDelay) * speedFactor;
        nextSpawnTime = Time.time + randomDelay;
    }
}