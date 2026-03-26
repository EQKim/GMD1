using System.Collections.Generic;
using UnityEngine;

public class EndlessPlatformManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableItem
    {
        public GameObject prefab;
        [Range(0f, 100f)] public float weight = 1f;
    }

    private class SpawnedItemData
    {
        public GameObject itemObject;
        public float xOffset;
    }

    [Header("Prefab / Pool")]
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private int poolSize = 12;

    [Header("Movement")]
    [SerializeField] private float fallSpeed = 2.5f;

    [Header("Camera-based bounds")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float verticalBuffer = 1.5f;

    [Header("Lane positions")]
    [SerializeField] private float[] laneXPositions = new float[] { -5f, -2.5f, 0f, 2.5f, 5f };

    [Header("Vertical spacing")]
    [SerializeField] private float minGapY = 0.8f;
    [SerializeField] private float maxGapY = 1.6f;

    [Header("Items / Weapons / Powerups")]
    [SerializeField] private List<SpawnableItem> spawnableItems = new();
    [SerializeField, Range(0f, 1f)] private float itemSpawnChance = 0.35f;
    [SerializeField] private float itemYOffset = 0.9f;
    [SerializeField] private float itemRandomXOffset = 0.4f;

    private readonly List<Rigidbody2D> platforms = new();
    private readonly Dictionary<Rigidbody2D, SpawnedItemData> platformItems = new();

    private int lastLaneIndex = -1;

    private void Awake()
    {
        if (platformPrefab == null)
        {
            Debug.LogError("Assign Platform Prefab in the Inspector.");
            enabled = false;
            return;
        }

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
        {
            Debug.LogError("No camera found. Assign Target Camera.");
            enabled = false;
            return;
        }

        if (laneXPositions == null || laneXPositions.Length == 0)
        {
            Debug.LogError("Add at least one lane X position.");
            enabled = false;
            return;
        }

        float spawnY = GetSpawnY();
        float y = spawnY;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject go = Instantiate(platformPrefab, transform);

            Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("Platform prefab must have a Rigidbody2D.");
                enabled = false;
                return;
            }

            float x = GetNextLaneX();
            rb.position = new Vector2(x, y);

            platforms.Add(rb);
            platformItems[rb] = null;

            TrySpawnItemOnPlatform(rb);

            y += Random.Range(minGapY, maxGapY);
        }
    }

    private void FixedUpdate()
    {
        float dy = fallSpeed * Time.fixedDeltaTime;
        float despawnY = GetDespawnY();

        for (int i = 0; i < platforms.Count; i++)
        {
            Rigidbody2D rb = platforms[i];
            Vector2 p = rb.position;
            p.y -= dy;

            if (p.y < despawnY)
            {
                ClearItem(rb);

                float highestY = GetHighestPlatformY();
                float newY = highestY + Random.Range(minGapY, maxGapY);
                float newX = GetNextLaneX();
                p = new Vector2(newX, newY);

                rb.position = p;
                TrySpawnItemOnPlatform(rb);
            }
            else
            {
                rb.MovePosition(p);
            }

            UpdateItemPosition(rb);
        }
    }

    private float GetNextLaneX()
    {
        if (laneXPositions.Length == 1)
            return laneXPositions[0];

        int newLane;

        do
        {
            newLane = Random.Range(0, laneXPositions.Length);
        }
        while (newLane == lastLaneIndex);

        lastLaneIndex = newLane;
        return laneXPositions[newLane];
    }

    private void TrySpawnItemOnPlatform(Rigidbody2D platform)
    {
        if (spawnableItems == null || spawnableItems.Count == 0)
            return;

        if (Random.value > itemSpawnChance)
            return;

        GameObject prefabToSpawn = GetRandomWeightedItem();
        if (prefabToSpawn == null)
            return;

        float xOffset = Random.Range(-itemRandomXOffset, itemRandomXOffset);

        GameObject item = Instantiate(prefabToSpawn);
        item.transform.position = new Vector3(
            platform.position.x + xOffset,
            platform.position.y + itemYOffset,
            0f
        );

        platformItems[platform] = new SpawnedItemData
        {
            itemObject = item,
            xOffset = xOffset
        };
    }

    private void UpdateItemPosition(Rigidbody2D platform)
    {
        if (!platformItems.TryGetValue(platform, out SpawnedItemData data))
            return;

        if (data == null || data.itemObject == null)
            return;

        data.itemObject.transform.position = new Vector3(
            platform.position.x + data.xOffset,
            platform.position.y + itemYOffset,
            0f
        );
    }

    private GameObject GetRandomWeightedItem()
    {
        float totalWeight = 0f;

        for (int i = 0; i < spawnableItems.Count; i++)
        {
            if (spawnableItems[i].prefab != null)
                totalWeight += spawnableItems[i].weight;
        }

        if (totalWeight <= 0f)
            return null;

        float roll = Random.Range(0f, totalWeight);
        float current = 0f;

        for (int i = 0; i < spawnableItems.Count; i++)
        {
            SpawnableItem item = spawnableItems[i];
            if (item.prefab == null)
                continue;

            current += item.weight;
            if (roll <= current)
                return item.prefab;
        }

        return null;
    }

    private void ClearItem(Rigidbody2D platform)
    {
        if (!platformItems.TryGetValue(platform, out SpawnedItemData data))
            return;

        if (data != null && data.itemObject != null)
            Destroy(data.itemObject);

        platformItems[platform] = null;
    }

    private float GetHighestPlatformY()
    {
        float highest = float.NegativeInfinity;
        for (int i = 0; i < platforms.Count; i++)
        {
            float y = platforms[i].position.y;
            if (y > highest)
                highest = y;
        }
        return highest;
    }

    private float GetSpawnY()
    {
        return targetCamera.transform.position.y + targetCamera.orthographicSize + verticalBuffer;
    }

    private float GetDespawnY()
    {
        return targetCamera.transform.position.y - targetCamera.orthographicSize - verticalBuffer;
    }
}