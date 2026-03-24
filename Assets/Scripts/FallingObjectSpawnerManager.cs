using System.Collections.Generic;
using UnityEngine;

public class FallingObjectSpawnerManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableObject
    {
        public GameObject prefab;
        [Range(0f, 100f)] public float weight = 1f;
    }

    [Header("Spawnable Objects")]
    [SerializeField] private List<SpawnableObject> spawnableObjects = new();

    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 1.5f;

    [Header("Fall")]
    [SerializeField] private float fallSpeed = 3f;

    [Header("Spawn Range")]
    [SerializeField] private float minX = -4f;
    [SerializeField] private float maxX = 4f;
    [SerializeField] private float spawnYOffset = 2f;

    [Header("Despawn")]
    [SerializeField] private float despawnYOffset = 2f;

    private float spawnTimer;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
        {
            Debug.LogError("No camera found. Assign Target Camera.");
            enabled = false;
            return;
        }

        if (spawnableObjects == null || spawnableObjects.Count == 0)
        {
            Debug.LogError("Add at least one spawnable object.");
            enabled = false;
        }
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnObject();
        }
    }

    private void SpawnObject()
    {
        GameObject prefab = GetRandomWeightedObject();
        if (prefab == null)
            return;

        float spawnY = targetCamera.transform.position.y + targetCamera.orthographicSize + spawnYOffset;
        float x = Random.Range(minX, maxX);

        GameObject go = Instantiate(prefab, new Vector3(x, spawnY, 0f), Quaternion.identity);

        FallingObject fallingObject = go.GetComponent<FallingObject>();
        if (fallingObject != null)
            fallingObject.Initialize(fallSpeed, GetDespawnY());
    }

    private float GetDespawnY()
    {
        return targetCamera.transform.position.y - targetCamera.orthographicSize - despawnYOffset;
    }

    private GameObject GetRandomWeightedObject()
    {
        float totalWeight = 0f;

        for (int i = 0; i < spawnableObjects.Count; i++)
        {
            if (spawnableObjects[i].prefab != null)
                totalWeight += spawnableObjects[i].weight;
        }

        if (totalWeight <= 0f)
            return null;

        float roll = Random.Range(0f, totalWeight);
        float current = 0f;

        for (int i = 0; i < spawnableObjects.Count; i++)
        {
            if (spawnableObjects[i].prefab == null)
                continue;

            current += spawnableObjects[i].weight;
            if (roll <= current)
                return spawnableObjects[i].prefab;
        }

        return null;
    }
}