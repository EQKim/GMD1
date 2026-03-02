using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LavaDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damage = 25;
    [SerializeField] private float damageCooldown = 0.5f;

    [Header("Launch Settings")]
    [SerializeField] private float launchUpVelocity = 12f;

    private float nextDamageTime;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider2D other)
    {
        if (Time.time < nextDamageTime) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) return;

        nextDamageTime = Time.time + damageCooldown;

        // Damage player
        health.TakeDamage(damage);

        // Shoot player upward
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, launchUpVelocity);
        }
    }
}