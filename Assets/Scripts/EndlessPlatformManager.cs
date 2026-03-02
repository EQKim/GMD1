using System.Collections.Generic;
using UnityEngine;

public class EndlessPlatformManager : MonoBehaviour
{
    [Header("Prefab / Pool")]
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private int poolSize = 12;

    [Header("Movement")]
    [SerializeField] private float fallSpeed = 2.5f;

    [Header("Camera-based bounds")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float verticalBuffer = 1.5f;

    [Header("Horizontal range (world X)")]
    [SerializeField] private float minX = -4f;
    [SerializeField] private float maxX = 4f;

    [Header("Vertical spacing")]
    [SerializeField] private float minGapY = 0.8f;
    [SerializeField] private float maxGapY = 1.6f;

    private readonly List<Rigidbody2D> platforms = new();

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

        float spawnY = GetSpawnY();
        float y = spawnY;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject go = Instantiate(platformPrefab, transform);

            Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("Platform prefab must have a Rigidbody2D (set to Kinematic).");
                enabled = false;
                return;
            }

            float x = Random.Range(minX, maxX);
            rb.position = new Vector2(x, y);

            platforms.Add(rb);

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
                float highestY = GetHighestPlatformY();
                float newY = highestY + Random.Range(minGapY, maxGapY);
                float newX = Random.Range(minX, maxX);
                p = new Vector2(newX, newY);
            }

            rb.MovePosition(p);
        }
    }

    private float GetHighestPlatformY()
    {
        float highest = float.NegativeInfinity;
        for (int i = 0; i < platforms.Count; i++)
        {
            float y = platforms[i].position.y;
            if (y > highest) highest = y;
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