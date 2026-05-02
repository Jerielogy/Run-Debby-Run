using UnityEngine;

public class MasterSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform groundSpawnPoint; // Drag Spawner_Ground here
    public Transform airSpawnPoint;    // Drag Spawner_Air here

    [Header("Obstacle Prefabs")]
    public GameObject[] groundPrefabs; // Rocks, Dunes, Deer
    public GameObject[] airPrefabs;    // Birds, Bats

    [Header("Global Timing")]
    public float minSpawnTime = 1.5f;
    public float maxSpawnTime = 3.0f;

    private float timer;

    void Start()
    {
        // Start the timer
        SetNextSpawnTime();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnRandomObstacle();
            SetNextSpawnTime();
        }
    }

    void SpawnRandomObstacle()
    {
        
        // adjust this to make Ground more common (e.g., > 0.3f)
        bool spawnGround = Random.value > 0.4f; // 60% Chance for Ground, 40% Air

        if (spawnGround)
        {
            SpawnGround();
        }
        else
        {
            SpawnAir();
        }
    }

    void SpawnGround()
    {
        if (groundPrefabs.Length == 0) return;
        int index = Random.Range(0, groundPrefabs.Length);

        GameObject newObj = Instantiate(groundPrefabs[index], groundSpawnPoint.position, Quaternion.identity);
        Destroy(newObj, 10f);
    }

    void SpawnAir()
    {
        if (airPrefabs.Length == 0) return;
        int index = Random.Range(0, airPrefabs.Length);

        GameObject newObj = Instantiate(airPrefabs[index], airSpawnPoint.position, Quaternion.identity);
        Destroy(newObj, 10f); 
    }

    void SetNextSpawnTime()
    {
        timer = Random.Range(minSpawnTime, maxSpawnTime);
    }
}