using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LavaSplashSpawner : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] private GameObject splashPrefab;
    [SerializeField] private float yOffset = 0.05f;

    [Header("Cooldown")]
    [SerializeField] private float spawnCooldown = 0.15f;

    private Collider2D lavaCollider;
    private float nextSpawnTime;

    private void Awake()
    {
        lavaCollider = GetComponent<Collider2D>();
        lavaCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TrySpawn(other);
    }

    private void TrySpawn(Collider2D other)
    {
        if (Time.time < nextSpawnTime)
            return;

        // Only spawn when player hits lava
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null)
            return;

        if (splashPrefab == null)
        {
            Debug.LogWarning("Splash Prefab not assigned.");
            return;
        }

        nextSpawnTime = Time.time + spawnCooldown;

        Bounds lavaBounds = lavaCollider.bounds;
        Bounds playerBounds = other.bounds;

        // X = player's center but clamped inside lava width
        float spawnX = Mathf.Clamp(playerBounds.center.x, lavaBounds.min.x, lavaBounds.max.x);

        // Y = top of lava
        float spawnY = lavaBounds.max.y + yOffset;

        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);

        Instantiate(splashPrefab, spawnPos, Quaternion.identity);
    }
}