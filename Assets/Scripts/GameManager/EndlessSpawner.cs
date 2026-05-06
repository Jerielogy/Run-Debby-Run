using UnityEngine;

public class EndlessSpawner : MonoBehaviour
{
    public GameObject[] obstacles;
    public Transform spawnPoint;

    public float minDelay = 3f;
    public float maxDelay = 6f;

    private float nextSpawnTime;

    void Start()
    {
        SetNextSpawnTime();
    }

    void Update()
    {
        if (EndlessManager.Instance == null || EndlessManager.Instance.isGameOver || EndlessManager.Instance.isCountingDown) return;

        if (Time.time >= nextSpawnTime)
        {
            // --- THE FIX: GLOBAL GAP CHECK ---
            // If another spawner JUST spawned something, push our spawn time back slightly
            float timeSinceLastGlobalSpawn = Time.time - EndlessManager.Instance.lastSpawnTime;

            if (timeSinceLastGlobalSpawn < EndlessManager.Instance.minGapBetweenSpawners)
            {
                // Delay this specific spawn by a tiny bit so they don't overlap
                nextSpawnTime = Time.time + 0.5f;
                return;
            }

            SpawnObstacle();
            SetNextSpawnTime();
        }
    }

    void SpawnObstacle()
    {
        // Change obstacles.length to obstacles.Length
        int randomIndex = Random.Range(0, obstacles.Length);
        Instantiate(obstacles[randomIndex], spawnPoint.position, Quaternion.identity);

        EndlessManager.Instance.lastSpawnTime = Time.time;
    }

    void SetNextSpawnTime()
    {
        // Adjusts spawn frequency based on world speed
        float currentSpeed = Mathf.Max(EndlessManager.Instance.worldSpeed, 1f);
        float speedFactor = 5f / currentSpeed;

        nextSpawnTime = Time.time + Random.Range(minDelay, maxDelay) * speedFactor;
    }
}