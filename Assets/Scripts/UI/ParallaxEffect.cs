using UnityEngine;
using System.Collections.Generic;

public class ParallaxEffect : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public string name;            // Name for organization (e.g., "Sky", "Clouds")
        public Sprite layerSprite;     // Drag your asset here
        [Range(0, 1)]
        public float parallaxFactor;   // 0 = Far distance, 1 = Close foreground
        public float yOffset = 0;      // Adjust the height of this specific layer
        public int sortingOrder = -10; // Ensure layers don't overlap wrongly
    }

    [Header("Setup")]
    public GameObject cam;             // Drag Main Camera here
    public List<ParallaxLayer> layers; // Drag your 7 assets here

    private List<GameObject> activeLayers = new List<GameObject>();
    private List<float> startPositions = new List<float>();
    private List<float> lengths = new List<float>();

    void Start()
    {
        if (cam == null) cam = Camera.main.gameObject;

        // This automatically sets up your 7 layers so you don't have to do it manually
        foreach (var layer in layers)
        {
            // 1. Create a parent for the two "Leapfrog" sprites
            GameObject layerParent = new GameObject("Layer_" + layer.name);
            layerParent.transform.SetParent(this.transform);

            // 2. Create the two sprites needed for infinite looping
            for (int i = 0; i < 2; i++)
            {
                GameObject obj = new GameObject(layer.name + "_" + i);
                obj.transform.SetParent(layerParent.transform);

                SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
                sr.sprite = layer.layerSprite;
                sr.sortingOrder = layer.sortingOrder;

                // Position them side-by-side
                float width = sr.bounds.size.x;
                obj.transform.position = new Vector3(i * width, layer.yOffset, 0);
            }

            activeLayers.Add(layerParent);
            startPositions.Add(0);
            lengths.Add(layer.layerSprite.bounds.size.x);
        }
    }

    void LateUpdate()
    {
        // --- THE FIX: ADD THIS CHECK ---
        if (Time.timeScale == 0) return;

        for (int i = 0; i < layers.Count; i++)
        {
            float temp = (cam.transform.position.x * (1 - layers[i].parallaxFactor));
            float dist = (cam.transform.position.x * layers[i].parallaxFactor);

            activeLayers[i].transform.position = new Vector3(startPositions[i] + dist, activeLayers[i].transform.position.y, 0);

            if (temp > startPositions[i] + lengths[i]) startPositions[i] += lengths[i];
            else if (temp < startPositions[i] - lengths[i]) startPositions[i] -= lengths[i];
        }
    }
}