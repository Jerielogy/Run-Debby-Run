using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Asset Pools")]
    public GameObject[] groundObstacles;
    public GameObject[] animalObstacles;

    [Header("Standard Spawn Timing")]
    public float startDelay = 2.0f;
    public float spawnInterval = 3.0f;

    [Header("Voice Control Adjustments")]
    [Tooltip("How long to wait between spawns when voice control is active.")]
    public float voiceSpawnInterval = 6.5f; // Doubled to give room for the 2s delay

    [Header("Lane Markers")]
    public Transform topLane;
    public Transform midLane;
    public Transform bottomLane;

    private float timer;

    void Start()
    {
        if (topLane == null || midLane == null || bottomLane == null)
        {
            Debug.LogError("Gideon, please assign all three lane markers in the Inspector!");
        }

        // Initialize timer
        timer = spawnInterval - startDelay;
    }

    void Update()
    {
        if (Time.timeScale == 0) return;

        // 1. Determine which interval to use based on VoiceController status
        float currentInterval = DetermineCurrentInterval();

        timer += Time.deltaTime;

        // 2. Spawn based on the chosen interval
        if (timer >= currentInterval)
        {
            SpawnCategorizedObstacle();
            timer = 0;
        }
    }

    float DetermineCurrentInterval()
    {
        // Check if the VoiceController exists and is currently enabled
        if (VoiceController.Instance != null && VoiceController.Instance.enabled)
        {
            return voiceSpawnInterval;
        }

        return spawnInterval;
    }

    void SpawnCategorizedObstacle()
    {
        int lane = Random.Range(0, 3);

        GameObject prefabToSpawn = null;
        Transform targetTransform = null;

        if (lane == 2) // BOTTOM
        {
            if (groundObstacles.Length > 0)
            {
                prefabToSpawn = groundObstacles[Random.Range(0, groundObstacles.Length)];
                targetTransform = bottomLane;
            }
        }
        else // TOP OR MID
        {
            if (animalObstacles.Length > 0)
            {
                prefabToSpawn = animalObstacles[Random.Range(0, animalObstacles.Length)];
                targetTransform = (lane == 0) ? topLane : midLane;
            }
        }

        if (prefabToSpawn != null && targetTransform != null)
        {
            Instantiate(prefabToSpawn, targetTransform.position, prefabToSpawn.transform.rotation);
        }
    }
}