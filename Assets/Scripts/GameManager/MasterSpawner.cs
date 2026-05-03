using UnityEngine;

public class MasterSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform groundSpawnPoint;
    public Transform airSpawnPoint;

    [Header("Obstacle Prefabs")]
    public GameObject[] groundPrefabs;
    public GameObject[] airPrefabs;

    [Header("Global Timing")]
    public float minSpawnTime = 1.5f;
    public float maxSpawnTime = 3.0f;

    [Header("Voice Control Adjustment")]
    [Tooltip("How much to multiply spawn time when using voice (e.g., 2.0 doubles the wait time).")]
    public float voiceTimeMultiplier = 2.0f;

    private float timer;

    void Start()
    {
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
        bool spawnGround = Random.value > 0.4f;

        if (spawnGround) SpawnGround();
        else SpawnAir();
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
        // 1. Calculate the base random time
        float randomTime = Random.Range(minSpawnTime, maxSpawnTime);

        // 2. If VoiceController is active and enabled, apply the multiplier[cite: 1]
        if (VoiceController.Instance != null && VoiceController.Instance.enabled)
        {
            randomTime *= voiceTimeMultiplier;
        }

        timer = randomTime;
    }
}