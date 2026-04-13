using System.Collections;
using UnityEngine;

public class FlyingDemonSpawner : MonoBehaviour
{
    [System.Serializable]
    public class DemonLane
    {
        public string laneName;
        public Transform targetPlayer;
        public Transform spawnPoint;

        [HideInInspector] public FlyingDemonAI activeDemon;
        [HideInInspector] public Coroutine respawnRoutine;
    }

    [Header("Prefab")]
    [SerializeField] private GameObject flyingDemonPrefab;

    [Header("Spawn Delay")]
    [SerializeField] private float minSpawnDelay = 30f;
    [SerializeField] private float maxSpawnDelay = 50f;

    [Header("Random Spawn Offset")]
    [SerializeField] private Vector2 randomSpawnOffsetMin = new Vector2(-2f, 1f);
    [SerializeField] private Vector2 randomSpawnOffsetMax = new Vector2(2f, 4f);

    [Header("Per Player Demon Lanes")]
    [SerializeField] private DemonLane player1Lane;
    [SerializeField] private DemonLane player2Lane;

    [Header("Cleanup")]
    [SerializeField] private string enemyProjectileTag = "EnemyProjectile";

    private bool spawnerActive;

    private void Start()
    {
        // Do not auto-start here.
        // GameStartScreen will control when the match begins.
        spawnerActive = false;
    }

    public void BeginMatch()
    {
        spawnerActive = true;

        StartLaneTimer(player1Lane);
        StartLaneTimer(player2Lane);
    }

    public void StopAndClear()
    {
        spawnerActive = false;

        StopLane(player1Lane);
        StopLane(player2Lane);

        DestroyActiveDemon(player1Lane);
        DestroyActiveDemon(player2Lane);

        DestroyAllEnemyProjectiles();
    }

    private void StopLane(DemonLane lane)
    {
        if (lane == null)
            return;

        if (lane.respawnRoutine != null)
        {
            StopCoroutine(lane.respawnRoutine);
            lane.respawnRoutine = null;
        }
    }

    private void DestroyActiveDemon(DemonLane lane)
    {
        if (lane == null)
            return;

        if (lane.activeDemon != null)
        {
            Destroy(lane.activeDemon.gameObject);
            lane.activeDemon = null;
        }
    }

    private void DestroyAllEnemyProjectiles()
    {
        if (string.IsNullOrWhiteSpace(enemyProjectileTag))
            return;

        GameObject[] projectiles = GameObject.FindGameObjectsWithTag(enemyProjectileTag);

        for (int i = 0; i < projectiles.Length; i++)
        {
            if (projectiles[i] != null)
                Destroy(projectiles[i]);
        }
    }

    private void StartLaneTimer(DemonLane lane)
    {
        if (!spawnerActive || lane == null)
            return;

        if (lane.targetPlayer == null || lane.spawnPoint == null)
        {
            Debug.LogWarning($"FlyingDemonSpawner: Lane '{lane.laneName}' is missing targetPlayer or spawnPoint.");
            return;
        }

        if (lane.activeDemon != null)
            return;

        if (lane.respawnRoutine != null)
            StopCoroutine(lane.respawnRoutine);

        lane.respawnRoutine = StartCoroutine(RespawnLaneRoutine(lane));
    }

    private IEnumerator RespawnLaneRoutine(DemonLane lane)
    {
        float waitTime = Random.Range(minSpawnDelay, maxSpawnDelay);
        yield return new WaitForSeconds(waitTime);

        lane.respawnRoutine = null;

        if (!spawnerActive)
            yield break;

        SpawnForLane(lane);
    }

    private void SpawnForLane(DemonLane lane)
    {
        if (!spawnerActive)
            return;

        if (flyingDemonPrefab == null)
        {
            Debug.LogWarning("FlyingDemonSpawner: flyingDemonPrefab is missing.");
            return;
        }

        if (lane.activeDemon != null)
            return;

        Vector3 randomOffset = new Vector3(
            Random.Range(randomSpawnOffsetMin.x, randomSpawnOffsetMax.x),
            Random.Range(randomSpawnOffsetMin.y, randomSpawnOffsetMax.y),
            0f
        );

        Vector3 spawnPosition = lane.spawnPoint.position + randomOffset;

        GameObject demonObject = Instantiate(flyingDemonPrefab, spawnPosition, Quaternion.identity);

        FlyingDemonAI demon = demonObject.GetComponent<FlyingDemonAI>();
        if (demon == null)
        {
            Debug.LogWarning("FlyingDemonSpawner: Spawned prefab is missing FlyingDemonAI.");
            Destroy(demonObject);
            return;
        }

        demon.Initialize(lane.targetPlayer, this);
        lane.activeDemon = demon;
    }

    public void NotifyDemonDied(FlyingDemonAI deadDemon)
    {
        if (!spawnerActive || deadDemon == null)
            return;

        if (player1Lane.activeDemon == deadDemon)
        {
            player1Lane.activeDemon = null;
            StartLaneTimer(player1Lane);
            return;
        }

        if (player2Lane.activeDemon == deadDemon)
        {
            player2Lane.activeDemon = null;
            StartLaneTimer(player2Lane);
        }
    }
}