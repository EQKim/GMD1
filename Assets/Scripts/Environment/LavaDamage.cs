using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
public class LavaDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damage = 25;
    [SerializeField] private float damageCooldown = 0.5f;

    [Header("Launch Settings")]
    [SerializeField] private float launchUpVelocity = 12f;

    [Header("Audio")]
    [SerializeField] private AudioClip lavaHissSfx;
    [Range(0f, 1f)]
    [SerializeField] private float lavaVolume = 1f;

    // Per-target cooldown so multiple players can be damaged/launched independently.
    private readonly Dictionary<PlayerHealth, float> nextDamageTimeByTarget = new Dictionary<PlayerHealth, float>();

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // 2D sound
    }

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnDisable()
    {
        // Avoid holding references after scene/objects are disabled.
        nextDamageTimeByTarget.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Cleanup to prevent the dictionary from growing over time.
        var health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            nextDamageTimeByTarget.Remove(health);
        }
    }

    private void TryDamage(Collider2D other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) return;

        float nextTime;
        if (nextDamageTimeByTarget.TryGetValue(health, out nextTime) && Time.time < nextTime)
            return;

        nextDamageTimeByTarget[health] = Time.time + damageCooldown;

        // Play lava hiss
        if (lavaHissSfx != null)
        {
            audioSource.PlayOneShot(lavaHissSfx, lavaVolume);
        }

        // Damage player
        health.TakeDamage(damage);

        // Launch player upward
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, launchUpVelocity);
        }
    }
}