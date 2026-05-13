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
    [SerializeField] private float startingFallSpeed = 2.5f;
    [SerializeField] private float maxFallSpeed = 8f;

    [Header("Speed Progression")]
    [SerializeField] private bool enableSpeedIncrease = true;
    [SerializeField] private float speedIncreaseInterval = 30f;
    [SerializeField] private float speedIncreaseAmount = 0.5f;

    [Header("Speed Increase Audio")]
    [SerializeField] private AudioSource speedIncreaseAudioSource;
    [SerializeField] private AudioClip speedIncreaseSfx;
    [Range(0f, 1f)]
    [SerializeField] private float speedIncreaseVolume = 1f;

    [Header("Linked Managers")]
    [SerializeField] private EndlessBackgroundManager backgroundManager;
    [SerializeField] private FallingObjectSpawnerManager fallingObjectManager;

    [Header("Linked Speed Multipliers")]
    [SerializeField] private float backgroundSpeedMultiplier = 0.6f;
    [SerializeField] private float fallingObjectSpeedMultiplier = 1.5f;

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
    private float currentFallSpeed;
    private float speedIncreaseTimer;
    private bool isRunning;

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

        if (speedIncreaseAudioSource != null)
        {
            speedIncreaseAudioSource.playOnAwake = false;
            speedIncreaseAudioSource.loop = false;
            speedIncreaseAudioSource.spatialBlend = 0f;
        }

        ResetRuntimeState();
        ClearAllPlatforms();
    }

    private void Update()
    {
        if (!isRunning)
            return;

        HandleSpeedIncreaseTimer();
    }

    private void FixedUpdate()
    {
        if (!isRunning)
            return;

        float dy = currentFallSpeed * Time.fixedDeltaTime;
        float despawnY = GetDespawnY();

        for (int i = 0; i < platforms.Count; i++)
        {
            Rigidbody2D rb = platforms[i];

            if (rb == null)
                continue;

            Vector2 position = rb.position;
            position.y -= dy;

            if (position.y < despawnY)
            {
                ClearItem(rb);

                float highestY = GetHighestPlatformY();
                float newY = highestY + Random.Range(minGapY, maxGapY);
                float newX = GetNextLaneX();

                position = new Vector2(newX, newY);
                rb.position = position;

                TrySpawnItemOnPlatform(rb);
            }
            else
            {
                rb.MovePosition(position);
            }

            UpdateItemPosition(rb);
        }
    }

    public void BeginRun()
    {
        StopRun();

        ResetRuntimeState();
        BuildPlatformPool();

        isRunning = true;
    }

    public void StopRun()
    {
        isRunning = false;
        ResetRuntimeState();
        ClearAllPlatforms();
    }

    public void ResetSpeedProgression()
    {
        currentFallSpeed = startingFallSpeed;
        speedIncreaseTimer = 0f;
        UpdateLinkedManagerSpeeds();
    }

    public float GetCurrentFallSpeed()
    {
        return currentFallSpeed;
    }

    public FallingObjectSpawnerManager GetFallingObjectManager()
    {
        return fallingObjectManager;
    }

    public EndlessBackgroundManager GetBackgroundManager()
    {
        return backgroundManager;
    }

    public void SetSpeedIncreaseVolume(float volume)
    {
        speedIncreaseVolume = Mathf.Clamp01(volume);
    }

    private void ResetRuntimeState()
    {
        currentFallSpeed = startingFallSpeed;
        speedIncreaseTimer = 0f;
        lastLaneIndex = -1;
        UpdateLinkedManagerSpeeds();
    }

    private void BuildPlatformPool()
    {
        ClearAllPlatforms();

        float y = GetSpawnY();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject platformObject = Instantiate(platformPrefab, transform);

            Rigidbody2D rb = platformObject.GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                Debug.LogError("Platform prefab must have a Rigidbody2D.");
                Destroy(platformObject);
                continue;
            }

            float x = GetNextLaneX();
            rb.position = new Vector2(x, y);

            platforms.Add(rb);
            platformItems[rb] = null;

            TrySpawnItemOnPlatform(rb);

            y += Random.Range(minGapY, maxGapY);
        }
    }

    private void ClearAllPlatforms()
    {
        for (int i = 0; i < platforms.Count; i++)
        {
            Rigidbody2D rb = platforms[i];

            if (rb == null)
                continue;

            ClearItem(rb);
            Destroy(rb.gameObject);
        }

        platforms.Clear();
        platformItems.Clear();
    }

    private void HandleSpeedIncreaseTimer()
    {
        if (!enableSpeedIncrease || speedIncreaseInterval <= 0f)
            return;

        speedIncreaseTimer += Time.deltaTime;

        if (speedIncreaseTimer >= speedIncreaseInterval)
        {
            speedIncreaseTimer -= speedIncreaseInterval;
            IncreasePlatformSpeed();
        }
    }

    private void IncreasePlatformSpeed()
    {
        float previousSpeed = currentFallSpeed;
        currentFallSpeed = Mathf.Min(currentFallSpeed + speedIncreaseAmount, maxFallSpeed);

        if (Mathf.Approximately(previousSpeed, currentFallSpeed))
            return;

        UpdateLinkedManagerSpeeds();
        PlaySpeedIncreaseSfx();
    }

    private void UpdateLinkedManagerSpeeds()
    {
        if (backgroundManager != null)
            backgroundManager.SetScrollSpeed(currentFallSpeed * backgroundSpeedMultiplier);

        if (fallingObjectManager != null)
        {
            float fallingSpeed = currentFallSpeed * fallingObjectSpeedMultiplier;
            fallingObjectManager.SetFallSpeed(fallingSpeed);
        }
    }

    private void PlaySpeedIncreaseSfx()
    {
        if (speedIncreaseAudioSource == null || speedIncreaseSfx == null)
            return;

        speedIncreaseAudioSource.PlayOneShot(speedIncreaseSfx, Mathf.Clamp01(speedIncreaseVolume));
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
        float currentWeight = 0f;

        for (int i = 0; i < spawnableItems.Count; i++)
        {
            SpawnableItem item = spawnableItems[i];

            if (item.prefab == null)
                continue;

            currentWeight += item.weight;

            if (roll <= currentWeight)
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
        if (platforms.Count == 0)
            return GetSpawnY();

        float highest = float.NegativeInfinity;

        for (int i = 0; i < platforms.Count; i++)
        {
            Rigidbody2D rb = platforms[i];

            if (rb == null)
                continue;

            if (rb.position.y > highest)
                highest = rb.position.y;
        }

        if (float.IsNegativeInfinity(highest))
            return GetSpawnY();

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