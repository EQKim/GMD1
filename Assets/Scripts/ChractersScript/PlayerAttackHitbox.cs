using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class PlayerAttackHitbox : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 10;

    [Header("Weapon Effects")]
    [SerializeField] private float knockbackForce = 8f;

    [Tooltip("Upward multiplier applied to knockback force (0..1 is typical).")]
    [SerializeField] private float knockbackUpwardMultiplier = 0.25f;

    [SerializeField] private bool enableBleed = true;

    [Header("Target Filtering")]
    [SerializeField] private LayerMask targetLayers;

    [Header("Hit FX")]
    [SerializeField] private ParticleSystem bloodEffectPrefab;
    [SerializeField] private Vector3 bloodSpawnOffset = Vector3.zero;

    private Collider2D hitboxCollider;
    private PlayerHealth ownerHealth;
    private bool attackActive;

    private Transform ownerVisual;
    private Transform ownerRoot;

    private readonly HashSet<PlayerHealth> hitTargets = new HashSet<PlayerHealth>();

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider2D>();
        hitboxCollider.isTrigger = true;
        hitboxCollider.enabled = false;

        ownerHealth = GetComponentInParent<PlayerHealth>();
        ownerRoot = ownerHealth != null ? ownerHealth.transform : transform;

        if (ownerHealth != null)
        {
            Transform t = ownerHealth.transform.Find("Visual");
            ownerVisual = t != null ? t : ownerHealth.transform;
        }
        else
        {
            ownerVisual = transform;
        }
    }

    public void SetDamageValue(int newDamage)
    {
        damage = newDamage;
    }

    public void SetWeaponEffects(float knockbackForceAmount, bool bleedEnabled)
    {
        knockbackForce = Mathf.Max(0f, knockbackForceAmount);
        enableBleed = bleedEnabled;
    }

    public void SetWeaponEffects(bool knockbackEnabled, float knockbackAmount, bool bleedEnabled)
    {
        SetWeaponEffects(knockbackEnabled ? knockbackAmount : 0f, bleedEnabled);
    }

    public int GetDamage() => damage;

    public void EnableAttack()
    {
        attackActive = true;
        hitTargets.Clear();
        hitboxCollider.enabled = true;

        PlayerWeaponHolder holder = GetComponentInParent<PlayerWeaponHolder>();
        if (holder != null)
            holder.ResetAttackSfxGate();
    }

    public void DisableAttack()
    {
        attackActive = false;
        hitboxCollider.enabled = false;
        hitTargets.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other) => TryHit(other);
    private void OnTriggerStay2D(Collider2D other) => TryHit(other);

    private void TryHit(Collider2D other)
    {
        if (!attackActive)
            return;

        if (((1 << other.gameObject.layer) & targetLayers) == 0)
            return;

        FlyingDemonAI demon = other.GetComponentInParent<FlyingDemonAI>();
        if (demon != null)
        {
            demon.TakeDamage(damage);

            PlayerWeaponHolder holder = GetComponentInParent<PlayerWeaponHolder>();
            if (holder != null)
                holder.PlayAttackSfx();

            if (enableBleed)
                SpawnBloodEffect(other);

            return;
        }

        PlayerHealth targetHealth = other.GetComponentInParent<PlayerHealth>();
        if (targetHealth == null)
            return;

        if (targetHealth == ownerHealth)
            return;

        if (hitTargets.Contains(targetHealth))
            return;

        if (targetHealth.transform == ownerRoot || targetHealth.transform.IsChildOf(ownerRoot))
            return;

        bool didDamage = targetHealth.TakeDamage(damage);
        if (!didDamage)
            return;

        hitTargets.Add(targetHealth);

        PlayerWeaponHolder weaponHolder = GetComponentInParent<PlayerWeaponHolder>();
        if (weaponHolder != null)
            weaponHolder.PlayAttackSfx();

        if (IsKnockbackEnabled())
            ApplyKnockback(targetHealth);

        if (enableBleed)
            SpawnBloodEffect(other);
    }

    private void OnDrawGizmosSelected()
    {
        if (targetLayers == 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.2f);
        }
    }

    private bool IsKnockbackEnabled() => knockbackForce > 0.0001f;

    private void ApplyKnockback(PlayerHealth targetHealth)
    {
        Rigidbody2D targetRb = targetHealth.GetComponent<Rigidbody2D>();
        if (targetRb == null)
            targetRb = targetHealth.GetComponentInParent<Rigidbody2D>();

        if (targetRb == null)
            return;

        float direction = 1f;
        if (ownerVisual != null)
            direction = ownerVisual.localScale.x >= 0f ? 1f : -1f;

        targetRb.linearVelocity = new Vector2(0f, targetRb.linearVelocity.y);

        Vector2 impulse = new Vector2(
            direction * knockbackForce,
            knockbackForce * Mathf.Max(0f, knockbackUpwardMultiplier)
        );

        targetRb.AddForce(impulse, ForceMode2D.Impulse);

        PlayerController2D targetController = targetHealth.GetComponent<PlayerController2D>();
        if (targetController == null)
            targetController = targetHealth.GetComponentInParent<PlayerController2D>();

        if (targetController != null)
            targetController.ApplyKnockbackLock(0f);
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