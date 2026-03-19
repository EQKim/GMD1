using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private int damage = 10;

    [Header("Hit FX")]
    [SerializeField] private ParticleSystem bloodEffectPrefab;
    [SerializeField] private Vector3 bloodSpawnOffset = Vector3.zero;

    private Vector2 direction;
    private Rigidbody2D rb;
    private PlayerHealth owner;

    public void Initialize(Vector2 dir, int dmg, PlayerHealth ownerRef)
    {
        direction = dir.normalized;
        damage = dmg;
        owner = ownerRef;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth target = other.GetComponentInParent<PlayerHealth>();

        if (target != null && target != owner)
        {
            bool didDamage = target.TakeDamage(damage);

            if (didDamage)
                SpawnBloodEffect(other);

            Destroy(gameObject);
            return;
        }

        if (!other.isTrigger)
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