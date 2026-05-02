using System.Collections;
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

    [Header("Warning")]
    [SerializeField] private GameObject warningPrefab;
    [SerializeField] private float warningDuration = 1f;
    [SerializeField] private float warningYOffsetFromTop = 0.5f;

    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 1.5f;

    [Header("Fall Speed")]
    [SerializeField] private float fallSpeed = 4f;
    [SerializeField] private float minFallingObjectSpeed = 4f;
    [SerializeField] private float maxFallingObjectSpeed = 14f;

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
            return;
        }

        if (warningPrefab == null)
            Debug.LogWarning("No warning prefab assigned. Rocks will spawn without warning.");
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            StartCoroutine(SpawnObjectWithWarning());
        }
    }

    public void SetFallSpeed(float newFallSpeed)
    {
        fallSpeed = Mathf.Clamp(newFallSpeed, minFallingObjectSpeed, maxFallingObjectSpeed);
    }


    private IEnumerator SpawnObjectWithWarning()
    {
        GameObject prefab = GetRandomWeightedObject();

        if (prefab == null)
            yield break;

        float x = Random.Range(minX, maxX);

        if (warningPrefab != null)
        {
            float warningY = targetCamera.transform.position.y
                             + targetCamera.orthographicSize
                             - warningYOffsetFromTop;

            GameObject warning = Instantiate(
                warningPrefab,
                new Vector3(x, warningY, 0f),
                Quaternion.identity
            );

            Destroy(warning, warningDuration);
        }

        yield return new WaitForSeconds(warningDuration);

        SpawnObjectAtPosition(prefab, x);
    }

    private void SpawnObjectAtPosition(GameObject prefab, float x)
    {
        float spawnY = targetCamera.transform.position.y
                       + targetCamera.orthographicSize
                       + spawnYOffset;

        GameObject spawnedObject = Instantiate(
            prefab,
            new Vector3(x, spawnY, 0f),
            Quaternion.identity
        );

        FallingObject fallingObject = spawnedObject.GetComponent<FallingObject>();

        if (fallingObject != null)
            fallingObject.Initialize(GetCurrentRockFallSpeed(), GetDespawnY());
    }

    private float GetCurrentRockFallSpeed()
    {
        return fallSpeed;
    }

    private float GetDespawnY()
    {
        return targetCamera.transform.position.y
               - targetCamera.orthographicSize
               - despawnYOffset;
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
        float currentWeight = 0f;

        for (int i = 0; i < spawnableObjects.Count; i++)
        {
            SpawnableObject spawnableObject = spawnableObjects[i];

            if (spawnableObject.prefab == null)
                continue;

            currentWeight += spawnableObject.weight;

            if (roll <= currentWeight)
                return spawnableObject.prefab;
        }

        return null;
    }
}