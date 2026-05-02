using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Asset Pools")]
    public GameObject[] groundObstacles; // Put your 1-2 plant assets here
    public GameObject[] animalObstacles; // Put your animal assets here

    [Header("Spawn Timing")]
    public float startDelay = 2.0f;
    public float spawnInterval = 3.0f;

    [Header("Lane Markers")]
    public Transform topLane;
    public Transform midLane;
    public Transform bottomLane;

    private float timer;

    void Start()
    {
        // Safety check to ensure all points are assigned
        if (topLane == null || midLane == null || bottomLane == null)
        {
            Debug.LogError("Gideon, please assign all three lane markers in the Inspector!");
        }

        timer = spawnInterval - startDelay;
    }

    void Update()
    {
        if (Time.timeScale == 0) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnCategorizedObstacle();
            timer = 0;
        }
    }

    void SpawnCategorizedObstacle()
    {
        // 1. Pick the lane first
        int lane = Random.Range(0, 3); // 0 = Top, 1 = Mid, 2 = Bottom

        GameObject prefabToSpawn = null;
        Transform targetTransform = null;

        // 2. Determine which asset to spawn based on the lane
        if (lane == 2) // BOTTOM LANE (Fixed Assets)
        {
            if (groundObstacles.Length > 0)
            {
                prefabToSpawn = groundObstacles[Random.Range(0, groundObstacles.Length)];
                targetTransform = bottomLane;
            }
        }
        else // TOP OR MID LANE (Animals)
        {
            if (animalObstacles.Length > 0)
            {
                prefabToSpawn = animalObstacles[Random.Range(0, animalObstacles.Length)];
                targetTransform = (lane == 0) ? topLane : midLane;
            }
        }

        // 3. Instantiate if we have a valid selection
        if (prefabToSpawn != null && targetTransform != null)
        {
            Instantiate(prefabToSpawn, targetTransform.position, prefabToSpawn.transform.rotation);
        }
    }
}