using UnityEngine;

public class FlyingDemonFireball : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private int damage = 15;
    [SerializeField] private float lifeTime = 5f;

    [Header("Collision")]
    [SerializeField] private LayerMask playerLayers;

    [Header("Hit FX")]
    [SerializeField] private ParticleSystem bloodEffectPrefab;
    [SerializeField] private Vector3 bloodSpawnOffset = Vector3.zero;

    private Vector2 moveDirection;
    private Transform target;

    public void Initialize(Vector2 direction, Transform assignedTarget)
    {
        moveDirection = direction.normalized;
        target = assignedTarget;

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayers) == 0)
            return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null)
            health = other.GetComponentInParent<PlayerHealth>();

        if (health == null)
            return;

        if (target != null && health.transform != target)
            return;

        bool didDamage = health.TakeDamage(damage);

        if (didDamage)
            SpawnBloodEffect(other);

        Destroy(gameObject);
    }

    private void SpawnBloodEffect(Collider2D other)
    {
        if (bloodEffectPrefab == null)
            return;

        Vector3 spawnPos = other.bounds.center + bloodSpawnOffset;

        ParticleSystem fx = Instantiate(bloodEffectPrefab, spawnPos, Quaternion.identity);
        fx.Play();

        Destroy(fx.gameObject, 2f);
    }
}