using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    public float startDelay = 2.0f;
    public float spawnInterval = 3.0f;

    [Header("Lane Markers (Empty GameObjects)")]
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform spawnPoint3;

    private float timer;

    void Start()
    {
        // Safety check to make sure you didn't forget to assign the points
        if (spawnPoint1 == null || spawnPoint2 == null || spawnPoint3 == null)
        {
            Debug.LogError("Gideon, you forgot to drag the spawn points into the Inspector!");
        }

        timer = spawnInterval - startDelay;
    }

    void Update()
    {
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

        // 1. Pick a random obstacle
        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject prefabToSpawn = obstaclePrefabs[randomIndex];

        // 2. Pick a random lane using the markers
        Transform chosenLane = GetRandomLane();

        if (chosenLane != null)
        {
            // 3. Spawn at the marker's exact position
            Instantiate(prefabToSpawn, chosenLane.position, prefabToSpawn.transform.rotation);
        }
    }

    // Helper function to pick one of the three transforms
    Transform GetRandomLane()
    {
        int lane = Random.Range(0, 3); // Picks 0, 1, or 2

        if (lane == 0) return spawnPoint1;
        if (lane == 1) return spawnPoint2;
        return spawnPoint3;
    }
}