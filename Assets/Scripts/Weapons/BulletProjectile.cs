using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private int damage = 10;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float upwardKnockback = 0.15f;
    [SerializeField] private float knockbackLockDuration = 0.18f;

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
        FlyingDemonAI demon = other.GetComponentInParent<FlyingDemonAI>();

        if (demon != null)
        {
            demon.TakeDamage(damage);
            ApplyKnockback(other);
            SpawnBloodEffect(other);
            Destroy(gameObject);
            return;
        }

        PlayerHealth target = other.GetComponentInParent<PlayerHealth>();

        if (target != null && target != owner)
        {
            bool didDamage = target.TakeDamage(damage);

            if (didDamage)
            {
                ApplyKnockback(other);
                SpawnBloodEffect(other);
            }

            Destroy(gameObject);
            return;
        }

        if (!other.isTrigger)
            Destroy(gameObject);
    }

    private void ApplyKnockback(Collider2D other)
    {
        Rigidbody2D targetRb = other.GetComponentInParent<Rigidbody2D>();
        PlayerController2D targetController = other.GetComponentInParent<PlayerController2D>();

        if (targetRb == null)
            return;

        float xDirection = Mathf.Sign(direction.x);

        if (xDirection == 0f)
            xDirection = transform.localScale.x >= 0f ? 1f : -1f;

        Vector2 knockbackDirection = new Vector2(xDirection, upwardKnockback).normalized;
        Vector2 force = knockbackDirection * knockbackForce;

        if (targetController != null)
            targetController.ApplyKnockbackLock(knockbackLockDuration);

        targetRb.linearVelocity = Vector2.zero;
        targetRb.AddForce(force, ForceMode2D.Impulse);
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